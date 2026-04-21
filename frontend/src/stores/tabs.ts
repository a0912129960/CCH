import { defineStore } from 'pinia';
import { ref } from 'vue';
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
  const refreshKeys = ref<Record<string, number>>({});

  // Add a new tab (新增標籤)
  function addTab(tab: TabItem) {
    // Never add dashboard (/) to the tab list (永遠不將儀表板加入頁籤列表)
    if (tab.path === '/') return;

    const exists = openTabs.value.find(t => t.path === tab.path);
    if (!exists) {
      openTabs.value.push(tab);
      refreshKeys.value[tab.path] = 0;
    }
    activeTabPath.value = tab.path;
  }

  // Refresh a specific tab (重新整理特定標籤)
  function refreshTab(path: string) {
    if (refreshKeys.value[path] !== undefined) {
      refreshKeys.value[path]++;
    } else {
      refreshKeys.value[path] = 1;
    }
  }

  // Remove a tab (關閉標籤)
  function removeTab(targetPath: string) {
    const index = openTabs.value.findIndex(t => t.path === targetPath);
    if (index === -1) return;

    const isRemovingActive = activeTabPath.value === targetPath;
    openTabs.value.splice(index, 1);
    delete refreshKeys.value[targetPath];

    // If no tabs left, redirect to home (如果沒有標籤剩餘，導向首頁)
    if (openTabs.value.length === 0) {
      activeTabPath.value = '';
      router.push('/');
      return;
    }

    // If removing active, jump to the last tab (如果關閉的是目前頁面，跳轉至最後一個標籤)
    if (isRemovingActive) {
      const nextTab = openTabs.value[openTabs.value.length - 1];
      router.push(nextTab.path);
    }
  }

  // Close all tabs (關閉全部標籤)
  function closeAll() {
    openTabs.value = [];
    activeTabPath.value = '';
    // Redirect to default dashboard based on role (根據角色導向預設儀表板)
    router.push('/');
  }

  // Close other tabs (關閉其他標籤)
  function closeOthers(currentPath: string) {
    const currentTab = openTabs.value.find(t => t.path === currentPath);
    if (currentTab) {
      openTabs.value = [currentTab];
      activeTabPath.value = currentPath;
      if (route.path !== currentPath) {
        router.push(currentPath);
      }
    }
  }

  // Update a specific tab title (動態更新標籤標題)
  function updateTabTitle(path: string, newTitle: string) {
    const tab = openTabs.value.find(t => t.path === path);
    if (tab) {
      tab.title = newTitle;
    }
  }

  return { openTabs, activeTabPath, refreshKeys, addTab, removeTab, refreshTab, closeAll, closeOthers, updateTabTitle };
});
