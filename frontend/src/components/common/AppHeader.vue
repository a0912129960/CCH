<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { authService } from '../../services/auth/auth';
import { switchLanguage } from '../../locales';
import { useRouter } from 'vue-router';
import { useUIStore } from '../../stores/ui';

// Import SVG Assets
import EarthIcon from '@/assets/images/earth.svg';
import UserIcon from '@/assets/images/user.svg';
import ChevronIcon from '@/assets/images/chervon.svg';

/**
 * AppHeader Component (上方區塊：精簡化與圖示更換)
 */

const { locale } = useI18n();
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
      <!-- Welcome Text Removed (已移除歡迎文字) -->
    </div>

    <div class="header-right">
      <!-- Language Switcher (Earth Icon) -->
      <el-dropdown trigger="click" @command="onLanguageChange" class="lang-dropdown">
        <span class="el-dropdown-link">
          <img :src="EarthIcon" alt="Earth" class="header-icon-svg" />
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="en">English</el-dropdown-item>
            <el-dropdown-item command="zh-TW">繁體中文</el-dropdown-item>
            <el-dropdown-item command="zh-CN">简体中文</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <!-- User Profile (User Icon + Name + Chevron -> Logout Only) -->
      <el-dropdown trigger="click" @command="(cmd) => cmd === 'logout' && handleLogout()">
        <div class="user-profile">
          <img :src="UserIcon" alt="User" class="user-icon-svg" />
          <span class="username">{{ authService.state.username }}</span>
          <img :src="ChevronIcon" alt="Chevron" class="chevron-icon-svg" />
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="logout" class="logout-item">
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
  flex-shrink: 0;
}

.header-left {
  display: flex;
  align-items: center;
  
  .collapse-btn {
    font-size: 20px;
    cursor: pointer;
    color: #909399;
    transition: color 0.2s;
    &:hover { color: #00a8e2; }
  }
}

.header-right {
  display: flex;
  align-items: center;
  gap: 24px;

  .header-icon-svg {
    width: 20px;
    height: 20px;
    margin-right: 6px;
  }

  .lang-dropdown {
    cursor: pointer;
    color: #606266;
    font-weight: 600;
    
    .el-dropdown-link {
      display: flex;
      align-items: center;
      &:hover { color: #00a8e2; }
    }

    .lang-text {
      font-size: 13px;
    }
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

    .user-icon-svg {
      width: 35px;
      height: 35px;
      opacity: 0.7;
    }

    .username {
      font-size: 16px;
      font-weight: 600;
    }

    .chevron-icon-svg {
      width: 8px;
      height: 8px;
      opacity: 0.5;
      transition: transform 0.3s;
    }
  }
}

/* Dropdown active state (選單開啟時旋轉小箭頭) */
.el-dropdown-self-define[aria-expanded="true"] {
  .chevron-icon-svg {
    transform: rotate(180deg);
  }
}

.logout-item {
  color: #f56c6c;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
}
</style>
