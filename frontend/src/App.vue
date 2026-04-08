<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import { computed } from 'vue';
import { authService } from './services/auth';

/**
 * Main App Component (主應用程式組件)
 */
const route = useRoute();

/**
 * Hide header if on login page (如果在登入頁則隱藏頁首)
 */
const showHeader = computed(() => {
  return route.name !== 'login' && authService.isAuthenticated();
});
</script>

<template>
  <!-- Only show header if authenticated and not on login page (僅在已驗證且不在登入頁時顯示頁首) -->
  <header v-if="showHeader">
    <nav>
      <RouterLink to="/">Home (首頁)</RouterLink>
      <RouterLink v-if="authService.state.role === 'EMPLOYEE'" to="/employee">Dashboard (儀表板)</RouterLink>
      <RouterLink v-if="authService.state.role === 'CUSTOMER'" to="/customer">Portal (入口)</RouterLink>
    </nav>
  </header>

  <main>
    <RouterView />
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
  padding: 1rem;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  margin-bottom: 2rem;
}

nav {
  width: 100%;
  font-size: 1rem;
  text-align: center;
}

nav a.router-link-exact-active {
  color: var(--primary-color, #00a8e2);
  font-weight: bold;
}

nav a {
  display: inline-block;
  padding: 0 1rem;
  text-decoration: none;
  color: #666;
}

nav a:not(:first-child) {
  border-left: 1px solid #ddd;
}

main {
  min-height: calc(100vh - 80px);
}
</style>
