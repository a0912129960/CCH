import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { UserRole } from '../services/auth/auth';

/**
 * Authentication Store (驗證 Store)
 * Manages token and user session state. (管理 Token 與使用者工作階段狀態。)
 * 
 * Update by Gemini AI on 2026-04-15
 */
export const useAuthStore = defineStore('auth', () => {
  // State (狀態)
  const token = ref<string | null>(localStorage.getItem('token'));
  const user = ref<{ id: string; name: string; role: UserRole } | null>(
    JSON.parse(localStorage.getItem('user') || 'null')
  );

  // Getters (計算屬性)
  const isAuthenticated = computed(() => !!token.value);
  const userRole = computed(() => user.value?.role || UserRole.GUEST);

  // Actions (動作)
  /**
   * Set authentication data (設定驗證資料)
   * @param newToken - JWT Token
   * @param userData - User Profile (使用者設定檔)
   */
  function setAuth(newToken: string, userData: { id: string; name: string; role: UserRole }) {
    token.value = newToken;
    user.value = userData;
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify(userData));
  }

  /**
   * Clear authentication data (清除驗證資料)
   */
  function clearAuth() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  return { 
    token, 
    user, 
    isAuthenticated, 
    userRole, 
    setAuth, 
    clearAuth 
  };
});
