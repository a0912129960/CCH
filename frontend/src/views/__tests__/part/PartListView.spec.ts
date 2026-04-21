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
vi.mock('../../../services/part/part', () => {
  const mockPartsData = [
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
  return {
    partService: {
      getParts: vi.fn().mockResolvedValue({ data: mockPartsData, total: 1 }),
      exportPartsToExcel: vi.fn().mockResolvedValue(true)
    },
    batchAcceptParts: vi.fn().mockResolvedValue({ success: true })
  };
});

vi.mock('../../../services/common/common', () => ({
  commonService: {
    getCustomers: vi.fn().mockResolvedValue([{ key: 'customer001', value: 'Test Customer' }]),
    getStatusOptions: vi.fn().mockResolvedValue([{ key: 'ACTIVE', value: 'Active' }]),
    getSuppliers: vi.fn().mockResolvedValue([{ key: 'S001', value: 'Supplier A' }]),
    formatDateTime: vi.fn().mockReturnValue('2026-04-21 12:00:00')
  }
}));

// Mock vue-router
const pushSpy = vi.fn();
vi.mock('vue-router', () => ({
  useRoute: () => ({ query: {} }),
  useRouter: () => ({ push: pushSpy })
}));

import { createPinia } from 'pinia';

describe('PartListView.vue', () => {
  const globalConfig = {
    global: {
      plugins: [createPinia()],
      stubs: {
        'el-select': { template: '<div class="el-select"><slot></slot></div>' },
        'el-option': { template: '<div class="el-option"></div>' },
        'el-pagination': true,
        'el-checkbox': true,
        'el-icon': true,
        Card: CardStub,
        Dot: DotStub,
        Button: ButtonStub
      },
      mocks: {
        $t: (key: string) => key
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
    await flushPromises();
    
    const vm = wrapper.vm as any;
    vm.searchQuery = 'PN-2024-001';
    await flushPromises();
    
    const rows = wrapper.findAll('tbody tr');
    expect(rows.length).toBeGreaterThan(0);
    expect(rows[0].text()).toContain('PN-2024-001');
  });
});
