<script setup lang="ts">
import { RouterLink, useRouter } from 'vue-router';
import { authService, UserRole } from '../../services/auth/auth';
import { useUIStore } from '../../stores/ui';
import { storeToRefs } from 'pinia';
import { computed } from 'vue';

// Import SVG Assets
import MyDimercoLogo from '@/assets/images/circle-logo.svg';
import HomeIcon from '@/assets/images/home.svg';
import SearchIcon from '@/assets/images/search_icon.svg';
import ChevronIcon from '@/assets/images/chervon.svg';

/**
 * Sidebar Component (左側區塊：包含 Hover 觸發的收合箭頭)
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

const toggleSidebar = () => {
  uiStore.toggleSidebar();
};
</script>

<template>
  <aside class="sidebar" :class="{ 'is-collapsed': isCollapsed }">
    <!-- Hover Toggle Button (滑鼠滑過時出現的收合箭頭) -->
    <div class="hover-toggle-container" @click="toggleSidebar">
      <div class="toggle-circle">
        <img 
          :src="ChevronIcon" 
          alt="Toggle" 
          class="toggle-chevron"
          :class="{ 'is-collapsed': isCollapsed }"
        />
      </div>
    </div>

    <!-- Sidebar Header: Dimerco CCH Logo -->
    <div class="sidebar-header">
      <div class="brand">
        <img :src="MyDimercoLogo" alt="Logo" class="brand-logo" />
        <transition name="fade">
          <span v-if="!isCollapsed" class="brand-name">Dimerco CCH</span>
        </transition>
      </div>
    </div>

    <!-- Navigation Menu -->
    <nav class="sidebar-nav">
      <!-- 1. Dashboard -->
      <RouterLink :to="dashboardPath" class="nav-item">
        <img :src="HomeIcon" alt="Home" class="nav-icon-svg" />
        <transition name="fade">
          <span v-if="!isCollapsed" class="nav-text">{{ $t('common.menu.dashboard') }}</span>
        </transition>
      </RouterLink>
      
      <!-- 2. Part List -->
      <RouterLink to="/parts" class="nav-item">
        <img :src="SearchIcon" alt="Search" class="nav-icon-svg" />
        <transition name="fade">
          <span v-if="!isCollapsed" class="nav-text">{{ $t('common.menu.parts') }}</span>
        </transition>
      </RouterLink>
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

  // When hovering the sidebar, show the toggle button (滑鼠滑過側邊欄時顯示切換按鈕)
  &:hover {
    .hover-toggle-container {
      opacity: 1;
      visibility: visible;
    }
  }
}

.sidebar.is-collapsed {
  width: 80px;
}

/* Hover Toggle Styles */
.hover-toggle-container {
  position: absolute;
  right: -12px;
  top: 32px;
  width: 24px;
  height: 24px;
  cursor: pointer;
  z-index: 1002;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;

  .toggle-circle {
    width: 24px;
    height: 24px;
    background-color: #00a8e2;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    border: 2px solid white;

    .toggle-chevron {
      width: 10px;
      height: 10px;
      filter: brightness(0) invert(1);
      transition: transform 0.3s;
      transform: rotate(90deg); // Facing left by default

      &.is-collapsed {
        transform: rotate(-90deg); // Facing right
      }
    }
  }

  &:hover .toggle-circle {
    background-color: #00b8f2;
    transform: scale(1.1);
  }
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
  white-space: nowrap;
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

  .nav-icon-svg {
    width: 20px;
    height: 20px;
    margin-right: 15px;
    filter: brightness(0) invert(1);
    opacity: 0.8;
  }

  .nav-text {
    font-size: 14px;
    font-weight: 600;
  }

  &:hover {
    background-color: rgba(255, 255, 255, 0.05);
    .nav-icon-svg { opacity: 1; }
  }

  &.router-link-active {
    background-color: #00a8e2;
    color: white;
    .nav-icon-svg { opacity: 1; }
  }
}

.is-collapsed {
  .nav-item {
    justify-content: center;
    padding: 0;
    .nav-icon-svg {
      margin-right: 0;
    }
  }
}

/* Fade Transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
