<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router';
import { authService, UserRole } from '../../services/auth/auth';
import { useI18n } from 'vue-i18n';
import { switchLanguage } from '../../locales';
import { useUIStore } from '../../stores/ui';
import { storeToRefs } from 'pinia';
import { computed } from 'vue';

const router = useRouter();
const { locale } = useI18n();
const uiStore = useUIStore();
const { isSidebarCollapsed: isCollapsed } = storeToRefs(uiStore);

const userRole = computed(() => authService.state.role);
const dashboardPath = computed(() => {
  return userRole.value === UserRole.EMPLOYEE ? '/employee' : '/customer';
});

const handleLogout = () => {
  authService.logout();
  router.push('/login');
};

const onLanguageChange = (val: string) => {
  switchLanguage(val);
};

const toggleSidebar = () => {
  uiStore.toggleSidebar();
};
</script>

<template>
  <aside class="sidebar" :class="{ 'is-collapsed': isCollapsed }">
    <!-- Floating Toggle Button -->
    <button class="floating-toggle" @click="toggleSidebar" :title="isCollapsed ? 'Expand' : 'Collapse'">
      <i class="icon">{{ isCollapsed ? '›' : '‹' }}</i>
    </button>

    <div class="sidebar-header">
      <div class="brand">
        <img src="../../assets/images/circle-logo.svg" alt="Logo" class="brand-logo" />
        <span v-if="!isCollapsed" class="brand-name">CCH</span>
      </div>
    </div>

    <nav class="sidebar-nav">
      <RouterLink :to="dashboardPath" class="nav-item" :title="$t('common.menu.dashboard')">
        <i class="icon">📊</i>
        <span v-if="!isCollapsed">{{ $t('common.menu.dashboard') }}</span>
      </RouterLink>
      <RouterLink to="/parts" class="nav-item" :title="$t('common.menu.parts')">
        <i class="icon">📦</i>
        <span v-if="!isCollapsed">{{ $t('common.menu.parts') }}</span>
      </RouterLink>
    </nav>

    <div class="sidebar-footer">
      <div class="lang-switcher" v-if="!isCollapsed">
        <el-select 
          :model-value="locale" 
          @change="onLanguageChange" 
          class="lang-select" 
          size="small"
        >
          <el-option value="en" label="English" />
          <el-option value="zh-TW" label="繁體中文" />
          <el-option value="zh-CN" label="简体中文" />
        </el-select>
      </div>
      <button class="logout-link" @click="handleLogout" :title="$t('common.logout')">
        <i class="icon">🚪</i>
        <span v-if="!isCollapsed">{{ $t('common.logout') }}</span>
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
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 4px 0 10px rgba(0,0,0,0.1);
}

.sidebar.is-collapsed {
  width: 80px;
}

/* Floating Toggle Button Style */
.floating-toggle {
  position: absolute;
  right: -12px;
  top: 32px;
  width: 24px;
  height: 24px;
  background: var(--primary-color);
  border: none;
  border-radius: 50%;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
  z-index: 1001;
  transition: transform 0.3s ease;
}

.floating-toggle:hover {
  transform: scale(1.1);
  background: #00b8f2;
}

.sidebar-header {
  padding: 2.5rem 1.5rem;
  border-bottom: 1px solid rgba(255,255,255,0.05);
}

.is-collapsed .sidebar-header {
  padding: 2.5rem 0;
  display: flex;
  justify-content: center;
}

.brand {
  display: flex;
  align-items: center;
  gap: 0.8rem;
}

.brand-logo {
  width: 36px;
  height: 36px;
  transition: all 0.3s ease;
}

.brand-name {
  font-size: 1.4rem;
  font-weight: 800;
  letter-spacing: 1px;
  color: white;
  white-space: nowrap;
}

.sidebar-nav {
  flex: 1;
  padding: 1.5rem 0;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 1.2rem;
  padding: 0.9rem 1.5rem;
  color: rgba(255,255,255,0.8);
  text-decoration: none;
  transition: all 0.2s ease;
  margin: 4px 12px;
  border-radius: 8px;
}

.is-collapsed .nav-item {
  margin: 4px 8px;
  padding: 0.9rem 0;
  justify-content: center;
}

.nav-item:hover {
  background-color: rgba(255,255,255,0.1);
  color: white;
}

.nav-item.router-link-active {
  background-color: var(--primary-color);
  color: white;
  font-weight: 600;
  box-shadow: 0 4px 12px rgba(0, 168, 226, 0.3);
}

.icon {
  font-style: normal;
  font-size: 1.25rem;
}

.sidebar-footer {
  padding: 1.5rem;
  border-top: 1px solid rgba(255,255,255,0.05);
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.is-collapsed .sidebar-footer {
  padding: 1.5rem 0;
  align-items: center;
}

.lang-select :deep(.el-input__wrapper) {
  background-color: rgba(255,255,255,0.08) !important;
  box-shadow: none !important;
  border: 1px solid rgba(255,255,255,0.1) !important;
}

.lang-select :deep(.el-input__inner) {
  color: rgba(255,255,255,0.9) !important;
  font-size: 12px;
}

.logout-link {
  display: flex;
  align-items: center;
  gap: 1.2rem;
  background: none;
  border: none;
  color: #ff8a8a;
  cursor: pointer;
  padding: 0.6rem 1.5rem;
  font-size: 0.95rem;
  width: 100%;
  text-align: left;
  transition: all 0.2s;
}

.is-collapsed .logout-link {
  padding: 0.6rem 0;
  justify-content: center;
}

.logout-link:hover {
  background: rgba(245, 108, 108, 0.1);
  color: #f56c6c;
}
</style>
