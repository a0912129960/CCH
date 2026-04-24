<script setup lang="ts">
import { authService, UserRole } from '@src/services/auth/auth';
// INTERNAL-AI-20260416: Import real API functions and types. Old mock imports preserved below.
// (INTERNAL-AI-20260416: 匯入真實 API 函式與型別。舊的 mock 匯入保留如下。)
/* import { partService, type Part, PartStatus } from '../../services/part/part'; */
import {
  /* partService, PartStatus, */
  getPartDetail, updatePart, submitPart, getMilestones, getHistory, acceptPart, returnPart, inactivatePart, sendToCustomerReview,
  getHtsRecommendation,
  statusToI18nKey,
  type PartDetailResponse, type PartDetailFields, type PartSavePayload, type Milestone
} from '@src/services/part/part';
// INTERNAL-AI-20260420: commonService import removed (supplier is now free-text, no dropdown needed).
// (INTERNAL-AI-20260420: 移除 commonService 匯入，供應商改為自由輸入文字，不需下拉清單。)
/* import { commonService, type SupplierOption } from '@src/services/common/common'; */
import { useTabStore } from '@src/stores/tabs';
/*
import { ElMessage, ElMessageBox } from 'element-plus';
*/

/**
 * Part No Detail View (零件編號詳細頁面)
 * INTERNAL-AI-20260416: Restructured to table layout matching the design specification.
 * (INTERNAL-AI-20260416: 依設計規範改為表格佈局。)
 * Update by Gemini AI on 2026-04-18: Global import cleanup and path alias refactor.
 */

const route = useRoute();
const router = useRouter();
const { t } = useI18n();
const tabStore = useTabStore();

const partIdStr = route.params.id as string;
const partId = Number(partIdStr);

const partDetail = ref<PartDetailResponse | null>(null);
const milestones = ref<Milestone[]>([]);
const loading = ref(true);
const saving = ref(false);
const submitting = ref(false);
const inactivating = ref(false);
const showHistoryPanel = ref(false);
const historyRows = ref<PartDetailFields[]>([]);

const openHistoryPanel = async () => {
  if (historyRows.value.length === 0) {
    historyRows.value = await getHistory(partId);
  }
  showHistoryPanel.value = true;
};

const DATA_FIELDS: (keyof PartDetailFields)[] = [
  'partNo', 'country', 'division', 'supplier', 'partDesc',
  'htsCode', 'rate', 'htsCode1', 'rate1', 'htsCode2', 'rate2',
  'htsCode3', 'rate3', 'htsCode4', 'rate4', 'remark'
];

const isSameData = (a: PartDetailFields, b: PartDetailFields): boolean =>
  DATA_FIELDS.every(f => (a[f] ?? null) === (b[f] ?? null));

// Filter out snapshots where only status changed (data fields identical to the older snapshot).
// History is newest-first; compare each row to index+1 (the snapshot it evolved from).
const filteredHistoryRows = computed(() =>
  historyRows.value.filter((row, i) => {
    const older = historyRows.value[i + 1];
    if (!older) return true; // oldest snapshot always shown
    return !isSameData(row, older);
  })
);

// INTERNAL-AI-20260420: Supplier is now free-text; dropdown and supplierOptions removed.
// (INTERNAL-AI-20260420: 供應商改為自由輸入文字，已移除下拉選單與 supplierOptions。)
/* const supplierOptions = ref<SupplierOption[]>([]); */

// HTS Code format validation (HTS Code 格式驗證)
const HTS_PATTERN = /^\d{4}\.\d{2}\.\d{4}$/;
const htsError = ref('');
const htsCode1Error = ref('');
const htsCode2Error = ref('');
const htsCode3Error = ref('');
const htsCode4Error = ref('');

const validateHts = (val: string | null | undefined, errRef: typeof htsError): boolean => {
  if (!val) { errRef.value = ''; return true; }
  if (!HTS_PATTERN.test(val)) {
    errRef.value = 'Format must be XXXX.XX.XXXX';
    return false;
  }
  errRef.value = '';
  return true;
};

// INTERNAL-AI-20260421: Auto-format HTS Code: strip non-digits, insert dots at XXXX.XX.XXXX positions.
// (INTERNAL-AI-20260421: HTS Code 自動格式化：濾除非數字字元，於對應位置自動插入點。)
type HtsField = 'htsCode' | 'htsCode1' | 'htsCode2' | 'htsCode3' | 'htsCode4';

const formatHtsCode = (raw: string): string => {
  const digits = raw.replace(/\D/g, '').slice(0, 10);
  if (digits.length <= 4) return digits;
  if (digits.length <= 6) return `${digits.slice(0, 4)}.${digits.slice(4)}`;
  return `${digits.slice(0, 4)}.${digits.slice(4, 6)}.${digits.slice(6)}`;
};

// Lookup map keeps refs in script scope — avoids passing Ref objects as template arguments.
const htsErrMap = {
  htsCode: htsError,
  htsCode1: htsCode1Error,
  htsCode2: htsCode2Error,
  htsCode3: htsCode3Error,
  htsCode4: htsCode4Error,
};

const validateHtsField = (field: HtsField, val: string | null | undefined): boolean =>
  validateHts(val, htsErrMap[field]);

const handleHtsInput = (field: HtsField, event: Event) => {
  const input = event.target as HTMLInputElement;
  const formatted = formatHtsCode(input.value);
  (form.value as any)[field] = formatted;
  input.value = formatted; // sync DOM to show formatted value immediately
  validateHts(formatted, htsErrMap[field]);
  // US HTS Code changed — reset existence check until user re-validates
  if (field === 'htsCode') htsExists.value = null;
};

// Tracks the stored/current USITC existence check result for the US HTS Code.
// null = not checked, true = found, false = not found.
const htsExists = ref<boolean | null>(null);

// HTS recommendation lookup — fires on blur of US HTS Code only.
// If fallback_used=true and general has a value, auto-fill General Duty Rate.
const htsLookupLoading = ref(false);

