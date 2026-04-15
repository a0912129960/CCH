import axios from 'axios';
import { useAuthStore } from '../stores/auth';
import { useUIStore } from '../stores/ui';

/**
 * Base API instance with Interceptors (基礎 API 實例與攔截器)
 * Handles token attachment, global error handling, and loading state. (處理 Token 附加、全域錯誤處理與 Loading 狀態。)
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
    const uiStore = useUIStore();
    
    // Show Loading (顯示 Loading)
    uiStore.setLoading(true);

    if (authStore.token) {
      config.headers.Authorization = `Bearer ${authStore.token}`;
    }
    return config;
  },
  (error) => {
    const uiStore = useUIStore();
    uiStore.setLoading(false);
    return Promise.reject(error);
  }
);

// Response Interceptor (回應攔截器)
api.interceptors.response.use(
  (response) => {
    const uiStore = useUIStore();
    // Hide Loading (隱藏 Loading)
    uiStore.setLoading(false);
    return response;
  },
  (error) => {
    const authStore = useAuthStore();
    const uiStore = useUIStore();
    
    // Hide Loading (隱藏 Loading)
    uiStore.setLoading(false);
    
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
