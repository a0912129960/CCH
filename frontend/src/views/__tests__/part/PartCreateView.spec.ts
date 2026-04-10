import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import PartCreateView from '../../part/PartCreateView.vue';
import { partService } from '../../../services/part/part';

// Mock element-plus
vi.mock('element-plus', () => ({
  ElMessage: {
    success: vi.fn(),
    error: vi.fn(),
    warning: vi.fn()
  }
}));

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key
  })
}));

/**
 * Part Creation View Component Tests (新增零件組件測試)
 * BR-08: HTS Format | BR-21: Description Quality
 */
vi.mock('../../../services/auth/auth', () => ({
  authService: {
    state: {
      role: 'CUSTOMER',
      customerId: 'customer001'
    }
  }
}));

vi.mock('../../../services/part/part', () => ({
  PartStatus: {
    ACTIVE: 'ACTIVE',
    PENDING_REVIEW: 'PENDING_REVIEW'
  },
  partService: {
    createPart: vi.fn().mockResolvedValue({ id: '999', partNo: 'TEST-PN' }),
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
    const { authService } = await import('../../../services/auth/auth');
    
    // Customer case
    authService.state.role = 'CUSTOMER';
    let wrapper = mount(PartCreateView, globalConfig);
    expect(wrapper.findComponent({ name: 'ElSelect', from: 'element-plus' }).exists()).toBe(false); // Using tag search instead

    // Employee case
    authService.state.role = 'EMPLOYEE';
    wrapper = mount(PartCreateView, globalConfig);
    // Find by the test data attribute I added
    expect(wrapper.find('[data-test="customer-select"]').exists()).toBe(true);
  });

  it('sets status to ACTIVE when employee creates part (員工建立零件時，狀態自動設為 ACTIVE)', async () => {
    const { authService } = await import('../../../services/auth/auth');
    authService.state.role = 'EMPLOYEE';
    const wrapper = mount(PartCreateView, globalConfig);
    
    // Manually set data to avoid stub event issues
    const vm = wrapper.vm as any;
    vm.form.customerId = 'customer001';
    vm.form.partNo = 'PN-EMP-001';
    vm.form.htsCode = '1111.22.3333';
    vm.form.supplier = 'Supplier A';
    vm.form.description = 'Description created by employee.';
    
    await wrapper.find('form').trigger('submit.prevent');
    
    expect(partService.createPart).toHaveBeenCalledWith(expect.objectContaining({
      status: 'ACTIVE',
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
    const htsInput = wrapper.find('[data-test="hts-code-input"]');
    
    // Test filtering: should remove letters and format dots
    await htsInput.setValue('1234abcd56ef7890');
    expect((htsInput.element as HTMLInputElement).value).toBe('1234.56.7890');

    // Test truncation: should not exceed 10 digits
    await htsInput.setValue('12345678901234');
    expect((htsInput.element as HTMLInputElement).value).toBe('1234.56.7890');

    // Test partial input
    await htsInput.setValue('123');
    expect((htsInput.element as HTMLInputElement).value).toBe('123');
    // Error for hts format should be gone
    expect(wrapper.find('[data-test="hts-code-error"]').exists()).toBe(false);
    });

  it('submits form successfully (成功提交表單)', async () => {
    const wrapper = mount(PartCreateView, globalConfig);
    
    // Manually set data to avoid stub issues
    const vm = wrapper.vm as any;
    vm.form.partNo = 'PN-001';
    vm.form.htsCode = '1234.56.7890';
    vm.form.supplier = 'Supplier A';
    vm.form.description = 'This is a strong description with enough words for the validation to pass.';
    
    await wrapper.find('form').trigger('submit.prevent');
    
    expect(partService.createPart).toHaveBeenCalled();
    expect(pushSpy).toHaveBeenCalledWith({ name: 'parts' });
    
    const { ElMessage } = await import('element-plus');
    expect(ElMessage.success).toHaveBeenCalled();
  });
});
