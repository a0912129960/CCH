import { vi, beforeEach } from 'vitest';
import { createPinia, setActivePinia } from 'pinia';

/**
 * Global Vitest Setup (全域 Vitest 設定)
 * Initialize Pinia for all tests to prevent "getActivePinia()" errors.
 */
beforeEach(() => {
  setActivePinia(createPinia());
});

// Global Mocks
vi.mock('axios', () => ({
  default: {
    create: vi.fn().mockReturnThis(),
    interceptors: {
      request: { use: vi.fn(), eject: vi.fn() },
      response: { use: vi.fn(), eject: vi.fn() }
    },
    get: vi.fn().mockResolvedValue({ data: {} }),
    post: vi.fn().mockResolvedValue({ data: { success: true, data: { token: 'mock', user: { role: 'admin' } } } }),
    put: vi.fn().mockResolvedValue({ data: {} }),
    delete: vi.fn().mockResolvedValue({ data: {} })
  }
}));

vi.mock('vue-i18n', async () => {
  const actual = await vi.importActual('vue-i18n') as any;
  return {
    ...actual,
    useI18n: () => ({
      t: (key: string) => key,
      locale: { value: 'en' }
    })
  };
});
