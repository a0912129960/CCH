import axios from 'axios';
import { useAuthStore } from '../stores/auth';

/**
 * Base API instance with Interceptors (基礎 API 實例與攔截器)
 * Handles token attachment and global error handling. (處理 Token 附加與全域錯誤處理。)
 * 
 * Update by Gemini AI on 2026-04-15
 */
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: {
    'Content-Type': 'application/json'
  }
});

// Request Interceptor (請求攔截器)
api.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore();
    if (authStore.token) {
      config.headers.Authorization = `Bearer ${authStore.token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor (回應攔截器)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    const authStore = useAuthStore();
    
    // Handle 401 Unauthorized (處理 401 未授權)
    if (error.response?.status === 401) {
      authStore.clearAuth();
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }
    
    return Promise.reject(error);
  }
);

export default api;
