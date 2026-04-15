import api from '../api';
import { useAuthStore } from '../../stores/auth';

/**
 * User Role Types (使用者角色類型)
 */
export const UserRole = {
  CUSTOMER: 'CUSTOMER',
  DCB: 'DCB',
  DIMERCO: 'DIMERCO'
} as const;
export type UserRole = typeof UserRole[keyof typeof UserRole];

/**
 * Auth Service (驗證服務)
 */
export const authService = {
  /**
   * Get current auth state (獲取當前驗證狀態)
   * Linked to Pinia store (與 Pinia Store 連結)
   */
  get state() {
    const authStore = useAuthStore();
    return {
      isLoggedIn: authStore.isAuthenticated,
      role: authStore.userRole,
      username: authStore.user?.name || '',
      customerId: authStore.user?.role === UserRole.CUSTOMER ? authStore.user.id : undefined
    };
  },

  /**
   * Login function (登入功能)
   * Connects to backend API (連接至後端 API)
   * 
   * @param {string} username - Account (帳號)
   * @param {string} password - Password (密碼)
   * @returns {Promise<boolean>} - Success status (成功狀態)
   */
  async login(username: string, password: string): Promise<boolean> {
    try {
      // 1. 发出实体 API 呼叫 (Make actual API call)
      const response = await api.post('/auth/login', { username, password });
      
      // 2. 处理回应 - 修正为读取 response.data.data (Fix to read response.data.data)
      const apiResult = response.data;
      if (apiResult.success && apiResult.data && apiResult.data.token) {
        const authStore = useAuthStore();
        
        // 3. 角色转换：将小写转换为大写以符合前端定义 (Convert lowercase role to uppercase)
        const userData = {
          ...apiResult.data.user,
          role: apiResult.data.user.role.toUpperCase() as UserRole
        };

        authStore.setAuth(apiResult.data.token, userData);
        return true;
      }
      return false;
    } catch (error) {
      console.error('Login failed (登入失敗):', error);
      return false;
    }
  },

  /**
   * Logout function (登出功能)
   */
  logout() {
    const authStore = useAuthStore();
    authStore.clearAuth();
  },

  /**
   * Check if user is authenticated (檢查是否已驗證)
   */
  isAuthenticated() {
    const authStore = useAuthStore();
    return authStore.isAuthenticated;
  }
};
