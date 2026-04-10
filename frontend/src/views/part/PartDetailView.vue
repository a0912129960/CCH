<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { authService, UserRole } from '../../services/auth/auth';
import { partService, type Part, PartStatus } from '../../services/part/part';
import Card from '../../components/common/Card.vue';
import Button from '../../components/common/Button.vue';
import { ElMessage, ElMessageBox } from 'element-plus';

/**
 * Part No Detail View (零件編號詳細頁面)
 * Updated by AI - 2026-04-10: 
 * 1. Dynamic breadcrumb based on referrer (Dashboard vs Parts List).
 * 2. Strictly limited actions based on role & latest requirements.
 */

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const partId = route.params.id as string;

const part = ref<Part | null>(null);
const loading = ref(true);
const actionRemark = ref('');

const userRole = computed(() => authService.state.role);
const isEmployee = computed(() => userRole.value === UserRole.EMPLOYEE);
const isCustomer = computed(() => userRole.value === UserRole.CUSTOMER);

/**
 * Dynamic Breadcrumb Logic (動態麵包屑邏輯)
 */
const prevPageLabel = computed(() => {
  const backState = (window.history.state as any)?.back;
  // If coming from dashboard routes (儀表板路徑入口)
  if (backState && (backState.includes('/customer') || backState.includes('/employee'))) {
    return t('common.menu.dashboard');
  }
  return t('common.menu.parts');
});

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
  try {
    await ElMessageBox.confirm(t('part_detail.accept_confirm'), t('common.confirm'), {
      confirmButtonClass: 'btn-confirm-orange',
      type: 'warning'
    });
    const success = await partService.updatePartStatus(partId, PartStatus.ACTIVE, 'Accepted by Customer');
    if (success) {
      ElMessage.success('Part accepted successfully.');
      router.back();
    }
  } catch { }
};

const handleApprove = async () => {
  if (!part.value) return;
  try {
    await ElMessageBox.confirm('Are you sure you want to approve this part classification?', 'Approve Part', {
      confirmButtonClass: 'btn-confirm-orange',
      type: 'warning'
    });
    const success = await partService.updatePartStatus(partId, PartStatus.ACTIVE, 'Approved by Employee');
    if (success) {
      ElMessage.success('Part approved successfully.');
      router.back();
    }
  } catch { }
};

