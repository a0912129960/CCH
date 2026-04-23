import { mount, flushPromises } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import EmployeeView from '../../employee/EmployeeView.vue';
import { UserRole } from '../../../services/auth/auth';
import { dashboardService } from '../../../services/dashboard/dashboard';
import { useAuthStore } from '../../../stores/auth';

// Mock Vue Router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn()
  })
}));

// Mock services
vi.mock('../../../services/dashboard/dashboard', () => ({
  dashboardService: {
    getStatusSummary: vi.fn().mockResolvedValue([]),
    getPendingReviewParts: vi.fn().mockResolvedValue([])
  }
}));

vi.mock('../../../services/common/common', () => ({
  commonService: {
    getCustomers: vi.fn().mockResolvedValue([{ key: 'customer001', value: 'Test Customer' }])
  }
}));

// Mock I18n handled by setup.ts
const $t = (key: string) => key;

describe('EmployeeView.vue', () => {
  const globalConfig = {
    global: {
      mocks: { $t },
      stubs: ['Card', 'Dot']
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    
    // Setup Employee Auth state via Store
    const authStore = useAuthStore();
    authStore.setAuth('mock-token', {
      id: 'E001',
      name: 'Y9999',
      role: UserRole.DIMERCO
    });
  });

  it('renders employee dashboard title and welcome message', async () => {
    const wrapper = mount(EmployeeView, globalConfig);
    await flushPromises();
    expect(wrapper.find('h1').text()).toContain('employee.title');
    expect(wrapper.find('.username').text()).toBe('Y9999');
  });

  it('fetches and displays status summary', async () => {
    const summarySpy = vi.spyOn(dashboardService, 'getStatusSummary');
    mount(EmployeeView, globalConfig);
    await flushPromises();
    expect(summarySpy).toHaveBeenCalledWith('all');
  });

  it('filters by customer when selection changes', async () => {
    const summarySpy = vi.spyOn(dashboardService, 'getStatusSummary');
    const wrapper = mount(EmployeeView, globalConfig);
    await flushPromises();
    
    // Simulate customer selection change
    const select = wrapper.find('select');
    await select.setValue('customer001');
    await flushPromises();
    
    expect(summarySpy).toHaveBeenCalledWith('customer001');
  });

  it('displays pending review parts in a table', async () => {
    vi.spyOn(dashboardService, 'getPendingReviewParts').mockResolvedValue([
      { id: 1, customer: 'Test Corp', partNo: 'PN-TEST-001', partDesc: 'Desc', htsCode: '1234.56.78', status: 'S02', updatedBy: 'user', updatedDate: '2026-04-10T10:00:00', slaStatus: 'green' } as any
    ]);

    const wrapper = mount(EmployeeView, globalConfig);
    await flushPromises();

    expect(wrapper.find('tbody tr td:nth-child(2)').text()).toBe('PN-TEST-001');
    expect(wrapper.find('tbody tr td:first-child').text()).toBe('Test Corp');
  });
});