const handleHtsCodeBlur = async () => {
  validateHtsField('htsCode', form.value.htsCode);
  if (htsError.value || !form.value.htsCode) return;

  htsLookupLoading.value = true;
  try {
    const result = await getHtsRecommendation(form.value.htsCode);
    if (!result) return; // API error — silently skip

    // Initial code returned [] (not found in USITC at all)
    if (!result.fallback_used && result.message === 'No recommendation data') {
      htsExists.value = false;
      htsError.value = 'HTS Code not found on hts.usitc.gov';
      ElMessage.warning('HTS Code not found on hts.usitc.gov');
      return;
    }

    // Code found in USITC (with or without general rate)
    htsExists.value = true;

    // Fallback to 8-digit was used and general rate found → auto-fill
    if (result.fallback_used && result.data?.general) {
      const raw = result.data.general.replace('%', '').trim();
      const rate = raw.toLowerCase() === 'free' ? 0 : parseFloat(raw);
      if (!isNaN(rate)) {
        form.value.rate = rate;
        ElMessage.info('General Duty Rate auto-filled from HTS recommendation.');
      }
    }
  } finally {
    htsLookupLoading.value = false;
  }
};

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
// const isEmployee = computed(() => userRole.value === UserRole.DIMERCO || userRole.value === UserRole.DCB);
const isCustomer = computed(() => userRole.value === UserRole.CUSTOMER);
const isDcb = computed(() => userRole.value === UserRole.DCB);
const isDimerco = computed(() => userRole.value === UserRole.DIMERCO);

const modified = computed(() => partDetail.value?.modified ?? null);
const before = computed(() => partDetail.value?.before ?? null);

// Status badge (狀態標籤)
const currentStatus = computed(() => partDetail.value?.status ?? '');
const statusBadgeLabel = computed(() => t('status.' + statusToI18nKey(currentStatus.value)).toUpperCase());
// INTERNAL-AI-20260420: Changed badge background to use SLA-based color per spec.
// (INTERNAL-AI-20260420: 依規格將標籤背景色改為 SLA 狀態顏色。)
/* const statusBadgeBg = computed(() => {
  const s = currentStatus.value;
  if (s === 'S04') return '#67C23A';
  if (s === 'S01' || s === 'Inactive') return '#909399';
  return '#E6673A';
}); */
const statusBadgeBg = computed(() => {
  const sla = partDetail.value?.slaStatus ?? 'green';
  if (sla === 'red')    return '#F56C6C';
  if (sla === 'orange') return '#FF9800';
  if (sla === 'yellow') return '#E6A23C';
  return '#67C23A'; // green (default)
});

// Basic fields editable when S01/S03, or S04 for non-DCB roles only.
// DCB in S04 can only edit Additional Rate fields.
const canEditBasicFields = computed(() =>
  ['S01', 'S03'].includes(currentStatus.value) ||
  (currentStatus.value === 'S04' && !isDcb.value)
);
// Additional duty fields + Remark: editable when S02/S04.
// (附加關稅欄位及備註在 S02/S04 狀態下可編輯。)
const canEditAdditionalFields = computed(() => ['S02', 'S04'].includes(currentStatus.value));

// S01 (Unknown): Customer sees [Save] + [Save & Send to Dimerco] + [Inactive].
// (S01 Unknown：Customer 顯示 Save + Save & Send to Dimerco + Inactive。)
const showCustomerButtons = computed(() => isCustomer.value && currentStatus.value === 'S01');

// S03 (Pending Customer Review): Customer sees [Save & Send to Dimerco] + [Inactive] only (no bare Save).
// (S03 Pending Customer Review：Customer 僅顯示 Save & Send to Dimerco + Inactive，不顯示 Save。)
const showCustomerS03Buttons = computed(() => isCustomer.value && currentStatus.value === 'S03');

// DCB review panel: Return Reason field + Accept / Return to Customer buttons, shown only for S02.
// (DCB 審核面板：退回原因欄位 + 接受 / 退回按鈕，僅在狀態 S02 時顯示。)
const showDcbReview = computed(() => isDcb.value && currentStatus.value === 'S02');

// S04 (Reviewed) buttons: all roles see Save; Customer additionally sees Save & Send to Dimerco.
// (S04 Reviewed 按鈕：所有角色皆顯示 Save；僅 Customer 額外顯示 Save & Send to Dimerco。)
const showS04Buttons = computed(() => currentStatus.value === 'S04');

// Dimerco Other (non-DCB Dimerco): Save only, for statuses other than S04 and Inactive.
// (Dimerco Other（非 DCB）：僅顯示 Save，適用於 S04 及 Inactive 以外的狀態。)
const showDimercoSaveOnly = computed(() =>
  isDimerco.value && ['S01', 'S02', 'S03'].includes(currentStatus.value)
);

// INTERNAL-AI-20260420: Inline date formatter for milestone display (YYYY-MM-DD HH:mm).
// (INTERNAL-AI-20260420: 里程碑日期格式化，顯示為 YYYY-MM-DD HH:mm。)
const formatDate = (iso: string): string => {
  if (!iso) return '';
  const d = new Date(iso);
  const y = d.getFullYear();
  const mo = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  const h = String(d.getHours()).padStart(2, '0');
  const mi = String(d.getMinutes()).padStart(2, '0');
  return `${y}-${mo}-${day} ${h}:${mi}`;
};
const returnReason = ref('');
const returnReasonError = ref('');

// Dynamic breadcrumb (動態麵包屑)
const prevPageLabel = computed(() => {
  const backState = (window.history.state as any)?.back;
  if (backState && (backState.includes('/customer') || backState.includes('/employee'))) {
    return t('common.menu.dashboard');
  }
  return t('common.menu.parts');
});

// Milestone dot color by action string (依操作名稱決定里程碑點顏色)
const milestoneColor = (action: string): string => {
  const a = action.toLowerCase();
  if (a.includes('customer')) return '#E6A23C';
  if (a.includes('dimerco') || a.includes('review')) return '#409EFF';
  if (a.includes('reviewed') || a.includes('accept')) return '#67C23A';
  return '#909399';
};

/**
 * Load Part Detail and update tab title (載入零件詳情並更新頁籤標題)
 * INTERNAL-AI-20260417: Added dedicated loader to support keep-alive changes.
 */