const handleReturn = async () => {
  if (!part.value || !actionRemark.value) {
    ElMessage.warning(t('part_detail.return_placeholder'));
    return;
  }
  const success = await partService.updatePartStatus(partId, PartStatus.RETURNED, actionRemark.value);
  if (success) {
    ElMessage.success('Part returned to customer for correction.');
    router.back();
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
    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
    </div>
    
    <div v-else-if="part" class="page-container">
      <!-- Dynamic Breadcrumb Navigation -->
      <nav class="breadcrumb">
        <a href="#" @click.prevent="router.back()">{{ prevPageLabel }}</a>
        <span class="separator">/</span>
        <span class="current">{{ part.partNo }}</span>
      </nav>

      <header class="page-header">
        <div class="header-left">
          <h1>{{ part.partNo }}</h1>
          <span class="status-pill" :style="{ backgroundColor: getStatusColor(part.status) }">
            {{ $t('status.' + part.status.toLowerCase()) }}
          </span>
        </div>
      </header>

      <div class="detail-content-grid">
        <!-- Left Column: Information -->
        <div class="main-column">
          <!-- Basic Info Card -->
          <Card class="info-card section-margin">
            <template #header>
              <div class="card-header">
                <div class="decorator"></div>
                <h3>{{ $t('part_detail.basic_info') }}</h3>
              </div>
            </template>
            <div class="card-body-padding">
              <div class="info-grid">
                <div class="info-cell">
                  <label>{{ $t('customer.part_no') }}</label>
                  <div class="value bold">{{ part.partNo }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('customer.hts_code') }}</label>
                  <div class="value code-box">{{ part.htsCode }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('common.supplier') }}</label>
                  <div class="value">{{ part.supplier }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('common.last_updated') }}</label>
                  <div class="value text-muted">{{ part.lastUpdated }}</div>
                </div>
              </div>
              <div class="info-cell full-width mt-6">
                <label>{{ $t('part_create.description') }}</label>
                <p class="description-text">{{ part.description || 'No description provided.' }}</p>
              </div>
            </div>
          </Card>

          <!-- Feedback Section (Visible for both if RETURNED) -->
          <Card v-if="part.status === PartStatus.RETURNED" class="feedback-card section-margin">
            <template #header>
              <div class="feedback-header">
                <span class="alert-icon">⚠️</span>
                <h3>{{ $t('part_detail.dimerco_feedback') }}</h3>
              </div>
            </template>
            <div class="card-body-padding">
              <div class="reason-group">
                <label>{{ $t('part_detail.reason') }}</label>
                <p class="reason-text">{{ part.dimercoRemark || 'N/A' }}</p>
              </div>
              <div class="suggest-group mt-6" v-if="part.replacementCode">
                <label>{{ $t('part_detail.suggested_code') }}</label>
                <div class="suggested-code">{{ part.replacementCode }}</div>
              </div>
            </div>
          </Card>

          <!-- Action Section (Consolidated) -->
          <Card 
            v-if="(isCustomer && part.status === PartStatus.RETURNED) || (isEmployee && part.status === PartStatus.PENDING_REVIEW)" 
            class="action-card section-margin"
          >
            <template #header>
              <div class="card-header">
                <div class="decorator orange-decorator"></div>
                <h3>{{ isEmployee ? $t('part_detail.review_action') : $t('common.actions') }}</h3>
              </div>
            </template>
            
            <div class="card-body-padding action-body">
              <!-- Remark only needed for Employee Return -->
              <div v-if="isEmployee" class="mb-4">
                <label class="remark-label">{{ $t('part_detail.return_reason_label') }}</label>
                <textarea 
                  v-model="actionRemark" 
                  class="remark-textarea" 
                  :placeholder="$t('part_detail.return_placeholder')"
                ></textarea>
              </div>
              
              <div class="action-buttons-group">
                <!-- Customer: Only Accept for RETURNED parts -->
                <button 
                  v-if="isCustomer" 
                  class="btn-cch btn-accept" 
                  @click="handleAccept"
                >
                  {{ $t('common.accept') }}
                </button>

                <!-- Employee: Accept and Return for PENDING_REVIEW parts -->
                <template v-if="isEmployee">
                  <button 
                    class="btn-cch btn-accept" 
                    @click="handleApprove"
                  >
                    {{ $t('common.accept') }}
                  </button>
                  <button 
                    class="btn-cch btn-return"
                    :disabled="!actionRemark"
                    @click="handleReturn"
                  >
                    {{ $t('part_detail.btn_return_customer') }}
                  </button>
                </template>
              </div>
            </div>
          </Card>
        </div>

        <!-- Right Column: Timeline -->
        <div class="side-column">
          <Card class="timeline-card">
            <template #header>
              <div class="card-header">
                <div class="decorator secondary"></div>
                <h3>{{ $t('part_detail.history') }}</h3>
              </div>
            </template>
            <div class="card-body-padding">
              <div class="timeline-wrapper">
                <div v-for="(item, index) in part.history" :key="item.id" class="timeline-item">
                  <div class="timeline-line" v-if="index !== (part.history?.length || 0) - 1"></div>
                  <div class="timeline-dot" :style="{ borderColor: getStatusColor(item.status) }"></div>
                  <div class="timeline-content">
                    <div class="time-header">
                      <span class="status-name" :style="{ color: getStatusColor(item.status) }">
                        {{ $t('status.' + item.status.toLowerCase()) }}
                      </span>
                      <span class="time-stamp">{{ item.updatedAt }}</span>
                    </div>
                    <div class="user-name">By: {{ item.updatedBy }}</div>
                    <p v-if="item.remark" class="remark-quote">"{{ item.remark }}"</p>
                  </div>
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
  background-color: #f4f7fc;
  min-height: 100vh;
  padding: 2rem 0;
}

.page-container {
  padding: 0 3rem;
  max-width: 1400px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

/* Breadcrumb */
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 2rem;
  font-size: 0.9rem;
}

.breadcrumb a {
  color: var(--primary-color);
  text-decoration: none;
}

