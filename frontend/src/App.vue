<script setup lang="ts">
import { authService } from '@src/services/auth/auth';
import Sidebar from '@src/components/common/Sidebar.vue';
import AppHeader from '@src/components/common/AppHeader.vue';
import AppTabs from '@src/components/common/AppTabs.vue';
import AppFooter from '@src/components/common/AppFooter.vue';
import Loading from '@src/components/common/Loading.vue';
import { useUIStore } from '@src/stores/ui';
import { useTabStore } from '@src/stores/tabs';

/**
 * Main App Component (主應用程式組件)
 * Integrated Multi-Tab Layout and Global Header.
 * Update by Gemini AI on 2026-04-18: Global import cleanup, path alias refactor, and applied professional standards. (全域匯入清理、路徑別名重構並套用專業標準。)
 */
const route = useRoute();
const uiStore = useUIStore();
const tabStore = useTabStore();
const { isSidebarCollapsed, isLoading } = storeToRefs(uiStore);

/**
 * Decide which layout to show (決定顯示哪種佈局)
 */
const isSidebarLayout = computed(() => {
  return authService.isAuthenticated() && route.name !== 'login';
});

/**
 * Watch route changes to add tabs (監聽路由變動以新增標籤)
 */
watch(
  () => route.path,
  () => {
    // Only add tab if it's not the root path (僅在非根目錄時新增頁籤)
    if (isSidebarLayout.value && route.meta.requiresAuth && route.path !== '/') {
      // INTERNAL-AI-20260417: If navigating to part-detail, check if we already have a better title or use a placeholder that gets updated.
      // (INTERNAL-AI-20260417: 若導向零件詳情，檢查是否已有更好的標題，或使用會被更新的預設值。)
      const existingTab = tabStore.openTabs.find(t => t.path === route.path);
      
      tabStore.addTab({
        title: existingTab?.title || (route.meta.title as string) || 'common.home',
        path: route.path,
        name: (route.name as string) || ''
      });
    }
  },
  { immediate: true }
);
</script>

<template>
  <div :class="{ 'app-layout': isSidebarLayout }">
    <!-- 0. Global Loading -->
    <Loading v-if="isLoading" />

    <!-- 1. Sidebar -->
    <Sidebar v-if="isSidebarLayout" />

    <!-- 2. Main Container -->
    <div :class="{ 'main-container': isSidebarLayout, 'is-collapsed': isSidebarCollapsed }">
      
      <!-- 3. Header & Tabs (Framework) -->
      <template v-if="isSidebarLayout">
        <AppHeader />
        <AppTabs />
      </template>

      <!-- 4. Content Area -->
      <main class="content-area">
        <RouterView v-slot="{ Component }">
          <keep-alive :exclude="['LoginView']">
            <component :is="Component" :key="route.fullPath" />
          </keep-alive>
        </RouterView>
      </main>

      <!-- 5. Footer (Based on mydimercolayout.png) -->
      <AppFooter v-if="isSidebarLayout" />
    </div>
  </div>
</template>

<style scoped lang="scss">
.app-layout {
  display: flex;
  min-height: 100vh;
  background-color: #f0f2f5;
}

.main-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100vh; /* Fixed height to support internal scroll */
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  
  // Account for sidebar fixed width
  margin-left: 260px;
  
  &.is-collapsed {
    margin-left: 80px;
  }
}

.content-area {
  flex: 1;
  padding: 0; /* Set to 0 to maximize space (設為 0 以極大化空間) */
  overflow-y: auto;
  background-color: #f5f7f9;
  position: relative;
}

main {
  width: 100%;
}
</style>
