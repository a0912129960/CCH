import { setActivePinia, createPinia } from 'pinia';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { useAuthStore } from '../auth';
import { UserRole } from '../../services/auth/auth';

describe('Auth Store (驗證 Store 測試)', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    localStorage.clear();
  });

  it('should set authentication state correctly (應能正確設定驗證狀態)', () => {
    const authStore = useAuthStore();
    const mockToken = 'test-token-123';
    const mockUser = { id: 'U001', name: 'Test User', role: UserRole.EMPLOYEE };

    authStore.setAuth(mockToken, mockUser);

    expect(authStore.token).toBe(mockToken);
    expect(authStore.user).toEqual(mockUser);
    expect(authStore.isAuthenticated).toBe(true);
    expect(authStore.userRole).toBe(UserRole.EMPLOYEE);
    expect(localStorage.getItem('token')).toBe(mockToken);
  });

  it('should clear authentication state correctly (應能正確清除驗證狀態)', () => {
    const authStore = useAuthStore();
    authStore.setAuth('token', { id: '1', name: 'user', role: UserRole.CUSTOMER });
    
    authStore.clearAuth();

    expect(authStore.token).toBeNull();
    expect(authStore.user).toBeNull();
    expect(authStore.isAuthenticated).toBe(false);
    expect(localStorage.getItem('token')).toBeNull();
  });
});
