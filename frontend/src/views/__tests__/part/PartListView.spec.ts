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
      createPart: vi.fn(),
      updatePartStatus: vi.fn()
    }
  };
});

vi.mock('../../../services/common/common', async () => {
  const actual = await vi.importActual('../../../services/common/common') as any;
  return {
    ...actual,
    commonService: {
      getCustomers: vi.fn().mockResolvedValue([{ key: 'customer001', value: 'Test Customer' }]),
      getStatusOptions: vi.fn().mockResolvedValue([{ key: 'ACTIVE', value: 'Active' }]),
      getSuppliers: vi.fn().mockResolvedValue([{ key: 'S001', value: 'Supplier A' }])
    }
  };
});

vi.mock('../../../services/auth/auth', async () => {
  const actual = await vi.importActual('../../../services/auth/auth') as any;
  return {
    ...actual,
    authService: {
      state: {
        role: 'CUSTOMER',
        customerId: 'customer001',
        isLoggedIn: true,
        username: 'test-user'
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
    // Manually trigger data load for test stability
    (wrapper.vm as any).parts = MOCK_PARTS_DATA;
    await wrapper.vm.$nextTick();
    expect(wrapper.find('h1').text()).toBe('part_list.title');
  });

  it('renders customer select (應看到客戶下拉選單)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await wrapper.vm.$nextTick();
    // In template it's always rendered now, but label depends on locale
    expect(wrapper.find('.customer-select').exists()).toBe(true);
  });

  it('navigates to create page when add button is clicked (點擊新增按鈕時導航至建立頁)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await wrapper.vm.$nextTick();
    
    const addButton = wrapper.findComponent({ name: 'Button' });
    if (!addButton.exists()) {
      const btn = wrapper.find('.app-button');
      await btn.trigger('click');
    } else {
      await addButton.trigger('click');
    }
    
    expect(pushSpy).toHaveBeenCalledWith({ name: 'part-create' });
  });

  it('performs keyword search (執行關鍵字搜尋)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    // Explicitly set the underlying refs to ensure filteredParts has data
    (wrapper.vm as any).parts = [...MOCK_PARTS_DATA];
    (wrapper.vm as any).loading = false;
    (wrapper.vm as any).customerFilter = 'customer001';
    await wrapper.vm.$nextTick();
    
    (wrapper.vm as any).searchQuery = 'PN-2024-001';
    await wrapper.vm.$nextTick();
    
    const filtered = (wrapper.vm as any).filteredParts;
    // console.log('DEBUG - Filtered Parts:', JSON.stringify(filtered));
    // console.log('DEBUG - Query:', (wrapper.vm as any).searchQuery);
    
    const rows = wrapper.findAll('tbody tr');
    expect(rows.length).toBeGreaterThan(0);
  });

  it('navigates to detail page when Part No link is clicked (點擊零件編號連結時導航至詳情頁)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    (wrapper.vm as any).parts = MOCK_PARTS_DATA;
    (wrapper.vm as any).loading = false;
    await wrapper.vm.$nextTick();
    
    const link = wrapper.find('.part-no-cell a');
    expect(link.exists()).toBe(true);
    await link.trigger('click');
    
    expect(pushSpy).toHaveBeenCalled();
    const callArgs = pushSpy.mock.calls[0][0];
    expect(callArgs.name).toBe('part-detail');
  });
});

const MOCK_PARTS_DATA = [
  { id: '1', partNo: 'PN-2024-001', htsCode: '8517.12.00', status: 'ACTIVE', supplier: 'Supplier A', customerId: 'customer001', customerName: 'Test Customer', lastUpdated: '2026-04-16' }
];
