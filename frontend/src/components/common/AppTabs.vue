<script setup lang="ts">
import { useTabStore } from '@src/stores/tabs';

/**
 * Multi-Tab Navigation Component (多標籤導覽組件)
 * Fixed: SVG Close buttons, corrected context menu i18n.
 * Update by Gemini AI on 2026-04-18: Path alias refactor and redundant import cleanup. (路徑別名重構與冗餘匯入清理。)
 */

const tabStore = useTabStore();
const router = useRouter();

// Context Menu State
const showMenu = ref(false);
const menuPos = ref({ x: 0, y: 0 });
const selectedPath = ref('');

const handleTabClick = (path: string) => {
  router.push(path);
};

const handleTabRemove = (path: string) => {
  tabStore.removeTab(path);
};

const handleRightClick = (e: MouseEvent, path: string) => {
  e.preventDefault();
  selectedPath.value = path;
  menuPos.value = { x: e.clientX, y: e.clientY };
  showMenu.value = true;
};

const closeMenu = () => {
  showMenu.value = false;
};

const closeAll = () => {
  tabStore.closeAll();
  closeMenu();
};

const closeOthers = () => {
  tabStore.closeOthers(selectedPath.value);
  closeMenu();
};

const handleRefresh = () => {
  tabStore.refreshTab(selectedPath.value);
  closeMenu();
};

onMounted(() => {
  window.addEventListener('click', closeMenu);
});
onUnmounted(() => {
  window.removeEventListener('click', closeMenu);
});
</script>

<template>
  <div class="app-tabs-container">
    <div class="app-tabs">
      <div 
        v-for="tab in tabStore.openTabs" 
        :key="tab.path"
        class="tab-item"
        :class="{ 'is-active': tabStore.activeTabPath === tab.path }"
        @click="handleTabClick(tab.path)"
        @contextmenu="handleRightClick($event, tab.path)"
      >
        <!-- Use i18n for title, but if title doesn't contain '.' it's likely a part no -->
        <span class="tab-title">{{ tab.title.includes('.') ? $t(tab.title) : tab.title }}</span>
        
        <div class="tab-actions">
          <!-- SVG Close Button -->
          <span class="action-btn close-btn" @click.stop="handleTabRemove(tab.path)">
            <svg viewBox="0 0 1024 1024" class="action-svg" xmlns="http://www.w3.org/2000/svg">
              <path fill="currentColor" d="M764.288 214.592 512 466.88 259.712 214.592a31.936 31.936 0 0 0-45.12 45.12L466.88 512 214.592 764.288a31.936 31.936 0 1 0 45.12 45.12L512 557.12l252.288 252.288a31.936 31.936 0 1 0 45.12-45.12L557.12 512l252.288-252.288a31.936 31.936 0 0 0-45.12-45.12z"></path>
            </svg>
          </span>
        </div>
      </div>
    </div>

    <!-- Context Menu (純多語系) -->
    <ul v-if="showMenu" class="context-menu" :style="{ top: menuPos.y + 'px', left: menuPos.x + 'px' }">
      <li @click="handleRefresh">{{ $t('common.refresh') }}</li>
      <li @click="closeOthers">{{ $t('common.close_others') }}</li>
      <li @click="closeAll">{{ $t('common.close_all') }}</li>
    </ul>
  </div>
</template>

<style scoped lang="scss">
.app-tabs-container {
  background: #fff;
  border-bottom: 1px solid #e1e8ed;
  position: relative;
  z-index: 10;
}

.app-tabs {
  height: 40px;
  display: flex;
  align-items: flex-end;
  padding: 0 12px;
  gap: 2px;
  overflow-x: auto;

  &::-webkit-scrollbar { height: 0; }

  .tab-item {
    height: 34px;
    min-width: 140px;
    padding: 0 10px;
    background: #f5f7f9;
    border: 1px solid #e1e8ed;
    border-bottom: none;
    border-radius: 6px 6px 0 0;
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 8px;
    cursor: pointer;
    font-size: 12px;
    color: #606266;
    transition: all 0.2s;
    user-select: none;

    &:hover {
      background: #eef2f5;
      color: #00a8e2;
    }

    &.is-active {
      background: #fff;
      color: #00a8e2;
      font-weight: 600;
      height: 36px;
      border-top: 2px solid #00a8e2;
      z-index: 2;

      .action-svg {
        color: #909399;
      }
    }

    .tab-title {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      flex: 1;
    }

    .tab-actions {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .action-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 18px;
      height: 18px;
      border-radius: 50%;
      transition: all 0.2s;

      &:hover {
        background: rgba(0, 0, 0, 0.05);
      }
    }

    .close-btn:hover {
      .action-svg { color: #f56c6c; }
    }

    .action-svg {
      width: 12px;
      height: 12px;
      color: #c0c4cc;
    }
  }
}

.context-menu {
  position: fixed;
  z-index: 3000;
  background: #fff;
  border: 1px solid #e1e8ed;
  border-radius: 4px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  list-style: none;
  padding: 5px 0;
  margin: 0;
  min-width: 140px;

  li {
    padding: 8px 16px;
    font-size: 13px;
    color: #606266;
    cursor: pointer;

    &:hover {
      background: #f5f7f9;
      color: #00a8e2;
    }
  }
}
</style>
