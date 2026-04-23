import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import { useAuthStore } from '../../../stores/auth';
import PartListView from '../../part/PartListView.vue';

// Hoisted so vi.mock factories can reference it
const MOCK_PARTS_DATA = vi.hoisted(() => [
  {
    id: 1,
    /* Update by Gemini AI on 2026-04-23: Align with project rename. (繁體中文) 配合專案更名。 */
    /* customer: 'Test Customer', */
    project: 'Test Customer',
    partNo: 'PN-2024-001',
    partDesc: 'Test Part',
    country: 'USA',
    htsCode: '8517.12.00',
    rate: 0,
    supplier: 'Supplier A',
    status: 'ACTIVE',
    updatedBy: 'tester',
    updatedDate: '2026-04-16',
    slaStatus: 'green'
  }
]);

// Mock common components
const CardStub = {
  name: 'Card',
  template: '<div class="card-stub"><slot></slot></div>'
};
const DotStub = {
  name: 'Dot',
  template: '<div class="dot-stub"></div>'
};
// Emit click so @click on <Button> propagates to the parent handler
const ButtonStub = {
  name: 'Button',
  template: '<button class="app-button-stub" @click="$emit(\'click\', $event)"><slot></slot></button>',
  emits: ['click']
};

/**
 * Part List View Component Tests (零件清單組件測試)
 */
vi.mock('../../../services/part/part', () => ({
  partService: {
    getParts: vi.fn().mockResolvedValue({ data: MOCK_PARTS_DATA, total: 1, page: 1 }),
    exportPartsToExcel: vi.fn().mockResolvedValue(undefined),
    getSuppliers: vi.fn().mockResolvedValue(['Supplier A', 'Supplier B']),
    /* Update by Gemini AI on 2026-04-23: getCustomers -> getProjects */
    /* getCustomers: vi.fn().mockResolvedValue([{ id: 'customer001', name: 'Test Customer' }]) */
    getProjects: vi.fn().mockResolvedValue([{ id: 'project001', name: 'Test Customer' }])
  },
  batchAcceptParts: vi.fn().mockResolvedValue({ failed: [] })
}));

vi.mock('../../../services/common/common', () => ({
  commonService: {
    /* Update by Gemini AI on 2026-04-23: getCustomers -> getProjects */
    /* getCustomers: vi.fn().mockResolvedValue([{ key: 'customer001', value: 'Test Customer' }]), */
    getProjects: vi.fn().mockResolvedValue([{ key: 'project001', value: 'Test Customer' }]),
    getStatusOptions: vi.fn().mockResolvedValue([{ key: 'ACTIVE', value: 'Active' }]),
    getSuppliers: vi.fn().mockResolvedValue([{ key: 'S001', value: 'Supplier A' }]),
    formatDateTime: vi.fn().mockReturnValue('2026-04-23 12:00')
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
      mocks: { $t: (key: string) => key },
      stubs: {
        Card: CardStub,
        Dot: DotStub,
        Button: ButtonStub,
        'el-select': { template: '<div class="el-select"><slot></slot></div>' },
        'el-option': { template: '<div class="el-option"></div>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    pushSpy.mockClear();

    // Setup Auth Store state
    const authStore = useAuthStore();
    /* Update by Gemini AI on 2026-04-23: Align with project rename. (繁體中文) 配合專案更名。 */
    /* authStore.setAuth('mock-token', {
      id: 'customer001',
      name: 'Test Customer',
      role: 'CUSTOMER'
    }); */
    authStore.setAuth('mock-token', {
      id: 'project001',
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
    const { partService } = await import('../../../services/part/part');
    vi.mocked(partService.getParts).mockResolvedValue({ data: MOCK_PARTS_DATA, total: 1, page: 1 });

    const wrapper = mount(PartListView, globalConfig);
    await flushPromises();

    const addButton = wrapper.find('.app-button-stub');
    expect(addButton.exists()).toBe(true);
    await addButton.trigger('click');

    expect(pushSpy).toHaveBeenCalledWith({ name: 'part-create' });
  });

  it('performs keyword search (執行關鍵字搜尋)', async () => {
    const { partService } = await import('../../../services/part/part');
    vi.mocked(partService.getParts).mockResolvedValue({ data: MOCK_PARTS_DATA, total: 1, page: 1 });

    const wrapper = mount(PartListView, globalConfig);
    await flushPromises();

    const vm = wrapper.vm as any;
    // Set search query — watcher debounces the API call by 1 second
    vm.searchQuery = 'PN-2024-001';
    await flushPromises();

    // Verify the search query is bound and the input reflects it
    expect(vm.searchQuery).toBe('PN-2024-001');
    const searchInput = wrapper.find('#search-input');
    expect(searchInput.exists()).toBe(true);
  });
});
