<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { authService } from '../services/auth';
import { useRouter } from 'vue-router';
import { dashboardService, type StatusCount, type SLAItem } from '../services/dashboard';
import Card from '../components/common/Card.vue';
import Dot from '../components/common/Dot.vue';

/**
 * Customer Dashboard View (客戶儀表板頁面)
 * Style adjusted to match MyDimerco brand language.
 * BR-28: Status Summary | BR-29: SLA Countdown
 */

const router = useRouter();
const username = authService.state.username;

const statusSummary = ref<StatusCount[]>([]);
const slaItems = ref<SLAItem[]>([]);
const loading = ref(true);

onMounted(async () => {
  try {
    const [summary, sla] = await Promise.all([
      dashboardService.getStatusSummary(),
      dashboardService.getSLAItems()
    ]);
    statusSummary.value = summary;
    slaItems.value = sla;
  } finally {
    loading.value = false;
  }
});

const handleLogout = () => {
  authService.logout();
  router.push('/login');
};
</script>

<template>
  <div class="dashboard-wrapper">
    <div class="dashboard-container">
      <header class="dashboard-header">
        <div class="title-group">
          <h1>{{ $t('customer.title') }}</h1>
          <p class="subtitle">{{ $t('common.app_full_name') }}</p>
        </div>
        <div class="user-action">
          <div class="user-info">
            <span class="welcome-text">{{ $t('common.welcome') }},</span>
            <span class="username">{{ username }}</span>
          </div>
          <button class="logout-btn" @click="handleLogout">{{ $t('common.logout') }}</button>
        </div>
      </header>

      <div v-if="loading" class="loading-overlay">
        <div class="spinner"></div>
      </div>
      
      <div v-else class="dashboard-content">
        <!-- BR-28: Status Summary (狀態摘要) -->
        <section class="summary-section">
          <div class="section-header">
            <div class="decorator"></div>
            <h3>{{ $t('customer.status_summary') }}</h3>
          </div>
          <div class="status-grid">
            <Card v-for="item in statusSummary" :key="item.status" class="status-card">
              <div class="status-header">
                <Dot :color="item.color" size="10px" />
                <span class="status-label">{{ $t(item.labelKey) }}</span>
              </div>
              <div class="status-value">{{ item.count }}</div>
            </Card>
          </div>
        </section>

        <!-- BR-29: SLA Countdown (SLA 倒數) -->
        <section class="sla-section">
          <div class="section-header">
            <div class="decorator secondary"></div>
            <h3>{{ $t('customer.sla_countdown') }}</h3>
          </div>
          <div class="sla-list">
            <Card v-for="item in slaItems" :key="item.id" class="sla-card">
              <div class="sla-main">
                <div class="part-badge">
                  <span class="part-label">{{ $t('customer.part_no') }}</span>
                  <span class="part-no">{{ item.partNo }}</span>
                </div>
                <span class="status-pill" :style="{ backgroundColor: item.remainingMinutes < 60 ? 'var(--warning-color)' : 'var(--primary-color)' }">
                  {{ $t('status.' + item.status.toLowerCase()) }}
                </span>
              </div>
              <div class="sla-countdown" :class="{ 'urgent': item.remainingMinutes < 60 }">
                <span class="timer-icon">🕒</span>
                <span class="timer-text">{{ $t('customer.sla_remaining', { min: item.remainingMinutes }) }}</span>
              </div>
            </Card>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-wrapper {
  background-color: var(--dashboard-bg, #f3f6f8);
  min-height: 100vh;
  width: 100%;
}

.dashboard-container {
  padding: 2rem;
  max-width: 1400px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

/* Header Styles */
.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2.5rem;
}

h1 {
  font-size: 2rem;
  color: var(--sidebar-color, #465363);
  margin-bottom: 0.2rem;
}

.subtitle {
  color: #8898aa;
  font-size: 0.9rem;
}

.user-action {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.5rem;
}

.user-info {
  font-size: 0.9rem;
  color: var(--sidebar-color);
}

.username {
  margin-left: 0.3rem;
  color: var(--primary-color, #00a8e2);
  font-weight: bold;
}

.logout-btn {
  background: white;
  color: #f56c6c;
  border: 1px solid #f56c6c;
  padding: 4px 12px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.8rem;
  transition: all 0.2s;
}

.logout-btn:hover {
  background: #f56c6c;
  color: white;
}

/* Section Common Styles */
.section-header {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  margin-bottom: 1.5rem;
}

.decorator {
  width: 4px;
  height: 24px;
  background-color: var(--primary-color);
  border-radius: 2px;
}

.decorator.secondary {
  background-color: var(--warning-color, #f68b39);
}

h3 {
  font-size: 1.25rem;
  color: var(--sidebar-color);
}

/* Status Summary Grid */
.status-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 1.2rem;
  margin-bottom: 3rem;
}

.status-card {
  padding: 1.25rem;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
  background: white;
  transition: transform 0.2s;
}

.status-card:hover {
  transform: translateY(-4px);
}

.status-header {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin-bottom: 1rem;
}

.status-label {
  font-size: 0.85rem;
  color: #8898aa;
}

.status-value {
  font-size: 2rem;
  font-weight: bold;
  color: var(--sidebar-color);
}

/* SLA List Styles */
.sla-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.sla-card {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.2rem 2rem;
  border-radius: 12px;
  background: white;
  box-shadow: 0 2px 8px rgba(0,0,0,0.03);
}

.sla-main {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.part-badge {
  display: flex;
  flex-direction: column;
}

.part-label {
  font-size: 0.75rem;
  color: #adb5bd;
}

.part-no {
  font-size: 1.1rem;
  color: var(--sidebar-color);
  font-weight: bold;
}

.status-pill {
  color: white;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
}

.sla-countdown {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  color: #525f7f;
}

.sla-countdown.urgent {
  color: var(--warning-color);
}

.timer-text {
  font-size: 1.1rem;
  font-weight: bold;
}

/* Utils */
.loading-overlay {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 300px;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(0,0,0,0.1);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
