<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import { computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { authService } from './services/auth';
import { switchLanguage } from './locales';

/**
 * Main App Component (主應用程式組件)
 */
const route = useRoute();
const { locale } = useI18n();

/**
 * Hide header if on login page (如果在登入頁則隱藏頁首)
 */
const showHeader = computed(() => {
  return route.name !== 'login';
});

/**
 * Handle language change (處理語系切換)
 * @param {Event} event - Selection change event (選擇變更事件)
 */
const onLanguageChange = (event: Event) => {
  const target = event.target as HTMLSelectElement;
  switchLanguage(target.value);
};
</script>

<template>
  <header>
    <div class="header-content">
      <nav v-if="authService.isAuthenticated()">
        <RouterLink to="/">{{ $t('common.home') }}</RouterLink>
        <RouterLink v-if="authService.state.role === 'EMPLOYEE'" to="/employee">{{ $t('common.dashboard') }}</RouterLink>
        <RouterLink v-if="authService.state.role === 'CUSTOMER'" to="/customer">{{ $t('common.portal') }}</RouterLink>
      </nav>
      
      <!-- Language Switcher (語系切換器) -->
      <div class="lang-switcher">
        <select :value="locale" @change="onLanguageChange">
          <option value="en">English</option>
          <option value="zh-TW">繁體中文</option>
          <option value="zh-CN">简体中文</option>
        </select>
      </div>
    </div>
  </header>

  <main>
    <RouterView />
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
  padding: 1rem;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  margin-bottom: 2rem;
}

.header-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.app-brand {
  display: flex;
  align-items: center;
}

.brand-name {
  font-size: 1.5rem;
  font-weight: bold;
  color: var(--primary-color, #00a8e2);
  margin-right: 2rem;
}

nav {
  font-size: 1rem;
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

nav a:not(:first-child) {
  border-left: 1px solid #ddd;
}

.lang-switcher select {
  padding: 4px 8px;
  border-radius: 4px;
  border: 1px solid #ddd;
  outline: none;
}

main {
  min-height: calc(100vh - 80px);
}
</style>
