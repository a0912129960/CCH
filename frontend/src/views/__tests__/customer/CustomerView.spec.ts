import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia } from 'pinia';
import CustomerView from '../../customer/CustomerView.vue';

/**
 * Customer Dashboard View Tests (客戶儀表板測試)
 * Update on 2026-04-23: Refactored from Customer to Project focus.
 */

vi.mock('../../../services/dashboard/dashboard', () => ({
  dashboardService: {
    getStatusSummary: vi.fn().mockResolvedValue([
      { status: 'S04', count: 10, labelKey: 'status.active', color: '#67C23A' }
    ]),
    getPendingReviewParts: vi.fn().mockResolvedValue([
      { id: 1, project: 'Test Project Corp', partNo: 'PN-001', partDesc: 'Desc', htsCode: '1234.56.7890', status: 'S03', updatedBy: 'user', updatedDate: '2026-04-23T10:00:00', slaStatus: 'red' }
    ]),
    getSLAItems: vi.fn().mockResolvedValue([])
  }
}));

vi.mock('../../../services/part/part', () => ({
  partService: {
    getProjects: vi.fn().mockResolvedValue([])
  },
  statusToI18nKey: vi.fn((s: string) => s.toLowerCase()),
  statusToColor: vi.fn(() => '#67C23A'),
  PartStatus: {
    PENDING_CUSTOMER: 'S03',
    RETURNED: 'S03'
  }
}));

vi.mock('vue-router', () => ({
  useRouter: () => ({ push: vi.fn() })
}));

describe('CustomerView.vue', () => {
  const globalConfig = {
    global: {
      plugins: [createPinia()],
      mocks: {
        $t: (key: string, params?: any) => {
          if (params && params.min) return `${key}:${params.min}`;
          return key;
        }
      },
      stubs: {
        Card: { template: '<div class="card"><slot></slot></div>' },
        Dot: { template: '<div class="dot"></div>' }
      }
    }
  };

  beforeEach(() => {
    setActivePinia(createPinia());
    vi.clearAllMocks();
  });

  it('renders status summary correctly (正確渲染狀態摘要)', async () => {
    const wrapper = mount(CustomerView, globalConfig);
    await flushPromises();

    expect(wrapper.find('.summary-section').exists()).toBe(true);
    expect(wrapper.find('.status-value').text()).toBe('10');
    expect(wrapper.find('.status-label').text()).toBe('status.active');
  });

  it('renders pending review table correctly (正確渲染待審核零件列表)', async () => {
    const wrapper = mount(CustomerView, globalConfig);
    await flushPromises();

    expect(wrapper.find('.pending-section').exists()).toBe(true);
    expect(wrapper.find('tbody tr td.bold').text()).toBe('PN-001');
  });

  it('applies SLA dot for pending parts (待審核零件顯示 SLA 燈號)', async () => {
    const wrapper = mount(CustomerView, globalConfig);
    await flushPromises();

    expect(wrapper.find('tbody tr').exists()).toBe(true);
  });
});
