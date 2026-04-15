<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { authService } from '../../services/auth/auth';
import { switchLanguage } from '../../locales';
import { useRouter } from 'vue-router';
import { useUIStore } from '../../stores/ui';

/**
 * AppHeader Component (上方區塊：包含 Menu、多語系、User Name 與 Logout)
 * Update by Gemini AI on 2026-04-15
 */

const { locale, t } = useI18n();
const router = useRouter();
const uiStore = useUIStore();

const handleLogout = () => {
  authService.logout();
  router.push('/login');
};

const onLanguageChange = (lang: string) => {
  switchLanguage(lang);
};
</script>

<template>
  <header class="app-header">
    <div class="header-left">
      <!-- Menu Toggle -->
      <div class="collapse-btn" @click="uiStore.toggleSidebar">
        <i class="el-icon-menu"></i>
      </div>
      <div class="welcome-text">
        Welcome to Project <span class="project-name">CCH Compliance</span>
      </div>
    </div>

    <div class="header-right">
      <!-- Language Switcher -->
      <el-dropdown trigger="click" @command="onLanguageChange" class="lang-dropdown">
        <span class="el-dropdown-link">
          <i class="el-icon-basketball" style="margin-right: 5px;"></i>
          {{ locale.toUpperCase() }}
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="en">English</el-dropdown-item>
            <el-dropdown-item command="zh-TW">繁體中文</el-dropdown-item>
            <el-dropdown-item command="zh-CN">简体中文</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <!-- User Profile (User Name + Arrow -> Logout) -->
      <el-dropdown trigger="click" @command="handleLogout">
        <div class="user-profile">
          <span class="username">{{ authService.state.username }}</span>
          <i class="el-icon-arrow-down arrow-icon"></i>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item disabled class="role-display">{{ authService.state.role }}</el-dropdown-item>
            <el-dropdown-item divided command="logout" class="logout-item">
              <i class="el-icon-switch-button"></i> {{ $t('common.logout') }}
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </header>
</template>

<style scoped lang="scss">
.app-header {
  height: 64px;
  background: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  border-bottom: 1px solid #e1e8ed;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 20px;
  
  .collapse-btn {
    font-size: 20px;
    cursor: pointer;
    color: #909399;
    transition: color 0.2s;
    &:hover { color: #00a8e2; }
  }

  .welcome-text {
    font-size: 14px;
    color: #606266;
    .project-name {
      color: #00a8e2;
      font-weight: 700;
    }
  }
}

.header-right {
  display: flex;
  align-items: center;
  gap: 24px;

  .lang-dropdown {
    cursor: pointer;
    color: #606266;
    font-weight: 600;
    display: flex;
    align-items: center;
    &:hover { color: #00a8e2; }
  }

  .user-profile {
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
    padding-left: 20px;
    border-left: 1px solid #e1e8ed;
    color: #333;
    transition: color 0.2s;

    &:hover {
      color: #00a8e2;
    }

    .username {
      font-size: 14px;
      font-weight: 600;
    }

    .arrow-icon {
      font-size: 12px;
      color: #909399;
    }
  }
}

.role-display {
  font-size: 12px;
  font-weight: bold;
  color: #00a8e2 !important;
}

.logout-item {
  color: #f56c6c;
}
</style>
