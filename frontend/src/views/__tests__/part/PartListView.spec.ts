import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import { useAuthStore } from '../../../stores/auth';
import PartListView from '../../part/PartListView.vue';

// Mock common components
const CardStub = { 
  name: 'Card',
  template: '<div class="card-stub"><slot></slot></div>' 
};
const DotStub = { 
  name: 'Dot',
  template: '<div class="dot-stub"></div>' 
};
const ButtonStub = { 
  name: 'Button',
  template: '<button class="app-button-stub"><slot></slot></button>' 
};

/**
 * Part List View Component Tests (零件清單組件測試)
 */
vi.mock('../../../services/part/part', () => ({
  partService: {
    getParts: vi.fn().mockResolvedValue([]),
    getSuppliers: vi.fn().mockResolvedValue(['Supplier A', 'Supplier B']),
    getCustomers: vi.fn().mockResolvedValue([{ id: 'customer001', name: 'Test Customer' }])
  }
}));

vi.mock('../../../services/common/common', () => ({
  commonService: {
    getCustomers: vi.fn().mockResolvedValue([{ key: 'customer001', value: 'Test Customer' }]),
    getStatusOptions: vi.fn().mockResolvedValue([{ key: 'ACTIVE', value: 'Active' }]),
    getSuppliers: vi.fn().mockResolvedValue([{ key: 'S001', value: 'Supplier A' }])
  }
}));

// Mock vue-router
const pushSpy = vi.fn();
vi.mock('vue-router', () => ({
  useRoute: () => ({ query: {} }),
  useRouter: () => ({ push: pushSpy })
}));

import { createPinia } from 'pinia';

// ... (existing mocks) ...

describe('PartListView.vue', () => {
  const globalConfig = {
    global: {
      plugins: [createPinia()],
      mocks: { $t: (key: string) => key },
      components: {
        Card: CardStub,
        Dot: DotStub,
        Button: ButtonStub
      },
      stubs: {
        'el-select': { template: '<div class="el-select"><slot></slot></div>' },
        'el-option': { template: '<div class="el-option"></div>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Setup Auth Store state
    const authStore = useAuthStore();
    authStore.setAuth('mock-token', {
      id: 'customer001',
      name: 'Test Customer',
      role: 'CUSTOMER'
    });
  });

  it('renders correctly (正確渲染)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await flushPromises();
    expect(wrapper.find('h1').text()).toBe('part_list.title');
  });

  it('navigates to create page when add button is clicked (點擊新增按鈕時導航至建立頁)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    await flushPromises();
    
    const addButton = wrapper.find('.app-button-stub');
    expect(addButton.exists()).toBe(true);
    await addButton.trigger('click');
    
    expect(pushSpy).toHaveBeenCalledWith({ name: 'part-create' });
  });

  it('performs keyword search (執行關鍵字搜尋)', async () => {
    const wrapper = mount(PartListView, globalConfig);
    
    // Manually set parts and loading to simulate fetched data
    const vm = wrapper.vm as any;
    vm.parts = MOCK_PARTS_DATA;
    vm.loading = false;
    await flushPromises();
    
    vm.searchQuery = 'PN-2024-001';
    await flushPromises();
    
    // Check if table exists
    const table = wrapper.find('table');
    if (!table.exists()) {
      // console.log('DEBUG - HTML:', wrapper.html());
    }
    
    const rows = wrapper.findAll('tbody tr');
    expect(rows.length).toBeGreaterThan(0);
    expect(rows[0].text()).toContain('PN-2024-001');
  });
});

const MOCK_PARTS_DATA = [
  { 
    id: 1, 
    partNo: 'PN-2024-001', 
    htsCode: '8517.12.00', 
    status: 'ACTIVE', 
    supplier: 'Supplier A', 
    customerId: 'customer001',
    customerName: 'Test Customer', 
    updatedDate: '2026-04-16' 
  }
];
