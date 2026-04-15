<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router';
import { authService, UserRole } from '../../services/auth/auth';
import { useUIStore } from '../../stores/ui';
import { storeToRefs } from 'pinia';
import { computed } from 'vue';
import MyDimercoLogo from '@/assets/images/circle-logo.svg';

/**
 * Sidebar Component aligned with MyDimerco brand layout.
 * Update by Gemini AI on 2026-04-15
 */

const router = useRouter();
const uiStore = useUIStore();
const { isSidebarCollapsed: isCollapsed } = storeToRefs(uiStore);

const userRole = computed(() => authService.state.role);
const dashboardPath = computed(() => {
  if (userRole.value === UserRole.DIMERCO || userRole.value === UserRole.DCB) return '/employee';
  return '/customer';
});
</script>

<template>
  <aside class="sidebar" :class="{ 'is-collapsed': isCollapsed }">
    <!-- Sidebar Header: MyDimerco Logo -->
    <div class="sidebar-header">
      <div class="brand">
        <img :src="MyDimercoLogo" alt="Logo" class="brand-logo" />
        <span v-if="!isCollapsed" class="brand-name">MyDimerco</span>
      </div>
    </div>

    <!-- Navigation Menu -->
    <nav class="sidebar-nav">
      <RouterLink :to="dashboardPath" class="nav-item">
        <i class="el-icon-odometer icon"></i>
        <span v-if="!isCollapsed" class="nav-text">Dashboard</span>
      </RouterLink>
      
      <RouterLink to="/parts" class="nav-item">
        <i class="el-icon-search icon"></i>
        <span v-if="!isCollapsed" class="nav-text">Track and Trace</span>
      </RouterLink>

      <div class="nav-item disabled">
        <i class="el-icon-document icon"></i>
        <span v-if="!isCollapsed" class="nav-text">Report Center</span>
      </div>

      <div class="nav-item disabled">
        <i class="el-icon-chat-dot-round icon"></i>
        <span v-if="!isCollapsed" class="nav-text">Message Center</span>
      </div>

      <div class="nav-item disabled">
        <i class="el-icon-notebook-2 icon"></i>
        <span v-if="!isCollapsed" class="nav-text">Bookings</span>
      </div>

      <div class="nav-item disabled">
        <i class="el-icon-setting icon"></i>
        <span v-if="!isCollapsed" class="nav-text">System Management</span>
      </div>

      <div class="nav-item disabled">
        <i class="el-icon-box icon"></i>
        <span v-if="!isCollapsed" class="nav-text">Inventory</span>
      </div>
    </nav>
  </aside>
</template>

<style scoped lang="scss">
.sidebar {
  width: 260px;
  background-color: #344759;
  height: 100vh;
  display: flex;
  flex-direction: column;
  position: fixed;
  left: 0;
  top: 0;
  color: white;
  z-index: 1000;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.sidebar.is-collapsed {
  width: 80px;
}

.sidebar-header {
  height: 64px;
  display: flex;
  align-items: center;
  padding: 0 20px;
  background-color: #344759;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
}

.brand-logo {
  width: 32px;
  height: 32px;
}

.brand-name {
  font-size: 1.25rem;
  font-weight: 700;
  color: white;
  letter-spacing: -0.5px;
}

.sidebar-nav {
  flex: 1;
  padding-top: 10px;
}

.nav-item {
  height: 50px;
  display: flex;
  align-items: center;
  padding: 0 20px;
  color: #fff;
  text-decoration: none;
  transition: all 0.2s ease;
  cursor: pointer;
  border-left: 4px solid transparent;

  .icon {
    font-size: 18px;
    width: 24px;
    text-align: center;
    margin-right: 15px;
  }

  .nav-text {
    font-size: 14px;
    font-weight: 600;
  }

  &:hover:not(.disabled) {
    background-color: rgba(255, 255, 255, 0.05);
  }

  &.router-link-active {
    background-color: #00a8e2;
    color: white;
  }

  &.disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
}

.is-collapsed {
  .nav-item {
    justify-content: center;
    padding: 0;
    .icon {
      margin-right: 0;
    }
  }
}
</style>
