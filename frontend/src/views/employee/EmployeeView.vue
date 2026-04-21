<script setup lang="ts">
import { authService } from '@src/services/auth/auth';
import { dashboardService, type StatusCount, type PendingReviewItem } from '@src/services/dashboard/dashboard';
import { partService, statusToI18nKey, statusToColor } from '@src/services/part/part';
import Card from '@src/components/common/Card.vue';
import Dot from '@src/components/common/Dot.vue';

/**
 * Employee Dashboard View (員工儀表板頁面)
 * BR-28: Status Summary | BR-30: Pending Review List
 * Update by Gemini AI on 2026-04-18: Global import cleanup and path alias refactor.
 */

const router = useRouter();
const username = computed(() => authService.state.username);

const customers = ref<{ id: string; name: string }[]>([]);
const selectedCustomerId = ref('all');
const statusSummary = ref<StatusCount[]>([]);
const pendingParts = ref<PendingReviewItem[]>([]);
const loading = ref(true);

const fetchData = async () => {
  loading.value = true;
  try {
    const [summary, pending] = await Promise.all([
      dashboardService.getStatusSummary(selectedCustomerId.value),
      dashboardService.getPendingReviewParts(selectedCustomerId.value)
    ]);
    statusSummary.value = summary;
    pendingParts.value = pending;
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  customers.value = await partService.getCustomers();
  await fetchData();
});

watch(selectedCustomerId, async () => {
  await fetchData();
});

const goToPartList = (status?: string) => {
  const query: any = { };
  if (status) query.status = status;
  if (selectedCustomerId.value !== 'all') query.customerId = selectedCustomerId.value;
  router.push({ name: 'parts', query });
};

const goToPartDetail = (id: number | string) => {
  router.push({ name: 'part-detail', params: { id } });
};

// Matches PartListView SLA color logic exactly (與 PartListView SLA 燈號邏輯一致)
const SLA_COLOR_MAP: Record<string, string> = {
  green:  '#67C23A',
  normal: '#67C23A',
  yellow: '#FADB14',
  orange: '#FF9900',
  red:    '#F56C6C'
};

const getSLAColor = (slaStatus?: string): string => {
  if (!slaStatus) return 'transparent';
  return SLA_COLOR_MAP[slaStatus.toLowerCase()] ?? 'transparent';
};

const formatDate = (dateStr: string) => {
  return dateStr.substring(0, 16);
};
</script>

