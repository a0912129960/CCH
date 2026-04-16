<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { authService, UserRole } from '../../services/auth/auth';
// INTERNAL-AI-20260416: Import real API function and new types. Old mock imports preserved below.
// (INTERNAL-AI-20260416: 匯入真實 API 函式與新型別。舊的 mock 匯入保留如下。)
/* import { partService, type Part, PartStatus } from '../../services/part/part'; */
import { partService, PartStatus, getPartDetail, updatePart, type PartDetailResponse, type PartSavePayload } from '../../services/part/part';
import { useTabStore } from '../../stores/tabs';
import Card from '../../components/common/Card.vue';
import { ElMessage, ElMessageBox } from 'element-plus';

/**
 * Part No Detail View (零件編號詳細頁面)
 * Updated by AI - 2026-04-10:
 * 1. Dynamic breadcrumb based on referrer (Dashboard vs Parts List).
 * 2. Strictly limited actions based on role & latest requirements.
 *
 * INTERNAL-AI-20260416: Updated to call real GET /api/parts/{partId} and display backend data.
 * (INTERNAL-AI-20260416: 更新為呼叫真實 GET /api/parts/{partId} API 並顯示後端資料。)
 */

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const tabStore = useTabStore();

// partId from route is string; convert to number for the real API (路由 partId 為字串；轉換為數字供真實 API 使用)
const partIdStr = route.params.id as string;
const partId = Number(partIdStr);

// INTERNAL-AI-20260416: New ref for real backend data. Old mock ref preserved below.
// (INTERNAL-AI-20260416: 新增對應後端真實資料的 ref。舊 mock ref 保留如下。)
/* const part = ref<Part | null>(null); */
const partDetail = ref<PartDetailResponse | null>(null);
const loading = ref(true);
const saving = ref(false);
const actionRemark = ref('');

// INTERNAL-AI-20260416: Editable form state for PUT /api/parts/{partId}.
// (INTERNAL-AI-20260416: PUT /api/parts/{partId} 的可編輯表單狀態。)
const form = ref<PartSavePayload>({
  partNo: '',
  countryId: null,
  division: '',
  supplier: '',
  partDesc: '',
  htsCode: '',
  rate: 0,
  htsCode1: null, rate1: null,
  htsCode2: null, rate2: null,
  htsCode3: null, rate3: null,
  htsCode4: null, rate4: null,
  remark: ''
});

const userRole = computed(() => authService.state.role);
const isEmployee = computed(() => userRole.value === UserRole.DIMERCO || userRole.value === UserRole.DCB);
const isCustomer = computed(() => userRole.value === UserRole.CUSTOMER);

// Shorthand to access the "modified" data object (存取 modified 資料物件的簡便計算屬性)
const modified = computed(() => partDetail.value?.modified ?? null);

/**
 * Dynamic Breadcrumb Logic (動態麵包屑邏輯)
 */
const prevPageLabel = computed(() => {
  const backState = (window.history.state as any)?.back;
  if (backState && (backState.includes('/customer') || backState.includes('/employee'))) {
    return t('common.menu.dashboard');
  }
  return t('common.menu.parts');
});

onMounted(async () => {
  try {
    // INTERNAL-AI-20260416: Call real API instead of mock.
    // (INTERNAL-AI-20260416: 改呼叫真實 API，取代 mock 資料。)
    /* const data = await partService.getPartById(partId);
    if (data) {
      part.value = data;
      tabStore.updateTabTitle(route.path, data.partNo);
    } else {
      router.push('/parts');
    } */
    const data = await getPartDetail(partId);
    if (data) {
      partDetail.value = data;
      tabStore.updateTabTitle(route.path, data.modified.partNo);
      // Populate editable form from the modified snapshot (從 modified 快照初始化表單)
      const m = data.modified;
      form.value = {
        partNo: m.partNo,
        countryId: null,
        division: m.division,
        supplier: m.supplier,
        partDesc: m.partDesc,
        htsCode: m.htsCode,
        rate: m.rate,
        htsCode1: m.htsCode1 ?? null, rate1: m.rate1 ?? null,
        htsCode2: m.htsCode2 ?? null, rate2: m.rate2 ?? null,
        htsCode3: m.htsCode3 ?? null, rate3: m.rate3 ?? null,
        htsCode4: m.htsCode4 ?? null, rate4: m.rate4 ?? null,
        remark: m.remark
      };
    } else {
      // Part not found (404): redirect to parts list (零件不存在 404：跳轉回零件清單)
      ElMessage.error('Part not found. / 零件不存在。');
      router.push('/parts');
    }
  } finally {
    loading.value = false;
  }
});

