<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router';
import { authService } from '../../services/auth';
import { useI18n } from 'vue-i18n';
import { switchLanguage } from '../../locales';

const router = useRouter();
const { locale } = useI18n();

const handleLogout = () => {
  authService.logout();
  router.push('/login');
};

const onLanguageChange = (event: Event) => {
  const target = event.target as HTMLSelectElement;
  switchLanguage(target.value);
};
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-header">
      <div class="brand">
        <span class="brand-name">CCH</span>
      </div>
    </div>

    <nav class="sidebar-nav">
      <RouterLink to="/customer" class="nav-item">
        <i class="icon">📊</i>
        <span>{{ $t('common.menu.dashboard') }}</span>
      </RouterLink>
      <RouterLink to="/parts" class="nav-item">
        <i class="icon">📦</i>
        <span>{{ $t('common.menu.parts') }}</span>
      </RouterLink>
    </nav>

    <div class="sidebar-footer">
      <div class="lang-switcher">
        <select :value="locale" @change="onLanguageChange">
          <option value="en">EN</option>
          <option value="zh-TW">繁中</option>
          <option value="zh-CN">简中</option>
        </select>
      </div>
      <button class="logout-link" @click="handleLogout">
        <i class="icon">🚪</i>
        <span>{{ $t('common.logout') }}</span>
      </button>
    </div>
  </aside>
</template>

<style scoped>
.sidebar {
  width: 260px;
  background-color: var(--sidebar-color, #465363);
  height: 100vh;
  display: flex;
  flex-direction: column;
  position: fixed;
  left: 0;
  top: 0;
  color: white;
  z-index: 1000;
}

.sidebar-header {
  padding: 2rem 1.5rem;
  border-bottom: 1px solid rgba(255,255,255,0.1);
}

.brand-name {
  font-size: 1.5rem;
  font-weight: bold;
  letter-spacing: 2px;
  color: var(--primary-color, #00a8e2);
}

.sidebar-nav {
  flex: 1;
  padding: 2rem 0;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem 1.5rem;
  color: rgba(255,255,255,0.7);
  text-decoration: none;
  transition: all 0.2s;
}

.nav-item:hover {
  background-color: rgba(255,255,255,0.05);
  color: white;
}

.nav-item.router-link-active {
  background-color: var(--primary-color);
  color: white;
  border-right: 4px solid white;
}

.icon {
  font-style: normal;
  font-size: 1.2rem;
}

.sidebar-footer {
  padding: 1.5rem;
  border-top: 1px solid rgba(255,255,255,0.1);
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.lang-switcher select {
  width: 100%;
  padding: 6px;
  background: rgba(255,255,255,0.1);
  border: 1px solid rgba(255,255,255,0.2);
  color: white;
  border-radius: 4px;
  outline: none;
}

.logout-link {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: none;
  border: none;
  color: #f56c6c;
  cursor: pointer;
  padding: 0.5rem 0;
  font-size: 0.9rem;
  width: 100%;
  text-align: left;
}

.logout-link:hover {
  opacity: 0.8;
}
</style>
