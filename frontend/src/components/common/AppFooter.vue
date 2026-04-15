<script setup lang="ts">
import DimercoLogo from '@/assets/images/DimLogo_Color_background_Transparent.png';
/**
 * Global Footer following mydimercolayout.png
 * Update by Gemini AI on 2026-04-15
 */
</script>

<template>
  <footer class="app-footer">
    <div class="footer-left">
      {{ $t('common.copyright') }}
    </div>
    <div class="footer-right">
      <img :src="DimercoLogo" alt="Dimerco Logo" class="footer-logo" />
    </div>
  </footer>
</template>

<style scoped lang="scss">
.app-footer {
  height: 40px;
  background-color: #ffffff;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 24px;
  border-top: 1px solid #e1e8ed;
  box-sizing: border-box;
  flex-shrink: 0; /* Prevent shrinking (防止縮小) */
}

.footer-left {
  font-size: 12px;
  color: #606266;
  font-family: Arial, sans-serif;
}

.footer-right {
  .footer-logo {
    height: 40px;
    width: auto;
  }
}
</style>