// INTERNAL-AI-20260416: Save handler for Customer role — calls PUT /api/parts/{partId}.
// (INTERNAL-AI-20260416: Customer 角色的儲存處理函式，呼叫 PUT /api/parts/{partId}。)
const handleSave = async () => {
  saving.value = true;
  try {
    // Sanitize optional number fields: convert empty string / NaN to null before sending.
    // (送出前將空白或 NaN 的選填數字欄位轉為 null，避免後端 decimal? 解析失敗。)
    const toNullableNumber = (v: any) => (v === '' || v === null || Number.isNaN(v) ? null : Number(v));
    const toNullableString = (v: any) => (v === '' ? null : v || null);

    const payload: PartSavePayload = {
      ...form.value,
      rate:     toNullableNumber(form.value.rate) ?? 0,
      rate1:    toNullableNumber(form.value.rate1),
      rate2:    toNullableNumber(form.value.rate2),
      rate3:    toNullableNumber(form.value.rate3),
      rate4:    toNullableNumber(form.value.rate4),
      htsCode1: toNullableString(form.value.htsCode1),
      htsCode2: toNullableString(form.value.htsCode2),
      htsCode3: toNullableString(form.value.htsCode3),
      htsCode4: toNullableString(form.value.htsCode4),
    };

    await updatePart(partId, payload);
    ElMessage.success('Saved successfully. / 儲存成功。');
  } catch (err: any) {
    // Error message is already shown globally by the api interceptor (錯誤訊息已由 api 攔截器全域顯示)
  } finally {
    saving.value = false;
  }
};

