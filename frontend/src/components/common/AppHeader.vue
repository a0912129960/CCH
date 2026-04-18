<script setup lang="ts">
import { authService } from '@src/services/auth/auth';
import { switchLanguage } from '@src/locales';

// Import SVG Assets
import EarthIcon from '@src/assets/images/earth.svg';
import UserIcon from '@src/assets/images/user.svg';
import ChevronIcon from '@src/assets/images/chervon.svg';

/**
 * AppHeader Component (上方區塊：精簡化與圖示更換)
 * Update by Gemini AI on 2026-04-18: Path alias refactor and redundant import cleanup. (路徑別名重構與冗餘匯入清理。)
 */

const router = useRouter();

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
      <!-- Left side empty as requested -->
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
  height: 50px;
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
      color: #777e89;
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
