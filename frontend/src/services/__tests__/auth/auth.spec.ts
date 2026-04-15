import { describe, it, expect, vi, beforeEach } from 'vitest';
import { setActivePinia, createPinia } from 'pinia';
import { authService, UserRole } from '../../auth/auth';
import api from '../../api';

// Mock the API and Pinia store (模擬 API 與 Pinia Store)
vi.mock('../../api');

describe('Auth Service (驗證服務測試)', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    vi.clearAllMocks();
    localStorage.clear();
    // Ensure store is reset (確保 Store 已重設)
    authService.logout();
  });

  it('login should succeed and store token/user (登入應成功並儲存 Token 與 User)', async () => {
    const mockResponse = {
      data: {
        token: 'fake-jwt-token',
        user: { id: 'U123', name: 'Admin', role: UserRole.EMPLOYEE }
      }
    };
    
    // Mock successful post (模擬成功的 POST)
    vi.mocked(api.post).mockResolvedValue(mockResponse);

    const result = await authService.login('Y9999', '888888');

    expect(result).toBe(true);
    expect(authService.isAuthenticated()).toBe(true);
    expect(authService.state.username).toBe('Admin');
    expect(authService.state.role).toBe(UserRole.EMPLOYEE);
  });

  it('login should fail on error (登入在發生錯誤時應失敗)', async () => {
    // Mock rejected post (模擬被拒絕的 POST)
    vi.mocked(api.post).mockRejectedValue(new Error('Network Error'));

    const result = await authService.login('Y9999', 'wrong');

    expect(result).toBe(false);
    expect(authService.isAuthenticated()).toBe(false);
  });
});