const initLoad = async (id: number) => {
  loading.value = true;
  htsError.value = '';
  htsCode1Error.value = '';
  htsCode2Error.value = '';
  htsCode3Error.value = '';
  htsCode4Error.value = '';
  try {
    // INTERNAL-AI-20260420: Supplier is free-text; only need detail + milestones.
    // (INTERNAL-AI-20260420: 供應商改為自由輸入，只需載入詳情與里程碑。)
    /* const [detailData, milestoneData, suppliersData] = await Promise.all([...]) */
    const [detailData, milestoneData] = await Promise.all([
      getPartDetail(id),
      getMilestones(id).catch(() => [] as Milestone[]),
    ]);

    if (detailData) {
      partDetail.value = detailData;
      tabStore.updateTabTitle(route.path, detailData.modified.partNo);
      htsExists.value = detailData.modified.isHTSExists ?? null;
      const m = detailData.modified;
      form.value = {
        partNo: m.partNo,
        countryId: m.countryId ?? null,
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
      ElMessage.error('Part not found. / 零件不存在。');
      router.push('/parts');
    }

    milestones.value = milestoneData;
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  initLoad(partId);
});

/**
 * INTERNAL-AI-20260417: Handle dynamic ID changes for Keep-Alive components.
 * (INTERNAL-AI-20260417: 處理 Keep-Alive 組件的動態 ID 變更。)
 */
watch(
  () => route.params.id,
  (newId) => {
    if (newId && route.name === 'part-detail') {
      initLoad(Number(newId));
    }
  }
);

// Sanitize helpers (清理輔助函式)
const toNullableNumber = (v: any) => (v === '' || v === null || Number.isNaN(v) ? null : Number(v));
const toNullableString = (v: any) => (v === '' ? null : v || null);

const buildPayload = (): PartSavePayload => ({
  ...form.value,
  rate:        toNullableNumber(form.value.rate) ?? 0,
  rate1:       toNullableNumber(form.value.rate1),
  rate2:       toNullableNumber(form.value.rate2),
  rate3:       toNullableNumber(form.value.rate3),
  rate4:       toNullableNumber(form.value.rate4),
  htsCode1:    toNullableString(form.value.htsCode1),
  htsCode2:    toNullableString(form.value.htsCode2),
  htsCode3:    toNullableString(form.value.htsCode3),
  htsCode4:    toNullableString(form.value.htsCode4),
  isHTSExists: htsExists.value,
});

// INTERNAL-AI-20260416: Save handler for Customer role — calls PUT /api/parts/{partId}.
// (INTERNAL-AI-20260416: Customer 角色的儲存處理函式，呼叫 PUT /api/parts/{partId}。)
const handleSave = async () => {
  saving.value = true;
  try {
    await updatePart(partId, buildPayload());
    ElMessage.success('Saved successfully. / 儲存成功。');
  } catch {
    // Error already shown by interceptor (錯誤訊息已由攔截器顯示)
  } finally {
    saving.value = false;
  }
};

// INTERNAL-AI-20260416: Submit handler for Customer — calls POST /api/parts/{partId}/submit.
// (INTERNAL-AI-20260416: Customer 角色的送審處理函式，呼叫 POST /api/parts/{partId}/submit。)
const handleSubmit = async () => {
  // INTERNAL-AI-20260420: Validate HTS Code is required before submitting.
  // (INTERNAL-AI-20260420: 送審前驗證 HTS Code 為必填。)
  if (!form.value.htsCode?.trim()) {
    ElMessage.error('US HTS Code is required before submitting. / 送審前 HTS Code 為必填。');
    return;
  }
  if (!validateHts(form.value.htsCode, htsError)) {
    ElMessage.error('US HTS Code format is invalid. / HTS Code 格式錯誤。');
    return;
  }

  try {
    await ElMessageBox.confirm(
      t('part_detail.btn_save_send') + '?',
      t('part_detail.btn_save_send'),
      { confirmButtonClass: 'btn-confirm-orange', type: 'warning' }
    );
  } catch {
    return;
  }

  submitting.value = true;
  try {
    await submitPart(partId, buildPayload());
    ElMessage.success('Submitted to Dimerco for review. / 已送審給 Dimerco。');
    router.push('/parts');
  } catch {
    // interceptor handles error (攔截器顯示錯誤)
  } finally {
    submitting.value = false;
  }
};

// INTERNAL-AI-20260420: Inactivate handler for Customer role — confirms then sets status to Inactive.
// (INTERNAL-AI-20260420: Customer 角色停用處理函式，確認後將狀態設為停用。)
const handleInactivate = async () => {
  try {
    await ElMessageBox.confirm(
      `Are you sure you want to set ${modified.value?.partNo ?? partId} as Inactive? This part will no longer be active in the system.`,
      'Confirm Inactivate',
      { confirmButtonText: 'Inactive', cancelButtonText: 'Cancel', type: 'warning' }
    );
  } catch {
    return;
  }

  inactivating.value = true;
  try {
    await inactivatePart(partId);
    ElMessage.success('Part has been set as Inactive. / 零件已停用。');
    router.push('/parts');
  } catch {
    // interceptor handles error (攔截器顯示錯誤)
  } finally {
    inactivating.value = false;
  }
};

// INTERNAL-AI-20260421: S04 — Save & Send to Dimerco for Dimerco/Customer: validates HTS Code, status → S03.
// (INTERNAL-AI-20260421: S04 狀態下 Dimerco/Customer 的 Save & Send to Dimerco，驗證 HTS Code 必填後狀態改為 S03。)
/* old: submitPart → S02 */
const handleSaveAndResend = async () => {
  if (!form.value.htsCode?.trim()) {
    ElMessage.error('US HTS Code is required before submitting. / 送審前 HTS Code 為必填。');
    return;
  }
  if (!validateHts(form.value.htsCode, htsError)) {
    ElMessage.error('US HTS Code format is invalid. / HTS Code 格式錯誤。');
    return;
  }

  try {
    await ElMessageBox.confirm(
      t('part_detail.btn_save_send') + '?',
      t('part_detail.btn_save_send'),
      { confirmButtonClass: 'btn-confirm-orange', type: 'warning' }
    );
  } catch {
    return;
  }

  submitting.value = true;
  try {
    await sendToCustomerReview(partId, buildPayload());
    ElMessage.success('Sent to Customer Review. / 已送交客戶審核。');
    router.push('/parts');
  } catch {
    // interceptor handles error (攔截器顯示錯誤)
  } finally {
    submitting.value = false;
  }
};

// INTERNAL-AI-20260416: Accept/Return handlers for DCB role.
// (INTERNAL-AI-20260416: DCB 角色的接受/退回處理函式。)
// INTERNAL-AI-20260420: Accept no longer shows a dialog — direct API call per spec.
// (INTERNAL-AI-20260420: 接受不再顯示確認對話框，依規格直接呼叫 API。)
const handleAccept = async () => {
  try {
    await acceptPart(partId);
    ElMessage.success('Part accepted. / 零件已接受。');
    router.push('/parts');
  } catch {
    // interceptor handles error (攔截器顯示錯誤)
  }
};

// INTERNAL-AI-20260420: Return reason is now an inline field; validate before calling API.
// (INTERNAL-AI-20260420: 退回原因改為頁面內欄位，呼叫 API 前先驗證。)
const handleReturn = async () => {
  if (!returnReason.value.trim()) {
    returnReasonError.value = 'Return Reason is required. / 退回原因為必填。';
    return;
  }
  returnReasonError.value = '';
  try {
    await returnPart(partId, returnReason.value.trim());
    ElMessage.success('Returned to customer. / 已退回給客戶。');
    router.push('/parts');
  } catch {
    // interceptor handles error (攔截器顯示錯誤)
  }
};
</script>

<template>
  <div class="page-wrapper">
    <div v-if="loading" class="loading-overlay">
      <div class="spinner"></div>
    </div>

    <div v-else-if="partDetail && modified" class="page-container">
      <!-- Breadcrumb (麵包屑) -->
      <nav class="breadcrumb">
        <a href="#" @click.prevent="router.back()">{{ prevPageLabel }}</a>
        <span class="sep">/</span>
        <span class="current">{{ modified.partNo }}</span>
      </nav>

      <!-- Header: title + status badge (標題 + 狀態標籤) -->
      <header class="page-header">
        <h1>
          {{ modified.partNo }}
          <span class="title-sep">–</span>
          <span class="title-desc">{{ modified.partDesc }}</span>
        </h1>
        <span class="status-badge" :style="{ backgroundColor: statusBadgeBg }">
          {{ statusBadgeLabel }}
        </span>
      </header>

      <!-- Two-column layout (雙欄佈局) -->
      <div class="detail-layout">

        <!-- Left: Part Information table (左側：零件資訊表格) -->
        <div class="main-col">
          <div class="info-card">
            <div class="card-header-row">
              <div class="header-left">
                <div class="header-decorator"></div>
                <span class="card-title">{{ $t('part_detail.part_information') }}</span>
              </div>
              <button class="btn-history" :title="$t('part_detail.history')" @click="openHistoryPanel">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <circle cx="12" cy="12" r="10"/>
                  <polyline points="12 6 12 12 16 14"/>
                </svg>
              </button>
            </div>

            <table class="part-table">
              <thead>
                <tr>
                  <th class="col-field">{{ $t('part_detail.field') }}</th>
                  <th class="col-before">{{ $t('part_detail.before') }}</th>
                  <th class="col-modified" style="color: var(--primary-color)">{{ $t('part_detail.modified_label') }}</th>
                </tr>
              </thead>
              <tbody>
                <!-- Project: read-only (專案：唯讀) -->
                <tr>
                  <td class="field-label">{{ $t('common.project') }}</td>
                  <td class="before-val"></td>
                  <td class="value-blue">{{ modified.project || '—' }}</td>
                </tr>
                <!-- Part No: read-only (零件編號：唯讀) -->
                <tr>
                  <td class="field-label">{{ $t('customer.part_no') }}</td>
                  <td class="before-val"></td>
                  <td class="value-blue">{{ modified.partNo }}</td>
                </tr>
                <!-- Country of Origin: read-only (原產地：唯讀) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.country_of_origin') }}</td>
                  <td class="before-val"></td>
                  <td class="value-blue">{{ modified.country }}</td>
                </tr>
                <!-- Division: editable when S01/S03/S04 (可在 S01/S03/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.division') }}<span v-if="canEditBasicFields" class="req">*</span></td>
                  <td class="before-val">{{ before?.division || '—' }}</td>
                  <td>
                    <input v-if="canEditBasicFields" v-model="form.division" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.division }}</span>
                  </td>
                </tr>
                <!-- Supplier: free-text, editable when S01/S03/S04 (自由輸入，可在 S01/S03/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('common.supplier') }}</td>
                  <td class="before-val">{{ before?.supplier || '—' }}</td>
                  <td>
                    <input v-if="canEditBasicFields" v-model="form.supplier" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.supplier }}</span>
                  </td>
                </tr>
                <!-- Part Description: editable when S01/S03/S04 (可在 S01/S03/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_create.description') }}<span v-if="canEditBasicFields" class="req">*</span></td>
                  <td class="before-val">{{ before?.partDesc || '—' }}</td>
                  <td>
                    <input v-if="canEditBasicFields" v-model="form.partDesc" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.partDesc }}</span>
                  </td>
                </tr>
                <!-- US HTS Code: required, format XXXX.XX.XXXX, editable when S01/S03/S04 -->
                <!-- (US HTS Code：必填，格式 XXXX.XX.XXXX，可在 S01/S03/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.us_hts_code') }}<span v-if="canEditBasicFields" class="req">*</span></td>
                  <td class="before-val mono">
                    {{ before?.htsCode || '—' }}
                    <template v-if="before?.htsCode">
                      <br />
                      <span v-if="before?.isHTSExists === true" class="hts-badge hts-found">✓ Verified on USITC</span>
                      <span v-else-if="before?.isHTSExists === false" class="hts-badge hts-not-found">✗ Not found on USITC</span>
                      <span v-else class="hts-badge hts-unknown">— Not yet verified</span>
                    </template>
                  </td>
                  <td>
                    <template v-if="canEditBasicFields">
                      <input
                        :value="form.htsCode"
                        class="cell-input mono"
                        :class="{ 'input-error': htsError }"
                        placeholder="XXXX.XX.XXXX"
                        inputmode="numeric"
                        :disabled="htsLookupLoading"
                        @blur="handleHtsCodeBlur"
                        @input="handleHtsInput('htsCode', $event)"
                      />
                      <div v-if="htsError" class="field-error">{{ htsError }}</div>
                      <div v-if="htsLookupLoading" class="field-hint">Looking up rate...</div>
                      <span v-else-if="htsExists === true" class="hts-badge hts-found">✓ Verified on USITC</span>
                      <span v-else-if="htsExists === false" class="hts-badge hts-not-found">✗ Not found on USITC</span>
                      <span v-else class="hts-badge hts-unknown">— Not yet verified</span>
                    </template>
                    <template v-else>
                      <span class="cell-text mono">{{ modified.htsCode }}</span>
                      <span v-if="htsExists === true" class="hts-badge hts-found">✓ Verified on USITC</span>
                      <span v-else-if="htsExists === false" class="hts-badge hts-not-found">✗ Not found on USITC</span>
                      <span v-else class="hts-badge hts-unknown">— Not yet verified</span>
                    </template>
                  </td>
                </tr>
                <!-- General Duty Rate: numeric, editable when S01/S03/S04 (數字，可在 S01/S03/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.general_duty_rate') }}</td>
                  <td class="before-val">{{ before?.rate ?? '—' }}</td>
                  <td>
                    <input v-if="canEditBasicFields" v-model.number="form.rate" type="number" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.rate }}</span>
                  </td>
                </tr>

                <!-- Additional Duty section header (附加關稅區段標題) -->
                <tr class="section-divider">
                  <td colspan="3">{{ $t('part_detail.additional_duty') }}</td>
                </tr>

                <!-- HTS Code (301): format XXXX.XX.XXXX, editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.hts_code_301') }}</td>
                  <td class="before-val mono">{{ before?.htsCode1 || '—' }}</td>
                  <td>
                    <template v-if="canEditAdditionalFields">
                      <input :value="form.htsCode1" class="cell-input mono" :class="{ 'input-error': htsCode1Error }" placeholder="XXXX.XX.XXXX" inputmode="numeric" @blur="validateHtsField('htsCode1', form.htsCode1)" @input="handleHtsInput('htsCode1', $event)" />
                      <div v-if="htsCode1Error" class="field-error">{{ htsCode1Error }}</div>
                    </template>
                    <span v-else class="cell-text mono">{{ modified.htsCode1 || '—' }}</span>
                  </td>
                </tr>
                <!-- Rate (301): editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.rate_301') }}</td>
                  <td class="before-val">{{ before?.rate1 ?? '—' }}</td>
                  <td>
                    <input v-if="canEditAdditionalFields" v-model.number="form.rate1" type="number" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.rate1 ?? '—' }}</span>
                  </td>
                </tr>
                <!-- HTS Code (IEEPA): format XXXX.XX.XXXX, editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.hts_code_ieepa') }}</td>
                  <td class="before-val mono">{{ before?.htsCode2 || '—' }}</td>
                  <td>
                    <template v-if="canEditAdditionalFields">
                      <input :value="form.htsCode2" class="cell-input mono" :class="{ 'input-error': htsCode2Error }" placeholder="XXXX.XX.XXXX" inputmode="numeric" @blur="validateHtsField('htsCode2', form.htsCode2)" @input="handleHtsInput('htsCode2', $event)" />
                      <div v-if="htsCode2Error" class="field-error">{{ htsCode2Error }}</div>
                    </template>
                    <span v-else class="cell-text mono">{{ modified.htsCode2 || '—' }}</span>
                  </td>
                </tr>
                <!-- Rate (IEEPA): editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.rate_ieepa') }}</td>
                  <td class="before-val">{{ before?.rate2 ?? '—' }}</td>
                  <td>
                    <input v-if="canEditAdditionalFields" v-model.number="form.rate2" type="number" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.rate2 ?? '—' }}</span>
                  </td>
                </tr>
                <!-- HTS Code (232 Aluminum): format XXXX.XX.XXXX, editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.hts_code_232') }}</td>
                  <td class="before-val mono">{{ before?.htsCode3 || '—' }}</td>
                  <td>
                    <template v-if="canEditAdditionalFields">
                      <input :value="form.htsCode3" class="cell-input mono" :class="{ 'input-error': htsCode3Error }" placeholder="XXXX.XX.XXXX" inputmode="numeric" @blur="validateHtsField('htsCode3', form.htsCode3)" @input="handleHtsInput('htsCode3', $event)" />
                      <div v-if="htsCode3Error" class="field-error">{{ htsCode3Error }}</div>
                    </template>
                    <span v-else class="cell-text mono">{{ modified.htsCode3 || '—' }}</span>
                  </td>
                </tr>
                <!-- Rate (232 Aluminum): editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.rate_232') }}</td>
                  <td class="before-val">{{ before?.rate3 ?? '—' }}</td>
                  <td>
                    <input v-if="canEditAdditionalFields" v-model.number="form.rate3" type="number" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.rate3 ?? '—' }}</span>
                  </td>
                </tr>
                <!-- HTS Code (Reciprocal Tariff): format XXXX.XX.XXXX, editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.hts_code_reciprocal') }}</td>
                  <td class="before-val mono">{{ before?.htsCode4 || '—' }}</td>
                  <td>
                    <template v-if="canEditAdditionalFields">
                      <input :value="form.htsCode4" class="cell-input mono" :class="{ 'input-error': htsCode4Error }" placeholder="XXXX.XX.XXXX" inputmode="numeric" @blur="validateHtsField('htsCode4', form.htsCode4)" @input="handleHtsInput('htsCode4', $event)" />
                      <div v-if="htsCode4Error" class="field-error">{{ htsCode4Error }}</div>
                    </template>
                    <span v-else class="cell-text mono">{{ modified.htsCode4 || '—' }}</span>
                  </td>
                </tr>
                <!-- Rate (Reciprocal Tariff): editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.rate_reciprocal') }}</td>
                  <td class="before-val">{{ before?.rate4 ?? '—' }}</td>
                  <td>
                    <input v-if="canEditAdditionalFields" v-model.number="form.rate4" type="number" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.rate4 ?? '—' }}</span>
                  </td>
                </tr>
                <!-- Remark: editable when S02/S04 (可在 S02/S04 狀態編輯) -->
                <tr>
                  <td class="field-label">{{ $t('part_detail.remark') }}</td>
                  <td class="before-val">{{ before?.remark || '—' }}</td>
                  <td>
                    <input v-if="canEditAdditionalFields" v-model="form.remark" class="cell-input" />
                    <span v-else class="cell-text">{{ modified.remark || '—' }}</span>
                  </td>
                </tr>

                <!-- Return Reason: DCB only, visible when status is S02 (Pending Dimerco Review) -->
                <!-- (退回原因：僅 DCB 角色可見，且僅在狀態為 S02 時顯示) -->
                <template v-if="showDcbReview">
                  <tr class="section-divider">
                    <td colspan="3">{{ $t('part_detail.review_action') }}</td>
                  </tr>
                  <tr>
                    <td class="field-label">
                      {{ $t('part_detail.return_reason_label') }}
                    </td>
                    <td colspan="2">
                      <textarea
                        v-model="returnReason"
                        class="cell-textarea"
                        :class="{ 'input-error': returnReasonError }"
                        :placeholder="$t('part_detail.return_placeholder')"
                        rows="3"
                        @input="returnReasonError = ''"
                      ></textarea>
                      <div v-if="returnReasonError" class="field-error">{{ returnReasonError }}</div>
                    </td>
                  </tr>
                </template>
              </tbody>
            </table>

            <!-- Action buttons (操作按鈕) -->
            <div class="action-footer">
              <!-- S01 (Unknown) Customer: Save + Save & Send to Dimerco + Inactive -->
              <!-- (S01 Unknown Customer：Save + Save & Send to Dimerco + Inactive) -->
              <template v-if="showCustomerButtons">
                <button class="btn-cch btn-save" :disabled="saving || submitting || inactivating" @click="handleSave">
                  {{ saving ? '...' : $t('common.save') }}
                </button>
                <button class="btn-cch btn-submit" :disabled="saving || submitting || inactivating" @click="handleSubmit">
                  {{ submitting ? '...' : $t('part_detail.btn_save_send') }}
                </button>
                <button class="btn-cch btn-inactive" :disabled="saving || submitting || inactivating" @click="handleInactivate">
                  {{ inactivating ? '...' : 'Inactive' }}
                </button>
              </template>
              <!-- S03 (Pending Customer Review) Customer: Save & Send to Dimerco + Inactive (no bare Save) -->
              <!-- (S03 Pending Customer Review Customer：Save & Send to Dimerco + Inactive，不顯示 Save) -->
              <template v-else-if="showCustomerS03Buttons">
                <button class="btn-cch btn-submit" :disabled="submitting || inactivating" @click="handleSubmit">
                  {{ submitting ? '...' : $t('part_detail.btn_save_send') }}
                </button>
                <button class="btn-cch btn-inactive" :disabled="submitting || inactivating" @click="handleInactivate">
                  {{ inactivating ? '...' : 'Inactive' }}
                </button>
              </template>
              <!-- DCB: Accept + Return to Customer only; no Save when status is S02 (Pending Dimerco Review) -->
              <!-- (DCB：僅顯示接受 + 退回給客戶；S02 狀態下不提供 Save) -->
              <template v-else-if="showDcbReview">
                <button class="btn-cch btn-accept" @click="handleAccept">
                  {{ $t('part_detail.btn_accept') }}
                </button>
                <button class="btn-cch btn-return-outline" @click="handleReturn">
                  {{ $t('part_detail.btn_return_customer') }}
                </button>
              </template>
              <!-- S04 (Reviewed): DCB sees Save only; Customer/Dimerco see Save & Send to Dimerco only (no bare Save) -->
              <!-- (S04 已審核：DCB 僅顯示 Save；Customer/Dimerco 僅顯示 Save & Send to Dimerco，不顯示 Save) -->
              <template v-else-if="showS04Buttons">
                <button v-if="isDcb" class="btn-cch btn-save" :disabled="saving || submitting" @click="handleSave">
                  {{ saving ? '...' : $t('common.save') }}
                </button>
                <button
                  v-if="isCustomer || isDimerco"
                  class="btn-cch btn-submit"
                  :disabled="saving || submitting"
                  @click="handleSaveAndResend"
                >
                  {{ submitting ? '...' : $t('part_detail.btn_save_send') }}
                </button>
              </template>
              <!-- Dimerco Other (non-DCB): Save only for S01/S02/S03 -->
              <!-- (Dimerco Other 非 DCB：S01/S02/S03 狀態下僅顯示 Save) -->
              <template v-else-if="showDimercoSaveOnly">
                <button class="btn-cch btn-save" :disabled="saving" @click="handleSave">
                  {{ saving ? '...' : $t('common.save') }}
                </button>
              </template>
            </div>
          </div>
        </div>

        <!-- Right: Timeline sidebar (右側：時間軸側欄) -->
        <div class="side-col">
          <div class="timeline-card">
            <div class="card-header-row">
              <div class="header-decorator secondary"></div>
              <span class="card-title">{{ $t('part_detail.timeline') }}</span>
            </div>

            <div class="timeline-body">
              <div
                v-for="(ms, idx) in milestones"
                :key="idx"
                class="timeline-item"
              >
                <!-- Vertical connector line (連接線) -->
                <div v-if="idx < milestones.length - 1" class="connector-line"></div>
                <!-- Colored dot (顏色點) -->
                <div
                  class="timeline-dot"
                  :style="{ borderColor: milestoneColor(ms.action), backgroundColor: milestoneColor(ms.action) }"
                ></div>
                <div class="timeline-content">
                  <div class="ms-action" :style="{ color: milestoneColor(ms.action) }">{{ ms.action }}</div>
                  <div class="ms-date">{{ formatDate(ms.updatedDate) }}</div>
                  <div class="ms-by">By: {{ ms.updatedBy }}</div>
                  <div v-if="ms.remark" class="ms-remark">"{{ ms.remark }}"</div>
                </div>
              </div>
              <p v-if="milestones.length === 0" class="no-milestones">—</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- History Modal (歷史紀錄視窗) -->
    <div v-if="showHistoryPanel" class="history-overlay" @click.self="showHistoryPanel = false">
      <div class="history-modal">
        <div class="history-modal-header">
          <div class="history-modal-title-group">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#f68b39" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="history-title-icon">
              <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
            </svg>
            <h2 class="history-modal-title">Change History</h2>
            <span class="history-modal-partno">— {{ modified?.partNo }}</span>
          </div>
          <button class="history-close-btn" @click="showHistoryPanel = false" aria-label="Close">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
              <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>

        <div class="history-table-wrap">
          <table class="history-table">
            <thead>
              <tr>
                <th class="h-col-date">{{ $t('common.updated_date') }}</th>
                <th class="h-col-by">{{ $t('common.updated_by') }}</th>
                <th>{{ $t('customer.part_no') }}</th>
                <th>{{ $t('part_detail.country_of_origin') }}</th>
                <th>{{ $t('part_detail.division') }}</th>
                <th>{{ $t('common.supplier') }}</th>
                <th>{{ $t('part_create.description') }}</th>
                <th>{{ $t('part_detail.us_hts_code') }}</th>
                <th>{{ $t('part_detail.general_duty_rate') }}</th>
                <th>{{ $t('part_detail.hts_code_301') }}</th>
                <th>{{ $t('part_detail.rate_301') }}</th>
                <th>{{ $t('part_detail.hts_code_ieepa') }}</th>
                <th>{{ $t('part_detail.rate_ieepa') }}</th>
                <th>{{ $t('part_detail.hts_code_232') }}</th>
                <th>{{ $t('part_detail.rate_232') }}</th>
                <th>{{ $t('part_detail.hts_code_reciprocal') }}</th>
                <th>{{ $t('part_detail.rate_reciprocal') }}</th>
                <th>{{ $t('part_detail.remark') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(row, idx) in filteredHistoryRows" :key="idx" :class="{ 'h-row-latest': idx === 0 }">
                <td class="h-col-date mono">{{ formatDate(row.updatedDate) }}</td>
                <td class="h-col-by">{{ row.updatedBy }}</td>
                <td class="h-bold">{{ row.partNo }}</td>
                <td>{{ row.country }}</td>
                <td>{{ row.division }}</td>
                <td>{{ row.supplier }}</td>
                <td>{{ row.partDesc }}</td>
                <td class="mono">{{ row.htsCode }}</td>
                <td class="h-num">{{ row.rate }}</td>
                <td class="mono">{{ row.htsCode1 || '—' }}</td>
                <td class="h-num">{{ row.rate1 ?? '—' }}</td>
                <td class="mono">{{ row.htsCode2 || '—' }}</td>
                <td class="h-num">{{ row.rate2 ?? '—' }}</td>
                <td class="mono">{{ row.htsCode3 || '—' }}</td>
                <td class="h-num">{{ row.rate3 ?? '—' }}</td>
                <td class="mono">{{ row.htsCode4 || '—' }}</td>
                <td class="h-num">{{ row.rate4 ?? '—' }}</td>
                <td>{{ row.remark || '—' }}</td>
              </tr>
            </tbody>
          </table>
          <p v-if="filteredHistoryRows.length === 0" class="history-empty">No history records found.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Layout (佈局) */
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

/* Breadcrumb */
.breadcrumb {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
  font-size: 0.9rem;
  color: #6c757d;
}
.breadcrumb a { color: var(--primary-color); text-decoration: none; }
.breadcrumb .sep { color: #adb5bd; }

/* Header */
.page-header {
  display: flex;
  align-items: center;
  gap: 1.2rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

h1 {
  font-size: 1.9rem;
  color: var(--sidebar-color);
  margin: 0;
  letter-spacing: -0.01em;
}

.title-sep {
  font-weight: 300;
  color: #adb5bd;
  font-size: 1.6rem;
  margin: 0 0.3rem;
}

.title-desc {
  font-weight: 400;
  color: #525f7f;
  font-size: 1.5rem;
}

.status-badge {
  color: #fff;
  padding: 5px 14px;
  border-radius: 20px;
  font-size: 0.78rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  white-space: nowrap;
}

/* Two-column grid */
.detail-layout {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: 2rem;
  align-items: start;
}

/* Cards */
.info-card, .timeline-card {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.06);
  overflow: hidden;
}

.card-header-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1.1rem 1.5rem;
  border-bottom: 1px solid #f1f3f5;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.btn-history {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: none;
  border-radius: 50%;
  background: #f4f7fc;
  color: #8898aa;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
  flex-shrink: 0;
}

.btn-history:hover {
  background: var(--primary-color);
  color: #fff;
}

.header-decorator {
  width: 4px;
  height: 18px;
  background-color: var(--primary-color);
  border-radius: 2px;
}
.header-decorator.secondary { background-color: #E6A23C; }

.card-title {
  font-size: 1rem;
  font-weight: 600;
  color: var(--sidebar-color);
}

/* Part Information table */
.part-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.part-table thead tr {
  background: #f8f9fe;
}

.part-table th {
  padding: 0.75rem 1.2rem;
  text-align: left;
  font-size: 0.8rem;
  font-weight: 600;
  color: #8898aa;
  text-transform: uppercase;
  border-bottom: 1px solid #eef0f5;
}

.col-field  { width: 26%; }
.col-before { width: 28%; }
.col-modified { width: 46%; }

.part-table tbody tr {
  border-bottom: 1px solid #f0f2f5;
  transition: background 0.15s;
}

.part-table tbody tr:hover { background: #fafbff; }

.part-table td {
  padding: 0.7rem 1.2rem;
  vertical-align: middle;
}

.field-label {
  color: #525f7f;
  font-weight: 500;
}

.before-val {
  color: #f68b39;
  font-weight: 500;
}

.value-blue {
  color: var(--primary-color);
  font-weight: 600;
}

.cell-text {
  color: #32325d;
}

.mono {
  font-family: 'Courier New', monospace;
}

/* Additional Duty section divider */
.section-divider td {
  background: #f4f6fb;
  color: #525f7f;
  font-weight: 700;
  font-size: 0.85rem;
  text-align: center;
  padding: 0.6rem 1.2rem;
  letter-spacing: 0.04em;
  text-transform: uppercase;
  border-top: 1px solid #e9ecef;
  border-bottom: 1px solid #e9ecef;
}

/* Input fields inside table cells */
.cell-input {
  width: 100%;
  box-sizing: border-box;
  padding: 6px 10px;
  border: 1px solid #d0d7de;
  border-radius: 6px;
  font-size: 0.9rem;
  font-family: inherit;
  color: #32325d;
  background: #fff;
  transition: border-color 0.2s;
}

.cell-input.mono {
  font-family: 'Courier New', monospace;
  color: var(--primary-color);
}

.cell-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(64,158,255,0.1);
}

.cell-input.input-error {
  border-color: #f56c6c;
}

.cell-textarea {
  width: 100%;
  box-sizing: border-box;
  padding: 6px 10px;
  border: 1px solid #d0d7de;
  border-radius: 6px;
  font-size: 0.9rem;
  font-family: inherit;
  color: #32325d;
  background: #fff;
  resize: vertical;
  transition: border-color 0.2s;
}

.cell-textarea:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(64,158,255,0.1);
}

.cell-textarea.input-error {
  border-color: #f56c6c;
}

.field-error {
  font-size: 0.75rem;
  color: #f56c6c;
  margin-top: 3px;
}

.field-hint {
  font-size: 0.75rem;
  color: #8898aa;
  margin-top: 3px;
}

.hts-badge {
  display: inline-block;
  font-size: 0.72rem;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 10px;
  margin-top: 4px;
}

.hts-found     { background: #f0faf0; color: #52c41a; border: 1px solid #b7eb8f; }
.hts-not-found { background: #fff2f0; color: #ff4d4f; border: 1px solid #ffa39e; }
.hts-unknown   { background: #f5f5f5; color: #adb5bd; border: 1px solid #d9d9d9; }

.cell-select {
  width: 100%;
  box-sizing: border-box;
  padding: 6px 10px;
  border: 1px solid #d0d7de;
  border-radius: 6px;
  font-size: 0.9rem;
  font-family: inherit;
  color: #32325d;
  background: #fff;
  cursor: pointer;
  appearance: auto;
}

.cell-select:focus {
  outline: none;
  border-color: var(--primary-color);
}

.req {
  color: #f56c6c;
  margin-left: 2px;
}

/* Action footer buttons */
.action-footer {
  display: flex;
  gap: 1rem;
  padding: 1.2rem 1.5rem;
  border-top: 1px solid #f1f3f5;
}

.btn-cch {
  padding: 9px 22px;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.18s;
  border: none;
  display: inline-flex;
  align-items: center;
}

.btn-save {
  background-color: var(--primary-color);
  color: #fff;
}
.btn-save:hover:not(:disabled) { background-color: #337ecc; }
.btn-save:disabled { opacity: 0.55; cursor: not-allowed; }

.btn-submit {
  background-color: #ff9800;
  color: #fff;
}
.btn-submit:hover:not(:disabled) { background-color: #f57c00; }
.btn-submit:disabled { opacity: 0.55; cursor: not-allowed; }

.btn-accept {
  background-color: #ff9800;
  color: #fff;
}
.btn-accept:hover { background-color: #f57c00; }

.btn-return-outline {
  background-color: #fff;
  color: #525f7f;
  border: 1px solid #d0d7de !important;
}
.btn-return-outline:hover { background-color: #f8f9fe; }

.btn-inactive {
  background-color: #909399;
  color: #fff;
}
.btn-inactive:hover:not(:disabled) { background-color: #73767a; }
.btn-inactive:disabled { opacity: 0.55; cursor: not-allowed; }

/* Timeline */
.timeline-card { padding-bottom: 0.5rem; }

.timeline-body {
  padding: 1.2rem 1.5rem;
}

.timeline-item {
  position: relative;
  display: flex;
  gap: 1rem;
  padding-bottom: 1.8rem;
}

.timeline-item:last-child { padding-bottom: 0; }

.connector-line {
  position: absolute;
  left: 7px;
  top: 18px;
  bottom: 0;
  width: 2px;
  background: #e9ecef;
}

.timeline-dot {
  flex-shrink: 0;
  width: 16px;
  height: 16px;
  border-radius: 50%;
  margin-top: 2px;
  z-index: 1;
}

.timeline-content { flex: 1; }

.ms-action {
  font-weight: 700;
  font-size: 0.88rem;
  margin-bottom: 2px;
}

.ms-date {
  font-size: 0.78rem;
  color: #8898aa;
}

.ms-by {
  font-size: 0.8rem;
  color: #8898aa;
  margin-bottom: 4px;
}

.ms-remark {
  font-size: 0.82rem;
  color: #525f7f;
  background: #f8f9fe;
  padding: 6px 10px;
  border-radius: 6px;
  border-left: 3px solid #dee2e6;
  margin-top: 4px;
}

.no-milestones {
  color: #adb5bd;
  font-size: 0.9rem;
  text-align: center;
  padding: 1rem 0;
}

/* Loading */
.loading-overlay {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.spinner {
  width: 42px;
  height: 42px;
  border: 4px solid rgba(0,0,0,0.06);
  border-top-color: #ff9800;
  border-radius: 50%;
  animation: spin 0.9s linear infinite;
}

@keyframes spin { to { transform: rotate(360deg); } }

/* ── History Modal ────────────────────────────────── */
.history-overlay {
  position: fixed;
  inset: 0;
  background: rgba(20, 28, 50, 0.48);
  z-index: 9000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 1.5rem;
}

.history-modal {
  background: #fff;
  border-radius: 14px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.22);
  width: 92vw;
  max-width: 1300px;
  max-height: 82vh;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.history-modal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #f0f2f5;
  flex-shrink: 0;
}

.history-modal-title-group {
  display: flex;
  align-items: center;
  gap: 0.6rem;
}

.history-title-icon { flex-shrink: 0; }

.history-modal-title {
  font-size: 1.05rem;
  font-weight: 700;
  color: #32325d;
  margin: 0;
}

.history-modal-partno {
  font-size: 1rem;
  font-weight: 500;
  color: #8898aa;
}

.history-close-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border: none;
  border-radius: 50%;
  background: #f4f7fc;
  color: #8898aa;
  cursor: pointer;
  transition: background 0.18s, color 0.18s;
  flex-shrink: 0;
}

.history-close-btn:hover {
  background: #f56c6c;
  color: #fff;
}

.history-table-wrap {
  overflow: auto;
  flex: 1;
}

.history-table {
  width: max-content;
  min-width: 100%;
  border-collapse: collapse;
  font-size: 0.82rem;
}

.history-table thead tr {
  background: #f8f9fe;
  position: sticky;
  top: 0;
  z-index: 1;
}

.history-table th {
  padding: 0.65rem 1rem;
  text-align: left;
  font-weight: 600;
  font-size: 0.75rem;
  color: #8898aa;
  text-transform: uppercase;
  letter-spacing: 0.04em;
  border-bottom: 2px solid #eef0f5;
  white-space: nowrap;
}

.history-table td {
  padding: 0.6rem 1rem;
  border-bottom: 1px solid #f0f2f5;
  color: #525f7f;
  white-space: nowrap;
  vertical-align: middle;
}

.history-table tbody tr:hover { background: #fafbff; }

.h-row-latest td {
  background: #fff8f2;
  font-weight: 500;
}

.h-col-date { min-width: 140px; }
.h-col-by   { min-width: 110px; }

.h-bold { font-weight: 600; color: var(--primary-color); }

.h-num { text-align: right; }

.history-empty {
  text-align: center;
  color: #adb5bd;
  padding: 2rem;
  font-size: 0.9rem;
}
</style>