// INTERNAL-AI-20260416: Action handlers below are preserved from old mock implementation.
// (INTERNAL-AI-20260416: 以下操作處理函式保留自舊 mock 實作，供日後接 API 時參考。)
/* const handleAccept = async () => {
  if (!part.value) return;
  try {
    await ElMessageBox.confirm(t('part_detail.accept_confirm'), t('common.confirm'), {
      confirmButtonClass: 'btn-confirm-orange',
      type: 'warning'
    });
    const success = await partService.updatePartStatus(partIdStr, PartStatus.ACTIVE, 'Accepted by Customer');
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
    const success = await partService.updatePartStatus(partIdStr, PartStatus.ACTIVE, 'Approved by Employee');
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
  const success = await partService.updatePartStatus(partIdStr, PartStatus.RETURNED, actionRemark.value);
  if (success) {
    ElMessage.success('Part returned to customer for correction.');
    router.back();
  }
}; */

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
    
    <!-- INTERNAL-AI-20260416: Updated to render real backend data via `modified` computed property. -->
    <!-- (INTERNAL-AI-20260416: 更新為使用 `modified` 計算屬性渲染後端真實資料。) -->
    <div v-else-if="partDetail && modified" class="page-container">
      <!-- Dynamic Breadcrumb Navigation (動態麵包屑導覽) -->
      <nav class="breadcrumb">
        <a href="#" @click.prevent="router.back()">{{ prevPageLabel }}</a>
        <span class="separator">/</span>
        <span class="current">{{ modified.partNo }}</span>
      </nav>

      <header class="page-header">
        <div class="header-left">
          <h1>{{ modified.partNo }}</h1>
        </div>
      </header>

      <div class="detail-content-grid">
        <div class="main-column">
          <!-- Basic Information Card (基本資料卡片) -->
          <!-- Customer: editable fields; Employee: read-only display -->
          <!-- (Customer 可編輯欄位；Employee 唯讀顯示) -->
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
                  <!-- Part No: read-only (非 Customer 或 Customer 均唯讀，PartNo 不可修改) -->
                  <div class="value code-box">{{ modified.partNo }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.country') }}</label>
                  <!-- Country: read-only (國家不可修改) -->
                  <div class="value code-box">{{ modified.country }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.division') }} <span v-if="isCustomer" class="required-mark">*</span></label>
                  <input v-if="isCustomer" v-model="form.division" class="field-input" />
                  <div v-else class="value code-box">{{ modified.division }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('common.supplier') }} <span v-if="isCustomer" class="required-mark">*</span></label>
                  <input v-if="isCustomer" v-model="form.supplier" class="field-input" />
                  <div v-else class="value code-box">{{ modified.supplier }}</div>
                </div>
              </div>
              <div class="info-cell full-width mt-6">
                <label>{{ $t('part_create.description') }} <span v-if="isCustomer" class="required-mark">*</span></label>
                <textarea v-if="isCustomer" v-model="form.partDesc" class="field-textarea" rows="3"></textarea>
                <p v-else class="description-text">{{ modified.partDesc || 'No description provided.' }}</p>
              </div>
            </div>
          </Card>

          <!-- HTS Code & Rate Card (HTS 代碼與稅率卡片) -->
          <Card class="info-card section-margin">
            <template #header>
              <div class="card-header">
                <div class="decorator"></div>
                <h3>{{ $t('part_detail.hts_and_rate') }}</h3>
              </div>
            </template>
            <div class="card-body-padding">
              <div class="info-grid">
                <div class="info-cell">
                  <label>{{ $t('customer.hts_code') }} <span v-if="isCustomer" class="required-mark">*</span></label>
                  <input v-if="isCustomer" v-model="form.htsCode" class="field-input code-font" placeholder="XXXX.XX.XXXX" />
                  <div v-else class="value code-box">{{ modified.htsCode }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.rate') }}</label>
                  <input v-if="isCustomer" v-model.number="form.rate" type="number" class="field-input" />
                  <div v-else class="value code-box">{{ modified.rate }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('customer.hts_code') }} 1</label>
                  <input v-if="isCustomer" v-model="form.htsCode1" class="field-input code-font" placeholder="XXXX.XX.XXXX" />
                  <div v-else class="value code-box">{{ modified.htsCode1 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.rate') }} 1</label>
                  <input v-if="isCustomer" v-model.number="form.rate1" type="number" class="field-input" />
                  <div v-else class="value code-box">{{ modified.rate1 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('customer.hts_code') }} 2</label>
                  <input v-if="isCustomer" v-model="form.htsCode2" class="field-input code-font" placeholder="XXXX.XX.XXXX" />
                  <div v-else class="value code-box">{{ modified.htsCode2 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.rate') }} 2</label>
                  <input v-if="isCustomer" v-model.number="form.rate2" type="number" class="field-input" />
                  <div v-else class="value code-box">{{ modified.rate2 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('customer.hts_code') }} 3</label>
                  <input v-if="isCustomer" v-model="form.htsCode3" class="field-input code-font" placeholder="XXXX.XX.XXXX" />
                  <div v-else class="value code-box">{{ modified.htsCode3 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.rate') }} 3</label>
                  <input v-if="isCustomer" v-model.number="form.rate3" type="number" class="field-input" />
                  <div v-else class="value code-box">{{ modified.rate3 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('customer.hts_code') }} 4</label>
                  <input v-if="isCustomer" v-model="form.htsCode4" class="field-input code-font" placeholder="XXXX.XX.XXXX" />
                  <div v-else class="value code-box">{{ modified.htsCode4 ?? '-' }}</div>
                </div>
                <div class="info-cell">
                  <label>{{ $t('part_detail.rate') }} 4</label>
                  <input v-if="isCustomer" v-model.number="form.rate4" type="number" class="field-input" />
                  <div v-else class="value code-box">{{ modified.rate4 ?? '-' }}</div>
                </div>
              </div>
              <!-- Remark (備註) -->
              <div class="info-cell full-width mt-6">
                <label>{{ $t('part_detail.remark') }}</label>
                <textarea v-if="isCustomer" v-model="form.remark" class="field-textarea" rows="2"></textarea>
                <p v-else class="description-text">{{ modified.remark || '-' }}</p>
              </div>
            </div>
          </Card>

          <!-- Save Button — Customer only (儲存按鈕，僅 Customer 顯示) -->
          <!-- INTERNAL-AI-20260416 -->
          <div v-if="isCustomer" class="save-row">
            <button class="btn-cch btn-save" :disabled="saving" @click="handleSave">
              {{ saving ? 'Saving...' : $t('common.save') }}
            </button>
          </div>

          <!-- Last Updated Info (最後更新資訊) -->
          <div class="meta-row">
            <span class="meta-label">{{ $t('part_detail.updated_by') }}：</span>
            <span class="meta-value">{{ modified.updatedBy }}</span>
            <span class="meta-sep">·</span>
            <span class="meta-label">{{ $t('part_detail.updated_date') }}：</span>
            <span class="meta-value">{{ modified.updatedDate }}</span>
          </div>
        </div>

        <!-- Side column preserved for future timeline/history integration (側欄保留供未來時間軸/歷程整合使用) -->
        <div class="side-column">
          <!-- Placeholder: history panel will be wired to GET /api/parts/{partId}/milestones (待串接里程碑 API) -->
          <Card class="timeline-card">
            <template #header>
              <div class="card-header">
                <div class="decorator secondary"></div>
                <h3>{{ $t('part_detail.history') }}</h3>
              </div>
            </template>
            <div class="card-body-padding">
              <p class="text-muted">Milestone history coming soon. / 里程碑歷程即將上線。</p>
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
}

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

.detail-content-grid {
  display: grid;
  grid-template-columns: 1fr 420px;
  gap: 2.5rem;
}

.section-margin { margin-bottom: 2.5rem; }
.card-body-padding { padding: 1.5rem 2rem; }
.mt-6 { margin-top: 1.5rem; }
.mb-4 { margin-bottom: 1rem; }

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
  display: block;
  color: var(--primary-color);
  font-weight: 700;
  border: 1px solid #e9ecef;
  width: 100%;
  box-sizing: border-box;
}

.description-text {
  color: #525f7f;
  line-height: 1.7;
  background: #fcfcfd;
  padding: 1.2rem;
  border-radius: 10px;
  border: 1px solid #f1f3f5;
}

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

/* INTERNAL-AI-20260416: Editable field styles matching code-box visual style */
/* (INTERNAL-AI-20260416: 可編輯欄位樣式，與 code-box 視覺風格一致) */
.field-input {
  display: block;
  width: 100%;
  box-sizing: border-box;
  padding: 8px 14px;
  border-radius: 8px;
  border: 1px solid #d0d7de;
  background: #fff;
  font-size: 1.1rem;
  color: #32325d;
  font-family: inherit;
  transition: border-color 0.2s;
}

.field-input.code-font {
  font-family: 'Courier New', monospace;
  color: var(--primary-color);
  font-weight: 700;
}

.field-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(64, 158, 255, 0.12);
}

.field-textarea {
  display: block;
  width: 100%;
  box-sizing: border-box;
  padding: 10px 14px;
  border-radius: 8px;
  border: 1px solid #d0d7de;
  background: #fff;
  font-size: 1rem;
  color: #32325d;
  font-family: inherit;
  resize: vertical;
  transition: border-color 0.2s;
}

.field-textarea:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(64, 158, 255, 0.12);
}

.required-mark {
  color: #f56c6c;
  margin-left: 2px;
}

.save-row {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 2.5rem;
}

.btn-save {
  background-color: var(--primary-color);
  color: white;
  border: none;
  min-width: 140px;
}

.btn-save:hover:not(:disabled) {
  background-color: #337ecc;
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(64, 158, 255, 0.3);
}

.btn-save:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* INTERNAL-AI-20260416: Styles for meta update info row (INTERNAL-AI-20260416: 最後更新資訊列樣式) */
.meta-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.85rem;
  color: #8898aa;
  margin-bottom: 1.5rem;
}

.meta-label { font-weight: 600; }
.meta-value { color: #525f7f; }
.meta-sep { color: #dee2e6; }

.text-muted { color: #8898aa; font-size: 0.9rem; }

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
