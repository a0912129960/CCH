import { describe, it, expect, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import { createTestingPinia } from '@pinia/testing';
import AppHeader from '../AppHeader.vue';
import { createRouter, createWebHistory } from 'vue-router';

// Simplest possible mock
vi.mock('vue-i18n', async () => {
  const actual = await vi.importActual('vue-i18n') as any;
  return {
    ...actual,
    useI18n: () => ({
      t: (k: string) => k,
      locale: { value: 'en' }
    })
  };
});

vi.mock('../../../services/auth/auth', async () => {
  const actual = await vi.importActual('../../../services/auth/auth') as any;
  return {
    ...actual,
    authService: {
      logout: vi.fn(),
      state: { username: 'Test User', role: 'DIMERCO' }
    }
  };
});

describe('AppHeader.vue', () => {
  it('mounts without crashing (應能正常掛載而不崩潰)', () => {
    const router = createRouter({
      history: createWebHistory(),
      routes: [{ path: '/', component: { template: 'div' } }]
    });
    
    const wrapper = mount(AppHeader, {
      global: {
        plugins: [createTestingPinia(), router],
        stubs: ['el-dropdown', 'el-dropdown-menu', 'el-dropdown-item'],
        mocks: { $t: (k: string) => k }
      }
    });
    expect(wrapper.exists()).toBe(true);
  });
});
