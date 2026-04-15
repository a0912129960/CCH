import { describe, it, expect, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import Sidebar from '../Sidebar.vue';

const globalMocks = {
  $t: (key: string) => key
};

vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key
  })
}));

vi.mock('../../../services/auth/auth', () => ({
  authService: {
    state: { role: 'DIMERCO' },
    isAuthenticated: () => true
  },
  UserRole: { CUSTOMER: 'CUSTOMER', DCB: 'DCB', DIMERCO: 'DIMERCO' }
}));

describe('Sidebar.vue', () => {
  it('renders brand name correctly', () => {
    const wrapper = mount(Sidebar, {
      global: {
        plugins: [createTestingPinia()],
        stubs: ['RouterLink'],
        mocks: globalMocks
      }
    });
    expect(wrapper.find('.brand-name').text()).toBe('Dimerco CCH');
  });
});
