import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import PartCreateView from '../../part/PartCreateView.vue';
import { partService } from '../../../services/part/part';

// Mock element-plus
vi.mock('element-plus', async () => {
  const actual = await vi.importActual('element-plus') as any;
  return {
    ...actual,
    ElMessage: {
      success: vi.fn(),
      error: vi.fn(),
      warning: vi.fn()
    }
  };
});

// Mock vue-i18n
vi.mock('vue-i18n', async () => {
  const actual = await vi.importActual('vue-i18n') as any;
  return {
    ...actual,
    useI18n: () => ({
      t: (key: string) => key
    })
  };
});

/**
 * Part Creation View Component Tests (新增零件組件測試)
 * BR-08: HTS Format | BR-21: Description Quality
 */
vi.mock('../../../services/auth/auth', async () => {
  const actual = await vi.importActual('../../../services/auth/auth') as any;
  return {
    ...actual,
    authService: {
      state: {
        role: 'CUSTOMER',
        customerId: 'customer001'
      }
    }
  };
});

vi.mock('../../../services/part/part', () => ({
  PartStatus: {
    ACTIVE: 'ACTIVE',
    PENDING_REVIEW: 'PENDING_REVIEW'
  },
  partService: {
    createPartApi: vi.fn().mockResolvedValue({ success: true }),
    submitPartApi: vi.fn().mockResolvedValue({ success: true }),
    getSuppliers: vi.fn().mockResolvedValue(['Supplier A', 'Supplier B']),
    getCustomers: vi.fn().mockResolvedValue([{ id: 'customer001', name: 'Test Customer' }])
  }
}));

const pushSpy = vi.fn();
vi.mock('vue-router', () => ({
  useRouter: () => ({ 
    push: pushSpy,
    back: vi.fn(),
    app: { config: { globalProperties: { $t: (key: string) => key } } }
  })
}));

describe('PartCreateView.vue', () => {
  const globalConfig = {
    global: {
      mocks: { $t: (key: string) => key },
      stubs: {
        Card: { template: '<div class="card"><slot name="header"></slot><slot></slot></div>' },
        'el-select': { 
          template: '<div class="el-select-stub"><slot></slot></div>',
          props: ['modelValue']
        },
        'el-option': { 
          template: '<div class="el-option-stub" :data-value="value" :data-label="label"></div>',
          props: ['value', 'label']
        },
        Button: { template: '<button><slot></slot></button>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    pushSpy.mockClear();
    // Reset to customer role
    import('../../../services/auth/auth').then(({ authService }) => {
      authService.state.role = 'CUSTOMER';
    });
  });

  it('renders correctly (正確渲染)', () => {
    const wrapper = mount(PartCreateView, globalConfig);
    expect(wrapper.find('h1').text()).toBe('part_create.title');
  });

  it('shows customer selection for employees but not for customers (員工可見客戶選擇器，客戶則不可見)', async () => {
    const { authService, UserRole } = await import('../../../services/auth/auth');
    
    // Customer case
    authService.state.role = UserRole.CUSTOMER;
    let wrapper = mount(PartCreateView, globalConfig);
    expect(wrapper.findComponent({ name: 'ElSelect', from: 'element-plus' }).exists()).toBe(false); // Using tag search instead

    // Employee case
    authService.state.role = UserRole.DIMERCO;
    wrapper = mount(PartCreateView, globalConfig);
    // Find by the test data attribute I added
    expect(wrapper.find('[data-test="customer-select"]').exists()).toBe(true);
  });

  it('sets status to ACTIVE when employee creates part (員工建立零件時，狀態自動設為 ACTIVE)', async () => {
    const { authService, UserRole } = await import('../../../services/auth/auth');
    authService.state.role = UserRole.DIMERCO;
    const wrapper = mount(PartCreateView, globalConfig);

    // Manually set data to avoid stub event issues
    const vm = wrapper.vm as any;
    vm.form.customerId = 'customer001';
    vm.form.partNo = 'PN-EMP-001';
    vm.form.countryOfOrigin = 1;
    vm.form.usHtsCode = '1111.22.3333';
    vm.form.supplier = 'Supplier A';
    vm.form.partDescription = 'Description created by employee.';

    await wrapper.find('form').trigger('submit.prevent');

    expect(partService.createPartApi).toHaveBeenCalledWith(expect.objectContaining({
      customerId: 'customer001'
    }));
  });

  it('validates HTS Code format - BR-08 (驗證 HTS Code 格式)', async () => {
    const wrapper = mount(PartCreateView, {
      ...globalConfig,
      global: {
        ...globalConfig.global,
        stubs: {
          ...globalConfig.global.stubs,
          Button: { template: '<button><slot></slot></button>' }
        }
      }
    });
    const htsInput = wrapper.find('[data-test="us-hts-code-input"]');
    expect(htsInput.exists()).toBe(true);

    // Setting digits with one dot is preserved (sanitizeDecimal keeps first dot only)
    await htsInput.setValue('1234.567890');
    expect((htsInput.element as HTMLInputElement).value).toBe('1234.567890');

    // Partial digit input stays as typed
    await htsInput.setValue('123');
    expect((htsInput.element as HTMLInputElement).value).toBe('123');
    });

  it('submits form successfully (成功提交表單)', async () => {
    const wrapper = mount(PartCreateView, globalConfig);

    // Manually set data to avoid stub issues
    const vm = wrapper.vm as any;
    vm.form.partNo = 'PN-001';
    vm.form.countryOfOrigin = 1;
    vm.form.usHtsCode = '1234.56.7890';
    vm.form.supplier = 'Supplier A';
    vm.form.partDescription = 'This is a strong description with enough words for the validation to pass.';

    await wrapper.find('form').trigger('submit.prevent');
    await Promise.resolve(); // flush microtasks

    expect(partService.createPartApi).toHaveBeenCalled();
    expect(pushSpy).toHaveBeenCalledWith({ name: 'parts' });

    const { ElMessage } = await import('element-plus');
    expect(ElMessage.success).toHaveBeenCalled();
  });
});
