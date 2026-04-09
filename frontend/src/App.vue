<script setup lang="ts">
import { RouterView, RouterLink, useRoute } from 'vue-router'
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { authService, UserRole } from './services/auth';
import { switchLanguage } from './locales';
import Sidebar from './components/common/Sidebar.vue';

/**
 * Main App Component (主應用程式組件)
 * Fixed: Explicitly import RouterLink and added defensive checks.
 */
const route = useRoute();
const { locale } = useI18n();

/**
 * Decide which layout to show (決定顯示哪種佈局)
 */
const isCustomerLayout = computed(() => {
  const state = authService.state;
  return state && authService.isAuthenticated() && state.role === UserRole.CUSTOMER;
});

const isLogin = computed(() => route.name === 'login');

/**
 * Handle language change (處理語系切換)
 */
const onLanguageChange = (event: Event) => {
  const target = event.target as HTMLSelectElement;
  switchLanguage(target.value);
};
</script>

<template>
  <div :class="{ 'app-layout': isCustomerLayout }">
    <!-- 1. Customer Sidebar (客戶側邊欄佈局) -->
    <Sidebar v-if="isCustomerLayout" />

    <!-- 2. Main Content Wrapper -->
    <div :class="{ 'main-content': isCustomerLayout }">
      
      <!-- 3. Standard Header (for Employee or Unauthenticated) -->
      <header v-if="!isCustomerLayout && !isLogin">
        <div class="header-content">
          <nav v-if="authService.isAuthenticated()">
            <RouterLink to="/">{{ $t('common.home') }}</RouterLink>
            <RouterLink v-if="authService.state?.role === UserRole.EMPLOYEE" to="/employee">{{ $t('common.dashboard') }}</RouterLink>
          </nav>
          
          <div class="lang-switcher">
            <select :value="locale" @change="onLanguageChange">
              <option value="en">English</option>
              <option value="zh-TW">繁體中文</option>
              <option value="zh-CN">简体中文</option>
            </select>
          </div>
        </div>
      </header>

      <!-- 4. Router View -->
      <main>
        <RouterView />
      </main>
    </div>
  </div>
</template>

<style scoped>
.app-layout {
  display: flex;
  min-height: 100vh;
}

.main-content {
  flex: 1;
  background-color: var(--dashboard-bg, #f3f6f8);
  min-height: 100vh;
}

/* Sidebar push margin only when sidebar is present */
.app-layout .main-content {
  margin-left: 260px;
}

header {
  line-height: 1.5;
  padding: 1rem;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

nav a.router-link-exact-active {
  color: var(--primary-color, #00a8e2);
  font-weight: bold;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  text-decoration: none;
  color: #666;
}

.lang-switcher select {
  padding: 4px 8px;
  border-radius: 4px;
  border: 1px solid #ddd;
}

main {
  width: 100%;
}
</style>
