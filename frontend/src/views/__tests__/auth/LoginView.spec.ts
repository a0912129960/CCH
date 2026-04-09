import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import LoginView from '../../auth/LoginView.vue';
import { authService } from '../../../services/auth/auth';
import { useRouter } from 'vue-router';

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key
  })
}));

// Mock vue-router
vi.mock('vue-router', () => ({
  useRouter: vi.fn()
}));

// Mock authService
vi.mock('../../../services/auth/auth', () => ({
  authService: {
    login: vi.fn(),
    state: {
      role: 'GUEST'
    },
    isAuthenticated: vi.fn().mockReturnValue(false)
  },
  UserRole: {
    EMPLOYEE: 'EMPLOYEE',
    CUSTOMER: 'CUSTOMER',
    GUEST: 'GUEST'
  }
}));

describe('LoginView.vue', () => {
  let pushMock: any;

  beforeEach(() => {
    vi.clearAllMocks();
    pushMock = vi.fn();
    (useRouter as any).mockReturnValue({
      push: pushMock
    });
  });

  const globalConfig = {
    mocks: {
      $t: (key: string) => key
    }
  };

  it('renders login form correctly (正確渲染登入表單)', () => {
    const wrapper = mount(LoginView, {
      global: globalConfig
    });
    
    expect(wrapper.find('h2').text()).toBe('login.title');
    expect(wrapper.find('#username').exists()).toBe(true);
    expect(wrapper.find('#password').exists()).toBe(true);
  });

  it('shows error message on failed login (登入失敗顯示錯誤訊息)', async () => {
    (authService.login as any).mockReturnValue(false);
    
    const wrapper = mount(LoginView, {
      global: globalConfig
    });

    await wrapper.find('#username').setValue('wrong');
    await wrapper.find('#password').setValue('wrong');
    await wrapper.find('button').trigger('click');

    expect(authService.login).toHaveBeenCalled();
    expect(wrapper.find('.error-msg').exists()).toBe(true);
    expect(wrapper.find('.error-msg').text()).toBe('login.error_invalid');
  });
});
