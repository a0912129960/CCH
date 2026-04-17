import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import PartDetailView from '../../part/PartDetailView.vue';
import { partService } from '../../../services/part/part';

// Mock element-plus
vi.mock('element-plus', () => ({
  ElMessage: { success: vi.fn(), warning: vi.fn() },
  ElMessageBox: { confirm: vi.fn().mockResolvedValue(true) }
}));

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key
  })
}));

// Mock authService
vi.mock('../../../services/auth/auth', () => ({
  UserRole: { DIMERCO: 'DIMERCO', CUSTOMER: 'CUSTOMER' },
  authService: {
    state: { role: 'CUSTOMER' }
  }
}));

// Mock vue-router
const mockBack = vi.fn();
const mockPush = vi.fn();
vi.mock('vue-router', () => ({
  useRoute: () => ({ params: { id: '7' } }),
  useRouter: () => ({
    back: mockBack,
    push: mockPush,
    app: { config: { globalProperties: { $t: (k: string) => k } } }
  })
}));

// Mock partService
vi.mock('../../../services/part/part', async () => {
  const actual = await vi.importActual('../../../services/part/part') as any;
  return {
    ...actual,
    partService: {
      getPartById: vi.fn().mockResolvedValue({
        id: '7',
        partNo: 'PN-2024-007',
        htsCode: '8517.12.00',
        status: 'RETURNED',
        supplier: 'TechCorp',
        lastUpdated: '2026-04-09',
        dimercoRemark: 'Reason text',
        history: [{ id: 'h1', status: 'RETURNED', updatedBy: 'Admin', updatedAt: '2026-04-09' }]
      }),
      updatePartStatus: vi.fn().mockResolvedValue(true)
    }
  };
});

describe('PartDetailView.vue', () => {
  const globalConfig = {
    global: {
      mocks: { $t: (key: string) => key },
      stubs: {
        Card: { template: '<div class="card"><slot name="header"></slot><slot></slot></div>' },
        Button: { template: '<button><slot></slot></button>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    // Default to customer
    import('../../../services/auth/auth').then(({ authService }) => {
      authService.state.role = 'CUSTOMER';
    });
    // Default history state
    vi.stubGlobal('window', {
      history: { state: { back: '/parts' } }
    });
  });

  it('renders part details correctly (正確渲染零件詳情)', async () => {
    const wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();

    expect(wrapper.find('h1').text()).toBe('PN-2024-007');
    expect(wrapper.find('.code-box').text()).toBe('8517.12.00');
  });

  it('shows dynamic breadcrumb based on history (根據歷史記錄顯示動態麵包屑)', async () => {
    // 1. From Parts List
    window.history.state.back = '/parts';
    let wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();
    expect(wrapper.find('.breadcrumb a').text()).toBe('common.menu.parts');

    // 2. From Dashboard
    window.history.state.back = '/customer';
    wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();
    expect(wrapper.find('.breadcrumb a').text()).toBe('common.menu.dashboard');
  });

  it('restricts actions for customers: only Accept for RETURNED parts (限制客戶動作：針對退回零件僅顯示接受)', async () => {
    const wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();

    // Should see Accept button
    expect(wrapper.find('.btn-accept').exists()).toBe(true);
    // Should NOT see Resubmit button (action-card is hidden or only has Accept)
    // In our implementation, customers only see Accept button, and no textarea for RETURNED status
    expect(wrapper.find('textarea').exists()).toBe(false);
  });

  it('enables employee review actions for PENDING_REVIEW parts (員工可核准或退回審核中的零件)', async () => {
    const { authService, UserRole } = await import('../../../services/auth/auth');
    authService.state.role = UserRole.DIMERCO;
    
    // Mock a pending review part
    partService.getPartById = vi.fn().mockResolvedValue({
      id: '4', partNo: 'PN-004', status: 'PENDING_REVIEW', history: []
    });

    const wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();

    // Should see both Accept and Return buttons
    expect(wrapper.find('.btn-accept').text()).toBe('common.accept');
    expect(wrapper.find('.btn-return').text()).toBe('part_detail.btn_return_customer');
    expect(wrapper.find('textarea').exists()).toBe(true);
  });
});
