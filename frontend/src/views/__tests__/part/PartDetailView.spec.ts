import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount, flushPromises } from '@vue/test-utils';
import PartDetailView from '../../part/PartDetailView.vue';

// Mock element-plus
vi.mock('element-plus', () => ({
  ElMessage: { success: vi.fn(), warning: vi.fn(), error: vi.fn() },
  ElMessageBox: { 
    confirm: vi.fn().mockResolvedValue(true),
    prompt: vi.fn().mockResolvedValue({ value: 'Test Reason' })
  }
}));

// Mock vue-i18n handled by setup.ts, but local override if needed
/* 
vi.mock('vue-i18n', () => ({
  useI18n: () => ({ t: (key: string) => key })
}));
*/

// Mock authService
vi.mock('../../../services/auth/auth', () => ({
  UserRole: { DIMERCO: 'DIMERCO', CUSTOMER: 'CUSTOMER', DCB: 'DCB' },
  authService: {
    state: { role: 'CUSTOMER', customerId: 'customer001' }
  }
}));

// Mock vue-router
const mockBack = vi.fn();
const mockPush = vi.fn();
vi.mock('vue-router', () => ({
  useRoute: () => ({ params: { id: '7' }, path: '/parts/7' }),
  useRouter: () => ({
    back: mockBack,
    push: mockPush
  })
}));

// Mock partService functions directly (matching PartDetailView.vue)
vi.mock('../../../services/part/part', () => ({
  getPartDetail: vi.fn().mockResolvedValue({
    status: 'S02', // RETURNED
    modified: { 
      partNo: 'PN-2024-007', 
      partDesc: 'Test Desc', 
      htsCode: '8517.12.00', 
      rate: 5,
      country: 'Taiwan',
      division: 'Div A',
      supplier: 'Supplier Alpha'
    },
    before: { division: 'Old Div', supplier: 'Old Supp', partDesc: 'Old Desc', htsCode: '8517.12.00', rate: 5 }
  }),
  getMilestones: vi.fn().mockResolvedValue([
    { action: 'Created', updatedDate: '2026-04-01', updatedBy: 'User A', remark: 'Initial' }
  ]),
  statusToI18nKey: (s: string) => s,
  updatePart: vi.fn().mockResolvedValue({ success: true }),
  submitPart: vi.fn().mockResolvedValue({ success: true }),
  acceptPart: vi.fn().mockResolvedValue({ success: true }),
  returnPart: vi.fn().mockResolvedValue({ success: true }),
  PartStatus: { ACTIVE: 'S04', PENDING_CUSTOMER: 'S02' }
}));

describe('PartDetailView.vue', () => {
  const globalConfig = {
    global: {
      mocks: { $t: (key: string) => key },
      stubs: {
        // Card is not a registered component in this test, but handled by unplugin-vue-components?
        // Actually we should stub it if it's not imported.
        Card: { template: '<div class="card"><slot></slot></div>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders part details correctly (正確渲染零件詳情)', async () => {
    const wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();

    expect(wrapper.find('h1').text()).toContain('PN-2024-007');
    // Using a more flexible selector for the value
    expect(wrapper.text()).toContain('8517.12.00');
  });

  it('shows dynamic breadcrumb based on history (根據歷史記錄顯示動態麵包屑)', async () => {
    // 1. From Parts List
    vi.stubGlobal('window', { history: { state: { back: '/parts' } } });
    let wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();
    expect(wrapper.find('.breadcrumb a').text()).toBe('common.menu.parts');

    // 2. From Dashboard
    vi.stubGlobal('window', { history: { state: { back: '/customer' } } });
    wrapper = mount(PartDetailView, globalConfig);
    await flushPromises();
    expect(wrapper.find('.breadcrumb a').text()).toBe('common.menu.dashboard');
  });
});
