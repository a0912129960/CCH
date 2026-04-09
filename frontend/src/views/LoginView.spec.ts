import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import LoginView from './LoginView.vue';
import { authService, UserRole } from '../services/auth';
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
vi.mock('../services/auth', () => ({
  authService: {
    login: vi.fn(),
    state: {
      role: 'GUEST'
    }
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
      $t: (key: string) => key // Mock global $t for templates
    },
    stubs: {
      AppButton: {
        template: '<button @click="$emit(\'click\')"><slot></slot></button>'
      }
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

  it('redirects to employee page on success employee login (員工登入成功應導向儀表板)', async () => {
    (authService.login as any).mockReturnValue(true);
    (authService as any).state.role = UserRole.EMPLOYEE;
    
    const wrapper = mount(LoginView, {
      global: globalConfig
    });

    await wrapper.find('#username').setValue('Y9999');
    await wrapper.find('#password').setValue('888888');
    await wrapper.find('button').trigger('click');

    expect(pushMock).toHaveBeenCalledWith('/employee');
  });

  it('redirects to customer page on success customer login (客戶登入成功應導向入口)', async () => {
    (authService.login as any).mockReturnValue(true);
    (authService as any).state.role = UserRole.CUSTOMER;
    
    const wrapper = mount(LoginView, {
      global: globalConfig
    });

    await wrapper.find('#username').setValue('customer001');
    await wrapper.find('#password').setValue('888888');
    await wrapper.find('button').trigger('click');

    expect(pushMock).toHaveBeenCalledWith('/customer');
  });
});
