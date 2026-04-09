import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import CustomerView from '../CustomerView.vue';
import { dashboardService } from '../../services/dashboard';

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => {
      if (params && params.min) return `${key}:${params.min}`;
      return key;
    }
  })
}));

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn()
  })
}));

// Mock dashboardService
vi.mock('../../services/dashboard', () => ({
  dashboardService: {
    getStatusSummary: vi.fn().mockResolvedValue([
      { status: 'ACTIVE', count: 10, labelKey: 'status.active', color: 'green' }
    ]),
    getSLAItems: vi.fn().mockResolvedValue([
      { id: '1', partNo: 'PN-001', status: 'PENDING_CUSTOMER', remainingMinutes: 30 }
    ])
  },
  PartStatus: {
    ACTIVE: 'ACTIVE',
    PENDING_CUSTOMER: 'PENDING_CUSTOMER'
  }
}));

describe('CustomerView.vue', () => {
  const globalConfig = {
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
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders status summary correctly (正確渲染狀態摘要)', async () => {
    const wrapper = mount(CustomerView, {
      global: globalConfig
    });

    // Wait for async onMounted (等待非同步掛載)
    await new Promise(resolve => setTimeout(resolve, 0));

    expect(wrapper.find('.summary-section').exists()).toBe(true);
    expect(wrapper.find('.status-count').text()).toBe('10');
    expect(wrapper.find('.status-label').text()).toBe('status.active');
  });

  it('renders SLA countdown list correctly (正確渲染 SLA 倒數清單)', async () => {
    const wrapper = mount(CustomerView, {
      global: globalConfig
    });

    await new Promise(resolve => setTimeout(resolve, 0));

    expect(wrapper.find('.sla-section').exists()).toBe(true);
    expect(wrapper.find('.part-no').text()).toBe('#PN-001');
    expect(wrapper.find('.sla-timer').text()).toContain('30');
  });

  it('applies warning class for low SLA (當 SLA 時間較短時套用警告樣式)', async () => {
    const wrapper = mount(CustomerView, {
      global: globalConfig
    });

    await new Promise(resolve => setTimeout(resolve, 0));

    expect(wrapper.find('.sla-timer').classes()).toContain('warning');
  });
});
