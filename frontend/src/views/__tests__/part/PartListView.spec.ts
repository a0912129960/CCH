import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import { createPinia, setActivePinia } from 'pinia';
import PartListView from '../../part/PartListView.vue';

/**
 * Part List View Component Tests (零件清單組件測試)
 */
vi.mock('../../../services/part/part', async () => {
  const actual = await vi.importActual('../../../services/part/part') as any;
  return {
    ...actual,
    partService: {
      getParts: vi.fn().mockResolvedValue(actual.MOCK_PARTS),
      getSuppliers: vi.fn().mockResolvedValue(['Supplier A', 'Supplier B']),
      getCustomers: vi.fn().mockResolvedValue([{ key: 'customer001', value: 'Test Customer' }])
    }
  };
});

vi.mock('../../../services/auth/auth', async () => {
  const actual = await vi.importActual('../../../services/auth/auth') as any;
  return {
    ...actual,
    authService: {
      state: {
        role: 'DIMERCO',
        customerId: undefined,
        isLoggedIn: true,
        username: 'test-admin'
      }
    }
  };
});

vi.mock('vue-i18n', () => ({
  useI18n: () => ({ t: (key: string) => key })
}));

const mockRoute = { query: { status: '' } };
const pushSpy = vi.fn();
vi.mock('vue-router', () => ({
  useRoute: () => mockRoute,
  useRouter: () => ({ push: pushSpy })
}));

describe('PartListView.vue', () => {
  const globalConfig = {
    global: {
      plugins: [createPinia()],
      mocks: { $t: (key: string) => key },
      stubs: {
        Card: { template: '<div class="card"><slot></slot></div>' },
        Dot: { template: '<div class="dot"></div>' },
        Button: { template: '<button class="app-button"><slot></slot></button>' },
        'el-select': { template: '<div class="el-select"><slot></slot></div>' },
        'el-option': { template: '<div class="el-option"></div>' }
      }
    }
  };

  beforeEach(() => {
    setActivePinia(createPinia());
    vi.clearAllMocks();
    mockRoute.query.status = '';
    pushSpy.mockClear();
  });

  it('renders correctly (正確渲染)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));
    expect(wrapper.find('h1').text()).toBe('part_list.title');
  });

  it('renders customer select for employee (員工應看到客戶下拉選單)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));
    expect(wrapper.find('.customer-select').exists()).toBe(true);
  });

  it('navigates to create page when add button is clicked (點擊新增按鈕時導航至建立頁)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));
    
    const addButton = wrapper.findComponent({ name: 'Button' });
    if (!addButton.exists()) {
      // Fallback to class if findComponent fails due to stubbing
      const btn = wrapper.find('.app-button');
      await btn.trigger('click');
    } else {
      await addButton.trigger('click');
    }
    
    expect(pushSpy).toHaveBeenCalledWith({ name: 'part-create' });
  });

  it('performs keyword search (執行關鍵字搜尋)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));
    
    const searchInput = wrapper.find('input[type="text"]');
    await searchInput.setValue('PN-2024-001');
    await wrapper.vm.$nextTick();
    
    const rows = wrapper.findAll('tbody tr');
    expect(rows.length).toBeGreaterThan(0);
  });

  it('navigates to detail page when Part No link is clicked (點擊零件編號連結時導航至詳情頁)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));
    
    const link = wrapper.find('.part-no-cell a');
    await link.trigger('click');
    
    expect(pushSpy).toHaveBeenCalled();
    const callArgs = pushSpy.mock.calls[0][0];
    expect(callArgs.name).toBe('part-detail');
    expect(callArgs.params).toHaveProperty('id');
  });
});
