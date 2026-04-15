import axios from 'axios';
import { ElMessage } from 'element-plus';
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
    uiStore.setLoading(false);

    // Global Success/Error Handling (全域成功/錯誤處理)
    const apiResult = response.data;
    if (apiResult && typeof apiResult.success === 'boolean' && !apiResult.success) {
      // If success is false, show error message via ElMessage (如果 success 為 false，顯示錯誤訊息)
      ElMessage.error(apiResult.message || 'Unknown Error (未知錯誤)');
    }

    return response;
  },
  (error) => {
    const authStore = useAuthStore();
    const uiStore = useUIStore();
    uiStore.setLoading(false);
    
    // Handle Network/Server Errors (處理網路/伺服器錯誤)
    const message = error.response?.data?.message || 'Network unstable, please try again later. (網路不穩定，請稍後再嘗試。)';
    ElMessage.error(message);
    
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