<template>
  <div class="dashboard-wrapper">
    <div class="dashboard-container">
      <header class="dashboard-header">
        <div class="title-group">
          <h1>{{ $t('employee.title') }}</h1>
        </div>
        
        <div class="header-right">
          <div class="user-info">
            <span class="welcome-text">{{ $t('common.welcome') }},</span>
            <span class="username">{{ username }}</span>
          </div>
        </div>
      </header>

      <div v-if="loading" class="loading-overlay">
        <div class="spinner"></div>
      </div>
      
      <div v-else class="dashboard-content">
        <!-- Customer Selector (客戶選擇器) - Moved here -->
        <div class="customer-selector-container">
          <div class="customer-selector">
            <label>{{ $t('employee.customer_select') }}</label>
            <select v-model="selectedCustomerId" class="app-select">
              <option value="all">{{ $t('employee.all_customers') }}</option>
              <option v-for="c in customers" :key="c.id" :value="c.id">{{ c.name }}</option>
            </select>
          </div>
        </div>

        <!-- Status Summary (狀態摘要) -->
        <section class="summary-section">
          <div class="section-header">
            <div class="decorator"></div>
            <h3>{{ $t('employee.status_summary') }}</h3>
          </div>
          <div class="status-grid">
            <Card 
              v-for="item in statusSummary" 
              :key="item.status" 
              class="status-card"
              @click="goToPartList(item.status)"
            >
              <div class="status-header">
                <span class="status-label">{{ $t(item.labelKey) }}</span>
              </div>
              <div class="status-value">{{ item.count }}</div>
            </Card>
          </div>
        </section>

        <!-- Pending Review Table (待審核列表) -->
        <section class="pending-section">
          <div class="section-header">
            <div class="decorator secondary"></div>
            <h3>{{ $t('employee.pending_audit') }}</h3>
          </div>
          
          <div class="table-container card-shadow">
            <table class="app-table">
              <thead>
                <tr>
                  <th>{{ $t('common.customer') }}</th>
                  <th>{{ $t('employee.part_no') }}</th>
                  <th>{{ $t('part_list.description') }}</th>
                  <th>{{ $t('employee.hts_code') }}</th>
                  <th>{{ $t('common.status') }}</th>
                  <th>{{ $t('part_list.updated_by') }}</th>
                  <th>{{ $t('employee.updated_date') }}</th>
                  <th>{{ $t('part_list.sla') }}</th>
                  <th>{{ $t('common.actions') }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="part in pendingParts" :key="part.id">
                  <td>{{ part.customer }}</td>
                  <td class="bold">{{ part.partNo }}</td>
                  <td>{{ part.partDesc || '—' }}</td>
                  <td>{{ part.htsCode }}</td>
                  <td>
                    <span class="status-badge" :style="{ backgroundColor: statusToColor(part.status) }">
                      {{ $t(`status.${statusToI18nKey(part.status)}`) }}
                    </span>
                  </td>
                  <td>{{ part.updatedBy || '—' }}</td>
                  <td>{{ formatDate(part.updatedDate) }}</td>
                  <td>
                    <div v-if="part.slaStatus && getSLAColor(part.slaStatus) !== 'transparent'">
                      <Dot :dotColor="getSLAColor(part.slaStatus)" :title="$t(`sla.${part.slaStatus}`)" />
                    </div>
                  </td>
                  <td>
                    <button class="btn-action" @click="goToPartDetail(part.id)">
                      {{ $t('employee.review') }}
                    </button>
                  </td>
                </tr>
                <tr v-if="pendingParts.length === 0">
                  <td colspan="9" class="no-data">{{ $t('employee.no_pending') }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard-wrapper {
  background-color: var(--dashboard-bg);
  min-height: 100vh;
  width: 100%;
}

.dashboard-container {
  padding: 2.5rem;
  max-width: 1400px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

/* Header Styles */
.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid rgba(0,0,0,0.05);
}

.header-right {
  display: flex;
  align-items: center;
  gap: 2.5rem;
}

h1 {
  font-size: 2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.customer-selector-container {
  margin-bottom: 2rem;
  display: flex;
  justify-content: flex-start;
}

.customer-selector {
  display: flex;
  align-items: center;
  gap: 1rem;
  background: white;
  padding: 0.5rem 1rem;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

.customer-selector label {
  font-size: 0.9rem;
  color: #8898aa;
  font-weight: 500;
}

.app-select {
  padding: 0.6rem 1rem;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  background-color: white;
  color: var(--sidebar-color);
  font-size: 0.9rem;
  min-width: 200px;
  outline: none;
  transition: border-color 0.2s;
}

.app-select:focus {
  border-color: var(--primary-color);
}

.user-info {
  font-size: 0.95rem;
  color: var(--sidebar-color);
}

.username {
  margin-left: 0.3rem;
  color: var(--primary-color);
  font-weight: bold;
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
  background-color: var(--warning-color);
}

h3 {
  font-size: 1.25rem;
  color: var(--sidebar-color);
  margin: 0;
}

/* Status Summary Grid */
.status-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: 1.2rem;
  margin-bottom: 3rem;
}

.status-card {
  padding: 1.2rem;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
  background: white;
  transition: transform 0.2s;
  cursor: pointer;
}

.status-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0,0,0,0.08);
}

.status-header {
  display: flex;
  align-items: center;
  gap: 0.6rem;
  margin-bottom: 0.8rem;
}

.status-label {
  font-size: 0.8rem;
  color: #8898aa;
}

.status-value {
  font-size: 1.8rem;
  font-weight: bold;
  color: var(--sidebar-color);
}

/* Table Styles */
.table-container {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 2rem;
}

.card-shadow {
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
}

.app-table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.app-table th {
  background-color: #f8f9fe;
  padding: 1rem 1.5rem;
  font-size: 0.85rem;
  color: #8898aa;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.025em;
  border-bottom: 1px solid #e9ecef;
}

.app-table td {
  padding: 1rem 1.5rem;
  font-size: 0.9rem;
  color: #525f7f;
  border-bottom: 1px solid #e9ecef;
}

.app-table tr:last-child td {
  border-bottom: none;
}

.app-table tr:hover td {
  background-color: #f8f9fe;
}

.bold {
  font-weight: 600;
  color: var(--sidebar-color);
}

.no-data {
  text-align: center;
  padding: 3rem !important;
  color: #8898aa;
  font-style: italic;
}

.status-badge {
  display: inline-block;
  padding: 0.2rem 0.6rem;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 600;
  color: white;
  white-space: nowrap;
}

.btn-action {
  background-color: var(--primary-color);
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-size: 0.85rem;
  cursor: pointer;
  transition: background-color 0.2s;
}

.btn-action:hover {
  background-color: var(--primary-dark, #003366);
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
