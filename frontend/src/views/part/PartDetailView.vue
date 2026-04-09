<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { partService, type Part, PartStatus } from '../../services/part/part';
import Card from '../../components/common/Card.vue';
import Dot from '../../components/common/Dot.vue';

/**
 * Part No Detail View (零件編號詳細頁面)
 * BR-14: History Timeline | BR-29: Returned Feedback | BR-30: Actions
 */

const route = useRoute();
const router = useRouter();
const partId = route.params.id as string;

const part = ref<Part | null>(null);
const loading = ref(true);
const resubmitRemark = ref('');

onMounted(async () => {
  try {
    const data = await partService.getPartById(partId);
    if (data) {
      part.value = data;
    } else {
      router.push('/parts');
    }
  } finally {
    loading.value = false;
  }
});

const handleAccept = async () => {
  if (!part.value) return;
  if (confirm(router.app.config.globalProperties.$t('part_detail.accept_confirm'))) {
    const success = await partService.updatePartStatus(partId, PartStatus.ACTIVE, 'Accepted by Customer');
    if (success) {
      router.push('/parts');
    }
  }
};

const handleResubmit = async () => {
  if (!part.value || !resubmitRemark.value) return;
  const success = await partService.updatePartStatus(partId, PartStatus.PENDING_REVIEW, resubmitRemark.value);
  if (success) {
    router.push('/parts');
  }
};

const getStatusColor = (status: PartStatus) => {
  const colors: Record<string, string> = {
    [PartStatus.UNKNOWN]: '#909399',
    [PartStatus.PENDING_CUSTOMER]: '#E6A23C',
    [PartStatus.PENDING_REVIEW]: '#409EFF',
    [PartStatus.RETURNED]: '#F56C6C',
    [PartStatus.ACTIVE]: '#67C23A',
    [PartStatus.FLAGGED]: '#E6A23C',
    [PartStatus.SUPERSEDED]: '#909399'
  };
  return colors[status] || '#909399';
};
</script>

<template>
  <div class="page-wrapper">
    <div v-if="loading" class="loading-overlay">Loading...</div>
    
    <div v-else-if="part" class="page-container">
      <header class="page-header">
        <div class="header-left">
          <button class="back-link" @click="router.back()">← {{ $t('common.back') }}</button>
          <h1>{{ part.partNo }}</h1>
          <span class="status-pill" :style="{ backgroundColor: getStatusColor(part.status) }">
            {{ $t('status.' + part.status.toLowerCase()) }}
          </span>
        </div>
        <div class="header-actions" v-if="part.status === PartStatus.RETURNED">
          <button class="btn btn-secondary" @click="handleAccept">{{ $t('common.accept') }}</button>
        </div>
      </header>

      <div class="detail-grid">
        <!-- 1. Left: Info & Action -->
        <div class="main-info">
          <Card class="info-card">
            <template #header>
              <div class="card-header">
                <div class="decorator"></div>
                <h3>{{ $t('part_detail.basic_info') }}</h3>
              </div>
            </template>
            <div class="info-list">
              <div class="info-item">
                <label>{{ $t('customer.hts_code') }}</label>
                <div class="value code-box">{{ part.htsCode }}</div>
              </div>
              <div class="info-item">
                <label>{{ $t('common.supplier') }}</label>
                <div class="value">{{ part.supplier }}</div>
              </div>
              <div class="info-item">
                <label>{{ $t('common.last_updated') }}</label>
                <div class="value">{{ part.lastUpdated }}</div>
              </div>
            </div>
          </Card>

          <!-- BR-29: Returned Feedback Section -->
          <Card v-if="part.status === PartStatus.RETURNED" class="feedback-card">
            <div class="feedback-header">
              <span class="alert-icon">⚠️</span>
              <h3>{{ $t('part_detail.dimerco_feedback') }}</h3>
            </div>
            <div class="feedback-content">
              <div class="reason-group">
                <label>{{ $t('part_detail.reason') }}</label>
                <p>{{ part.dimercoRemark }}</p>
              </div>
              <div class="suggest-group" v-if="part.replacementCode">
                <label>{{ $t('part_detail.suggested_code') }}</label>
                <div class="suggested-code">{{ part.replacementCode }}</div>
              </div>
            </div>
          </Card>

          <!-- BR-30: Action Input -->
          <Card v-if="part.status === PartStatus.RETURNED" class="action-card">
            <template #header>
              <h3>{{ $t('part_detail.resubmit_title') }}</h3>
            </template>
            <textarea 
              v-model="resubmitRemark" 
              class="remark-input" 
              :placeholder="$t('part_detail.resubmit_placeholder')"
            ></textarea>
            <div class="action-footer">
              <button 
                class="btn btn-primary" 
                :disabled="!resubmitRemark"
                @click="handleResubmit"
              >
                {{ $t('common.resubmit') }}
              </button>
            </div>
          </Card>
        </div>

        <!-- 2. Right: Timeline (BR-14) -->
        <div class="timeline-container">
          <Card class="timeline-card">
            <template #header>
              <div class="card-header">
                <div class="decorator secondary"></div>
                <h3>{{ $t('part_detail.history') }}</h3>
              </div>
            </template>
            <div class="timeline">
              <div v-for="item in part.history" :key="item.id" class="timeline-item">
                <div class="timeline-dot" :style="{ backgroundColor: getStatusColor(item.status) }"></div>
                <div class="timeline-content">
                  <div class="timeline-header">
                    <span class="time-status">{{ $t('status.' + item.status.toLowerCase()) }}</span>
                    <span class="time-date">{{ item.updatedAt }}</span>
                  </div>
                  <div class="time-user">By: {{ item.updatedBy }}</div>
                  <p v-if="item.remark" class="time-remark">"{{ item.remark }}"</p>
                </div>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper {
  background-color: var(--dashboard-bg);
  min-height: 100vh;
}

