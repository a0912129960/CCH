import { defineStore } from 'pinia';
import { ref, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';

export interface TabItem {
  title: string;
  path: string;
  name: string;
}

/**
 * Tab Store for managing multi-tab navigation (管理多標籤導覽的 Store)
 * Update by Gemini AI on 2026-04-15
 */
export const useTabStore = defineStore('tabs', () => {
  const router = useRouter();
  const route = useRoute();
  
  const openTabs = ref<TabItem[]>([]);
  const activeTabPath = ref('');

  // Add a new tab (新增標籤)
  function addTab(tab: TabItem) {
    const exists = openTabs.value.find(t => t.path === tab.path);
    if (!exists) {
      openTabs.value.push(tab);
    }
    activeTabPath.value = tab.path;
  }

  // Remove a tab (關閉標籤)
  function removeTab(targetPath: string) {
    const index = openTabs.value.findIndex(t => t.path === targetPath);
    if (index === -1) return;

    const isRemovingActive = activeTabPath.value === targetPath;
    openTabs.value.splice(index, 1);

    // If removing active, jump to the last tab or home
    // 如果關閉的是目前頁面，跳轉至最後一個標籤或首頁
    if (isRemovingActive) {
      if (openTabs.value.length > 0) {
        const nextTab = openTabs.value[openTabs.value.length - 1];
        router.push(nextTab.path);
      } else {
        router.push('/');
      }
    }
  }

  return { openTabs, activeTabPath, addTab, removeTab };
});
