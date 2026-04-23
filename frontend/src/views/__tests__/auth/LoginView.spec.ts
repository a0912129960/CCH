import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import LoginView from '../../auth/LoginView.vue';
import { authService } from '../../../services/auth/auth';
import { createRouter, createWebHistory } from 'vue-router';

// Global i18n mock
const globalMocks = {
  $t: (key: string) => key
};

vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key
  })
}));

/**
 * Login View Tests (登入頁面測試)
 * Update on 2026-04-23: Refactored from Customer to Project focus.
 */
vi.mock('../../../services/auth/auth', () => ({
  authService: {
    login: vi.fn(),
    state: { role: 'CUSTOMER', projectId: 'project001' },
    isAuthenticated: vi.fn()
  },
  UserRole: { CUSTOMER: 'CUSTOMER', DCB: 'DCB', DIMERCO: 'DIMERCO' }
}));

describe('LoginView.vue', () => {
  let router: any;

  beforeEach(() => {
    router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/customer', name: 'customer', component: { template: 'div' } }]
    });
    vi.clearAllMocks();
  });

  it('renders login form correctly', () => {
    const wrapper = mount(LoginView, {
      global: { plugins: [router], mocks: globalMocks }
    });
    expect(wrapper.find('h2').exists()).toBe(true);
  });

  it('calls authService.login on click', async () => {
    const wrapper = mount(LoginView, {
      global: { plugins: [router], mocks: globalMocks }
    });
    vi.mocked(authService.login).mockResolvedValue(true);
    await wrapper.find('input[type="text"]').setValue('user');
    await wrapper.find('input[type="password"]').setValue('pass');
    await wrapper.find('button').trigger('click');
    expect(authService.login).toHaveBeenCalled();
  });
});