.page-container {
  padding: 2.5rem;
  max-width: 1400px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

/* Header */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.back-link {
  background: none;
  border: none;
  color: var(--primary-color);
  cursor: pointer;
  font-weight: 500;
}

h1 {
  font-size: 2.2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.status-pill {
  color: white;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
}

/* Layout Grid */
.detail-grid {
  display: grid;
  grid-template-columns: 1fr 400px;
  gap: 2rem;
}

.main-info {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

/* Cards */
.card-header {
  display: flex;
  align-items: center;
  gap: 0.8rem;
}

.decorator {
  width: 4px;
  height: 20px;
  background-color: var(--primary-color);
  border-radius: 2px;
}

.decorator.secondary {
  background-color: var(--warning-color);
}

h3 {
  font-size: 1.1rem;
  color: var(--sidebar-color);
  margin: 0;
}

/* Info List */
.info-list {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.info-item label {
  display: block;
  font-size: 0.8rem;
  color: #8898aa;
  margin-bottom: 0.4rem;
}

.value {
  font-size: 1.1rem;
  color: var(--sidebar-color);
}

.code-box {
  background: #f8f9fe;
  padding: 8px 12px;
  border-radius: 6px;
  font-family: monospace;
  display: inline-block;
  color: var(--primary-color);
  font-weight: bold;
}

/* Feedback Section */
.feedback-card {
  background-color: #fff5f5;
  border: 1px solid #feb2b2;
}

.feedback-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.feedback-header h3 {
  color: #c53030;
}

.reason-group p {
  background: white;
  padding: 1rem;
  border-radius: 8px;
  border-left: 4px solid #f56c6c;
  color: #4a5568;
}

.suggest-group {
  margin-top: 1rem;
}

.suggested-code {
  font-size: 1.2rem;
  color: #2f855a;
  font-weight: bold;
}

/* Action Section */
.remark-input {
  width: 100%;
  height: 120px;
  padding: 1rem;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  resize: none;
  font-family: inherit;
  margin-bottom: 1rem;
}

.action-footer {
  display: flex;
  justify-content: flex-end;
}

/* Timeline */
.timeline {
  position: relative;
  padding-left: 20px;
}

.timeline::before {
  content: '';
  position: absolute;
  left: 4px;
  top: 0;
  bottom: 0;
  width: 2px;
  background: #e9ecef;
}

.timeline-item {
  position: relative;
  padding-bottom: 2rem;
}

.timeline-dot {
  position: absolute;
  left: -20px;
  top: 4px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  border: 2px solid white;
  z-index: 1;
}

.timeline-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 0.3rem;
}

.time-status {
  font-weight: bold;
  color: var(--sidebar-color);
}

.time-date {
  font-size: 0.8rem;
  color: #adb5bd;
}

.time-user {
  font-size: 0.85rem;
  color: #8898aa;
}

.time-remark {
  margin-top: 0.5rem;
  font-size: 0.9rem;
  font-style: italic;
  color: #4a5568;
  background: #f8f9fe;
  padding: 0.5rem;
  border-radius: 4px;
}

/* Buttons */
.btn {
  padding: 8px 20px;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.btn-primary {
  background-color: var(--primary-color);
  color: white;
}

.btn-primary:disabled {
  background-color: #cbd5e0;
  cursor: not-allowed;
}

.btn-secondary {
  background-color: #2f855a;
  color: white;
}

.loading-overlay {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}
</style>
