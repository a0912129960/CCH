import { mount, flushPromises } from '@vue/test-utils';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import EmployeeView from '../../employee/EmployeeView.vue';
import { authService, UserRole } from '../../../services/auth/auth';
import { partService } from '../../../services/part/part';
import { dashboardService } from '../../../services/dashboard/dashboard';

// Mock Vue Router
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: vi.fn()
  })
}));

// Mock I18n
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
    
    // Setup Employee Auth state
    authService.state.isLoggedIn = true;
    authService.state.role = UserRole.DIMERCO;
    authService.state.username = 'Y9999';
    authService.state.customerId = undefined;
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
      { id: '1', partNo: 'PN-TEST-001', customerName: 'Test Corp', htsCode: '1234.56.78', supplier: 'Supp A', lastUpdated: '2026-04-10 10:00' } as any
    ]);
    
    const wrapper = mount(EmployeeView, globalConfig);
    await flushPromises();
    
    expect(wrapper.find('tbody tr td').text()).toBe('PN-TEST-001');
    expect(wrapper.find('tbody tr td:nth-child(2)').text()).toBe('Test Corp');
  });
});
