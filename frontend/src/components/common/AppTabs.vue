<script setup lang="ts">
import { useTabStore } from '../../stores/tabs';
import { useRouter } from 'vue-router';

/**
 * Multi-Tab Navigation Component (多標籤導覽組件)
 * Update by Gemini AI on 2026-04-15
 */

const tabStore = useTabStore();
const router = useRouter();

const handleTabClick = (path: string) => {
  router.push(path);
};

const handleTabRemove = (path: string) => {
  tabStore.removeTab(path);
};
</script>

<template>
  <div class="app-tabs">
    <div 
      v-for="tab in tabStore.openTabs" 
      :key="tab.path"
      class="tab-item"
      :class="{ 'is-active': tabStore.activeTabPath === tab.path }"
      @click="handleTabClick(tab.path)"
    >
      <span v-if="tabStore.activeTabPath === tab.path" class="tab-dot"></span>
      <span class="tab-title">{{ $t(tab.title) }}</span>
      <i class="el-icon-close close-icon" @click.stop="handleTabRemove(tab.path)"></i>
    </div>
  </div>
</template>

<style scoped lang="scss">
.app-tabs {
  height: 44px;
  background: #fff;
  display: flex;
  align-items: center;
  padding: 0 16px;
  border-bottom: 1px solid #e8eef2;
  gap: 8px;
  overflow-x: auto;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.02);

  &::-webkit-scrollbar {
    height: 0px;
  }

  .tab-item {
    height: 32px;
    padding: 0 12px;
    background: #f5f7f9;
    border: 1px solid #e1e8ed;
    border-radius: 4px;
    display: flex;
    align-items: center;
    gap: 8px;
    cursor: pointer;
    font-size: 13px;
    color: #606266;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    white-space: nowrap;

    &:hover {
      background: #eef2f5;
      color: #00a8e2;
      border-color: #00a8e2;
    }

    &.is-active {
      background: #00a8e2;
      color: #fff;
      border-color: #00a8e2;
      font-weight: 500;
      box-shadow: 0 2px 8px rgba(0, 168, 226, 0.25);

      .close-icon {
        color: rgba(255, 255, 255, 0.8);
        &:hover {
          background: rgba(255, 255, 255, 0.2);
          color: #fff;
        }
      }
    }

    .close-icon {
      font-size: 14px; /* Increased size (加大尺寸) */
      width: 18px;
      height: 18px;
      line-height: 18px;
      text-align: center;
      border-radius: 50%;
      transition: all 0.2s;
      margin-left: 4px;
      color: #909399; /* Visible gray (清晰的灰色) */
      
      &:hover {
        background: #f56c6c;
        color: #fff !important;
      }
    }
  }
}
</style>
