<script setup lang="ts">
import { authService } from '../services/auth';
import { useRouter } from 'vue-router';

/**
 * Employee Dashboard View (員工儀表板頁面)
 */
const router = useRouter();
const username = authService.state.username;

const handleLogout = () => {
  authService.logout();
  router.push('/login');
};
</script>

<template>
  <div class="dashboard-container">
    <h1>Employee Dashboard (員工儀表板)</h1>
    <div class="welcome-card">
      <p>Welcome, <strong>{{ username }}</strong>! (歡迎，{{ username }}！)</p>
      <p>Role: <span class="badge">Dimerco Employee (Dimerco 員工)</span></p>
      
      <div class="actions">
        <button class="logout-btn" @click="handleLogout">Logout (登出)</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-container {
  padding: 2rem;
  max-width: 800px;
  margin: 0 auto;
}

h1 {
  color: var(--primary-color, #00a8e2);
  margin-bottom: 2rem;
}

.welcome-card {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.badge {
  background: #e7f4fa;
  color: #00a8e2;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: bold;
}

.actions {
  margin-top: 2rem;
}

.logout-btn {
  background: #f68b39;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
}

.logout-btn:hover {
  background: #ff6b00;
}
</style>
