import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import CustomerView from '../../customer/CustomerView.vue';

/**
 * Customer Dashboard View Tests (客戶儀表板測試)
 */

// Mock services
vi.mock('../../../services/dashboard/dashboard', () => ({
  dashboardService: {
    getStatusSummary: vi.fn().mockResolvedValue([
      { status: 'ACTIVE', count: 10, labelKey: 'status.active', color: 'green' }
    ]),
    getSLAItems: vi.fn().mockResolvedValue([
      { id: '1', partNo: 'PN-001', status: 'PENDING_CUSTOMER', remainingMinutes: 30 }
    ])
  }
}));

vi.mock('../../../services/part/part', () => ({
  PartStatus: {
    PENDING_CUSTOMER: 'PENDING_CUSTOMER',
    RETURNED: 'RETURNED'
  }
}));

vi.mock('vue-router', () => ({
  useRouter: () => ({ push: vi.fn() })
}));

describe('CustomerView.vue', () => {
  const globalConfig = {
    global: {
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
    vi.clearAllMocks();
  });

  it('renders status summary correctly (正確渲染狀態摘要)', async () => {
    const wrapper = mount(CustomerView, globalConfig);
    await flushPromises();

    expect(wrapper.find('.summary-section').exists()).toBe(true);
    expect(wrapper.find('.status-value').text()).toBe('10');
    expect(wrapper.find('.status-label').text()).toBe('status.active');
  });

  it('renders SLA countdown list correctly (正確渲染 SLA 倒數清單)', async () => {
    const wrapper = mount(CustomerView, globalConfig);
    await flushPromises();

    expect(wrapper.find('.sla-section').exists()).toBe(true);
    expect(wrapper.find('.part-no').text()).toBe('PN-001');
    expect(wrapper.find('.timer-text').text()).toContain('30');
  });

  it('applies urgent class for low SLA (當 SLA 時間較短時套用緊急樣式)', async () => {
    const wrapper = mount(CustomerView, globalConfig);
    await flushPromises();

    expect(wrapper.find('.sla-countdown').classes()).toContain('urgent');
  });
});
