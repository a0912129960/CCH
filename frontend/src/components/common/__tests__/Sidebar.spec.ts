import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import Sidebar from '../Sidebar.vue';

// 1. Partial mock vue-i18n to keep createI18n
vi.mock('vue-i18n', async (importOriginal) => {
  const actual = await importOriginal() as any;
  return {
    ...actual,
    useI18n: () => ({
      t: (key: string) => key,
      locale: { value: 'en' }
    })
  };
});

// 2. Mock vue-router
vi.mock('vue-router', () => ({
  RouterLink: { template: '<a><slot></slot></a>' },
  useRouter: () => ({
    push: vi.fn()
  })
}));

// 3. Mock internal locales module specifically
vi.mock('../../locales', () => ({
  switchLanguage: vi.fn(),
  default: {
    global: { locale: { value: 'en' } }
  }
}));

// 4. Mock auth service
vi.mock('../../../services/auth/auth', () => ({
  UserRole: {
    GUEST: 'GUEST',
    EMPLOYEE: 'EMPLOYEE',
    CUSTOMER: 'CUSTOMER'
  },
  authService: {
    logout: vi.fn(),
    isAuthenticated: vi.fn().mockReturnValue(true),
    state: {
      role: 'CUSTOMER'
    }
  }
}));

describe('Sidebar.vue', () => {
  const globalConfig = {
    global: {
      plugins: [createTestingPinia({ createSpy: vi.fn })],
      mocks: {
        $t: (key: string) => key
      },
      stubs: {
        RouterLink: { 
          template: '<a :to="to"><slot></slot></a>',
          props: ['to']
        },
        'el-select': { template: '<div class="el-select"><slot></slot></div>' },
        'el-option': { template: '<div class="el-option"></div>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    // Default role
    import('../../../services/auth/auth').then(({ authService }) => {
      authService.state.role = 'CUSTOMER';
    });
  });

  it('renders navigation links (正確渲染導覽連結)', () => {
    const wrapper = mount(Sidebar, globalConfig);
    expect(wrapper.text()).toContain('common.menu.dashboard');
    expect(wrapper.text()).toContain('common.menu.parts');
  });

  it('contains the application brand name (包含應用程式品牌名稱)', () => {
    const wrapper = mount(Sidebar, globalConfig);
    expect(wrapper.find('.brand-name').text()).toBe('CCH');
  });

  it('links to customer dashboard for customers (客戶角色連結至客戶儀表板)', async () => {
    const { authService } = await import('../../../services/auth/auth');
    authService.state.role = 'CUSTOMER';
    const wrapper = mount(Sidebar, globalConfig);
    const dashboardLink = wrapper.find('a'); // The first link is dashboard
    expect(dashboardLink.attributes('to')).toBe('/customer');
  });

  it('links to employee dashboard for employees (員工角色連結至員工儀表板)', async () => {
    const { authService } = await import('../../../services/auth/auth');
    authService.state.role = 'EMPLOYEE';
    const wrapper = mount(Sidebar, globalConfig);
    const dashboardLink = wrapper.find('a');
    expect(dashboardLink.attributes('to')).toBe('/employee');
  });
});
