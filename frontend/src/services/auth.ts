import { reactive } from 'vue';

/**
 * User Role Types (使用者角色類型)
 * @enum {string}
 */
export enum UserRole {
  GUEST = 'GUEST',
  EMPLOYEE = 'EMPLOYEE',
  CUSTOMER = 'CUSTOMER'
}

/**
 * Authentication State Interface (驗證狀態介面)
 */
interface AuthState {
  isLoggedIn: boolean;
  role: UserRole;
  username: string;
}

/**
 * Global reactive authentication state (全域響應式驗證狀態)
 */
const authState = reactive<AuthState>({
  isLoggedIn: false,
  role: UserRole.GUEST,
  username: ''
});

/**
 * Auth Service (驗證服務)
 */
export const authService = {
  /**
   * Get current auth state (獲取當前驗證狀態)
   */
  get state() {
    return authState;
  },

  /**
   * Mock Login function (模擬登入功能)
   * @param {string} username - Account (帳號)
   * @param {string} password - Password (密碼)
   * @returns {boolean} - Success status (成功狀態)
   */
  login(username: string, password: string): boolean {
    // Standard password for mock data (假資料統一密碼)
    const MOCK_PASSWORD = '888888';

    if (password !== MOCK_PASSWORD) return false;

    // Check Employee: Y9999 (檢查員工工號)
    if (username === 'Y9999') {
      authState.isLoggedIn = true;
      authState.role = UserRole.EMPLOYEE;
      authState.username = username;
      return true;
    }

    // Check Customer: customer001 (檢查客戶帳號)
    if (username === 'customer001') {
      authState.isLoggedIn = true;
      authState.role = UserRole.CUSTOMER;
      authState.username = username;
      return true;
    }

    return false;
  },

  /**
   * Logout function (登出功能)
   */
  logout() {
    authState.isLoggedIn = false;
    authState.role = UserRole.GUEST;
    authState.username = '';
  },

  /**
   * Check if user is authenticated (檢查是否已驗證)
   */
  isAuthenticated() {
    return authState.isLoggedIn;
  }
};
