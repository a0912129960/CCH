import { defineStore } from 'pinia';
import { ref } from 'vue';

/**
 * UI Store for managing layout states (管理佈局狀態的 UI Store)
 * Audit by Gemini AI on 2026-04-09
 */
export const useUIStore = defineStore('ui', () => {
  const isSidebarCollapsed = ref(false);

  const toggleSidebar = () => {
    isSidebarCollapsed.value = !isSidebarCollapsed.value;
  };

  const setSidebar = (state: boolean) => {
    isSidebarCollapsed.value = state;
  };

  return {
    isSidebarCollapsed,
    toggleSidebar,
    setSidebar
  };
});