.breadcrumb .separator { color: #adb5bd; }
.breadcrumb .current { color: #6c757d; }

/* Header */
.page-header {
  margin-bottom: 2.5rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

h1 {
  font-size: 2.25rem;
  color: var(--sidebar-color);
  margin: 0;
  letter-spacing: -0.02em;
}

.status-pill {
  color: white;
  padding: 6px 16px;
  border-radius: 30px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

/* Grid Layout */
.detail-content-grid {
  display: grid;
  grid-template-columns: 1fr 420px;
  gap: 2.5rem;
}

.section-margin { margin-bottom: 2.5rem; }
.card-body-padding { padding: 1.5rem 2rem; }
.mt-6 { margin-top: 1.5rem; }
.mb-4 { margin-bottom: 1rem; }

/* Card Styling */
.card-header {
  display: flex;
  align-items: center;
  gap: 0.8rem;
  padding: 1.2rem 2rem;
  border-bottom: 1px solid #f1f3f5;
}

.decorator {
  width: 4px;
  height: 20px;
  background-color: var(--primary-color);
  border-radius: 2px;
}

.decorator.secondary { background-color: var(--warning-color); }
.decorator.orange-decorator { background-color: #ff9800; }

h3 {
  font-size: 1.1rem;
  color: var(--sidebar-color);
  margin: 0;
  font-weight: 600;
}

/* Info Grid */
.info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 2rem;
}

.info-cell label {
  display: block;
  font-size: 0.85rem;
  color: #8898aa;
  margin-bottom: 0.6rem;
  font-weight: 500;
}

.value {
  font-size: 1.1rem;
  color: #32325d;
}

.value.bold { font-weight: 700; }

.code-box {
  background: #f8f9fe;
  padding: 8px 14px;
  border-radius: 8px;
  font-family: 'Courier New', monospace;
  display: inline-block;
  color: var(--primary-color);
  font-weight: 700;
  border: 1px solid #e9ecef;
}

.description-text {
  color: #525f7f;
  line-height: 1.7;
  background: #fcfcfd;
  padding: 1.2rem;
  border-radius: 10px;
  border: 1px solid #f1f3f5;
}

/* Feedback Card */
.feedback-card {
  background-color: #fff9f9;
  border: 1px solid #ffdada;
}

.feedback-header {
  display: flex;
  align-items: center;
  gap: 0.6rem;
}

.feedback-header h3 { color: #d9534f; }

.reason-text {
  background: white;
  padding: 1.2rem;
  border-radius: 10px;
  border-left: 5px solid #f56c6c;
  color: #4a5568;
  box-shadow: 0 2px 6px rgba(0,0,0,0.02);
}

.suggested-code {
  font-size: 1.3rem;
  color: #2dce89;
  font-weight: 800;
  margin-top: 0.5rem;
}

/* Action Section */
.action-card {
  border: 1px solid #ffe8cc;
}

.action-body {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.remark-label {
  font-size: 0.9rem;
  color: #525f7f;
  font-weight: 600;
}

.remark-textarea {
  width: 100%;
  height: 120px;
  padding: 1.2rem;
  border: 1px solid #e0e0e0;
  border-radius: 12px;
  resize: none;
  font-family: inherit;
  transition: all 0.2s;
  background: #fff;
}

.remark-textarea:focus {
  border-color: #ff9800;
  outline: none;
  box-shadow: 0 0 0 4px rgba(255, 152, 0, 0.1);
}

.action-buttons-group {
  display: flex;
  gap: 1.2rem;
  justify-content: flex-start;
  margin-top: 0.5rem;
}

/* CCH Standard Buttons */
.btn-cch {
  padding: 12px 28px;
  border-radius: 10px;
  font-weight: 700;
  font-size: 0.95rem;
  cursor: pointer;
  transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 140px;
}

.btn-accept {
  background-color: #ff9800;
  color: white;
  border: none;
}

.btn-accept:hover {
  background-color: #f57c00;
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(255, 152, 0, 0.3);
}

.btn-return {
  background-color: white;
  color: #ff9800;
  border: 2.5px solid #ff9800;
}

.btn-return:hover:not(:disabled) {
  background-color: rgba(255, 152, 0, 0.05);
  transform: translateY(-2px);
  box-shadow: 0 4px 10px rgba(255, 152, 0, 0.1);
}

.btn-return:disabled {
  border-color: #e0e0e0;
  color: #adb5bd;
  cursor: not-allowed;
}

/* Timeline */
.timeline-wrapper {
  padding: 0.5rem 0;
}

.timeline-item {
  position: relative;
  padding-left: 35px;
  padding-bottom: 2.5rem;
}

.timeline-line {
  position: absolute;
  left: 4px;
  top: 24px;
  bottom: 0;
  width: 2px;
  background: #e9ecef;
}

.timeline-dot {
  position: absolute;
  left: 0;
  top: 8px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: white;
  border: 2.5px solid;
  z-index: 2;
}

.time-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.status-name {
  font-weight: 700;
  font-size: 0.9rem;
}

.time-stamp {
  font-size: 0.75rem;
  color: #adb5bd;
}

.user-name {
  font-size: 0.85rem;
  color: #8898aa;
  margin-bottom: 0.6rem;
}

.remark-quote {
  font-size: 0.9rem;
  color: #525f7f;
  background: #f8f9fe;
  padding: 1rem;
  border-radius: 8px;
  border-left: 4px solid #dee2e6;
  margin: 0;
  line-height: 1.5;
}

/* Utils */
.loading-overlay {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.spinner {
  width: 45px;
  height: 45px;
  border: 4px solid rgba(0,0,0,0.05);
  border-top-color: #ff9800;
  border-radius: 50%;
  animation: spin 1s cubic-bezier(0.4, 0, 0.2, 1) infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }
</style>
