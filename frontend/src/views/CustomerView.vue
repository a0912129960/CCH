<script setup lang="ts">
import { authService } from '../services/auth';
import { useRouter } from 'vue-router';

/**
 * Customer Portal View (客戶入口頁面)
 */
const router = useRouter();
const username = authService.state.username;

const handleLogout = () => {
  authService.logout();
  router.push('/login');
};
</script>

<template>
  <div class="portal-container">
    <h1>{{ $t('customer.title') }}</h1>
    <div class="welcome-card">
      <p>{{ $t('common.welcome') }}, <strong>{{ username }}</strong>!</p>
      <p>{{ $t('common.role') }}: <span class="badge">{{ $t('customer.role_name') }}</span></p>
      
      <div class="actions">
        <button class="logout-btn" @click="handleLogout">{{ $t('common.logout') }}</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.portal-container {
  padding: 2rem;
  max-width: 800px;
  margin: 0 auto;
}

h1 {
  color: #465363;
  margin-bottom: 2rem;
}

.welcome-card {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  border-left: 4px solid #465363;
}

.badge {
  background: #f1f3f5;
  color: #465363;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: bold;
}

.actions {
  margin-top: 2rem;
}

.logout-btn {
  background: #6c757d;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.logout-btn:hover {
  background: #5a6268;
}
</style>
