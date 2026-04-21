<script setup lang="ts">
import { authService, UserRole } from '@src/services/auth/auth';
import { partService, type BulkUploadPreviewReport, type BulkUploadPartData } from '@src/services/part/part';
import Card from '@src/components/common/Card.vue';
import Button from '@src/components/common/Button.vue';
import { ElMessage, type UploadFile } from 'element-plus';
import { Edit } from '@element-plus/icons-vue';
import { useI18n } from 'vue-i18n';

/**
 * Bulk Upload View (批量上傳頁面)
 * Updated by Gemini AI on 2026-04-20: Added Preview logic and comparison UI.
 * (INTERNAL-AI-20260420: 新增預覽邏輯與對比 UI。)
 */

const { t } = useI18n();
const router = useRouter();
const { role, customerId: userCustomerId } = authService.state;
const isEmployee = role && role !== UserRole.CUSTOMER;

const uploadFile = ref<File | null>(null);
const previewing = ref(false);
const confirming = ref(false);
const isCompleted = ref(false);
const previewData = ref<BulkUploadPreviewReport | null>(null);
const filterStatus = ref<string | null>(null); // 'NEW', 'MODIFIED', 'ERROR', 'NOCHANGE' or null

const filteredRows = computed(() => {
  if (!previewData.value) return [];
  if (!filterStatus.value) return previewData.value.rows;
  return previewData.value.rows.filter(row => {
    const s = row.rowStatus?.toUpperCase();
    /* INTERNAL-AI-20260421: Strictly handle only specified statuses. (僅處理指定的狀態。) */
    return s === filterStatus.value;
  });
});

const handleFilter = (status: string | null) => {
  if (filterStatus.value === status) {
    filterStatus.value = null; // Toggle off if clicking the same (再次點擊則取消過濾)
  } else {
    filterStatus.value = status;
  }
};

const customers = ref<{ id: string; name: string }[]>([]);
const selectedCustomerId = ref(isEmployee ? '' : userCustomerId || '');

onMounted(async () => {
  if (isEmployee) {
    try {
      customers.value = await partService.getCustomers();
    } catch (error) {
      console.error('Failed to load customers:', error);
    }
  }
});

const handlePreview = async () => {
  if (isEmployee && !selectedCustomerId.value) {
    ElMessage.warning('Please select a customer first. (請先選擇客戶。)');
    return;
  }
  if (!uploadFile.value) return;

  previewing.value = true;
  try {
    const result = await partService.previewBulkUpload(uploadFile.value, Number(selectedCustomerId.value));
    previewData.value = result;
    ElMessage.success('Preview loaded. (預覽已載入。)');
  } catch (error: any) {
    ElMessage.error(error.message || 'Preview failed. (預覽失敗。)');
  } finally {
    previewing.value = false;
  }
};

const handleFileChange = (file: UploadFile) => {
  if (file.raw) {
    uploadFile.value = file.raw;
    previewData.value = null;

    // Auto-trigger preview (自動觸發預覽)
    handlePreview();
  }
};

const handleConfirm = async () => {
  if (!previewData.value || !previewData.value.rows.length) return;

  // Only confirm rows that are not ERROR (僅確認非 ERROR 的資料列)
  const validData = previewData.value.rows
    .filter(row => row.rowStatus?.toUpperCase() !== 'ERROR')
    .map(row => row.newData);

  if (validData.length === 0) {
    ElMessage.warning('No valid data to upload. (沒有可上傳的有效資料。)');
    return;
  }

  confirming.value = true;
  try {
    const result = await partService.confirmBulkUpload(validData);
    ElMessage.success(`${t('part_upload.confirm_success')} (Inserted: ${result.inserted}, Updated: ${result.updated}, Failed: ${result.failed})`);

    // If no failures, mark as completed (如果沒有失敗，標記為已完成)
    if (result.failed === 0) {
      isCompleted.value = true;
      /* INTERNAL-AI-20260421: Removed redirect as requested. (依要求移除跳轉。) */
    }
  } catch (error: any) {
    ElMessage.error(error.message || t('part_upload.confirm_failed'));
  } finally {
    confirming.value = false;
  }
};

/**
 * Reset the view state for another upload. (重置視圖狀態以進行另一次上傳。)
 */
const handleReset = () => {
  isCompleted.value = false;
  previewData.value = null;
  uploadFile.value = null;
  filterStatus.value = null;
  if (isEmployee) {
    selectedCustomerId.value = '';
  }
};

const handleDownloadTemplate = async () => {
  try {
    await partService.downloadTemplate();
  } catch (error) {
    console.error('Template download failed (範本下載失敗):', error);
  }
};

const getRowStatusType = (status: string) => {
  const s = status?.toUpperCase();
  switch (s) {
    case 'NEW': return 'success';
    case 'MODIFIED': return 'warning';
    case 'NOCHANGE': return 'info';
    case 'ERROR': return 'danger';
    default: return 'info';
  }
};

/**
 * Helper to get localized status label. (獲取本地化狀態標籤的輔助函式。)
 * Updated by Gemini AI on 2026-04-21: Strictly aligned to Error, New, Modified, NoChange.
 * (INTERNAL-AI-20260421: 嚴格對齊至 Error, New, Modified, NoChange。)
 */
const getStatusLabel = (status: string) => {
  const s = status?.toUpperCase();
  switch (s) {
    case 'NEW': return t('part_upload.summary.new');
    case 'MODIFIED': return t('part_upload.summary.modified');
    case 'ERROR': return t('part_upload.summary.error');
    case 'NOCHANGE': return t('part_upload.summary.no_change');
    default: return s;
  }
};
/**
 * Helper to check if a field has changed. (比對欄位是否變更的輔助函式。)
 */
const isFieldChanged = (newData: BulkUploadPartData, originalData: BulkUploadPartData | null | undefined, field: keyof BulkUploadPartData) => {
  if (!originalData) return false;
  return newData[field] !== originalData[field];
};

/**
 * Field Mapping for Comparison (對比欄位映射)
 */
const comparisonFields: { key: keyof BulkUploadPartData; label: string }[] = [
  { key: 'partNo', label: 'Part No' },
  { key: 'country', label: 'Country' },
  { key: 'division', label: 'Division' },
  { key: 'supplier', label: 'Supplier' },
  { key: 'partDesc', label: 'Description' },
  { key: 'htsCode', label: 'HTS Code' },
  { key: 'rate', label: 'Duty Rate (%)' },
  { key: 'htsCode1', label: '301 Duty Code' },
  { key: 'rate1', label: '301 Duty Rate (%)' },
  { key: 'htsCode2', label: 'IEEPA Duty Code' },
  { key: 'rate2', label: 'IEEPA Duty Rate (%)' },
  { key: 'htsCode3', label: '232 Aluminum Code' },
  { key: 'rate3', label: '232 Aluminum Rate (%)' },
  { key: 'htsCode4', label: 'Reciprocal Tariff Code' },
  { key: 'rate4', label: 'Reciprocal Tariff Rate (%)' },
  { key: 'remark', label: 'Remark' }
];
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
      <nav class="breadcrumb">
        <a href="#" @click.prevent="router.back()">{{ $t('common.menu.parts') }}</a>
        <span class="separator">/</span>
        <span class="current">{{ $t('part_upload.title') }}</span>
      </nav>

      <header class="page-header">
        <h1>{{ $t('part_upload.title') }}</h1>
      </header>

      <div class="upload-section">
        <Card class="upload-card">
          <template v-if="!isCompleted">
            <div class="template-action">
              <Button type="secondary" @click="handleDownloadTemplate">
                {{ $t('part_upload.download_template') }}
              </Button>
            </div>

            <div v-if="isEmployee" class="customer-selection-row mb-6">
              <label class="block text-sm text-gray-600 mb-2">{{ $t('employee.customer_select') }} <span class="text-red-500">*</span></label>
              <el-select 
                v-model="selectedCustomerId" 
                :placeholder="$t('employee.customer_select')"
                class="w-full max-w-md"
                filterable
                :disabled="!!previewData || previewing"
              >
                <el-option
                  v-for="c in customers"
                  :key="c.id"
                  :label="c.name"
                  :value="c.id"
                />
              </el-select>
            </div>

            <div v-loading="previewing">
              <el-upload
                class="upload-dragger"
                drag
                action="#"
                :auto-upload="false"
                :on-change="handleFileChange"
                :limit="1"
                accept=".csv,.xls,.xlsx"
                :show-file-list="false"
                :disabled="!!previewData || previewing"
              >
                <div class="el-upload__text">
                  {{ $t('part_upload.dragger_text') }}
                </div>
              </el-upload>
            </div>

            <!-- Move Bulk Upload Button Here (移至此處) -->
            <div v-if="previewData" class="action-footer mt-6 flex justify-center">
              <Button 
                :loading="confirming" 
                :disabled="previewData.summary.errorCount === previewData.summary.totalRows"
                @click="handleConfirm"
              >
                {{ $t('part_upload.confirm_button') }}
              </Button>
            </div>
          </template>
          
          <template v-else>
            <div class="completion-status text-center py-10">
              <el-result
                icon="success"
                :title="$t('part_upload.success_title')"
                :sub-title="$t('part_upload.success_message')"
              >
                <template #extra>
                  <Button @click="handleReset">{{ $t('part_upload.re_upload') }}</Button>
                </template>
              </el-result>
            </div>
          </template>
        </Card>
      </div>

      <!-- Preview Summary (預覽摘要) -->
      <div v-if="previewData" class="report-section mt-6">
        <div class="summary-banner mb-4">
          <div 
            class="summary-item cursor-pointer transition-all" 
            :class="{ 'is-active': filterStatus === null }"
            @click="handleFilter(null)"
          >
            <span class="label">{{ $t('part_upload.summary.total') }}</span>
            <span class="value">{{ previewData.summary.totalRows }}</span>
          </div>
          <div 
            class="summary-item text-success cursor-pointer transition-all" 
            :class="{ 'is-active': filterStatus === 'NEW' }"
            @click="handleFilter('NEW')"
          >
            <span class="label">{{ $t('part_upload.summary.new') }}</span>
            <span class="value">{{ previewData.summary.newCount }}</span>
          </div>
          <div 
            class="summary-item text-warning cursor-pointer transition-all" 
            :class="{ 'is-active': filterStatus === 'MODIFIED' }"
            @click="handleFilter('MODIFIED')"
          >
            <span class="label">{{ $t('part_upload.summary.modified') }}</span>
            <span class="value">{{ previewData.summary.modifiedCount }}</span>
          </div>
          <div 
            class="summary-item text-danger cursor-pointer transition-all" 
            :class="{ 'is-active': filterStatus === 'ERROR' }"
            @click="handleFilter('ERROR')"
          >
            <span class="label">{{ $t('part_upload.summary.error') }}</span>
            <span class="value">{{ previewData.summary.errorCount }}</span>
          </div>
          <div 
            class="summary-item text-info cursor-pointer transition-all" 
            :class="{ 'is-active': filterStatus === 'NOCHANGE' }"
            @click="handleFilter('NOCHANGE')"
          >
            <span class="label">{{ $t('part_upload.summary.no_change') }}</span>
            <span class="value">{{ previewData.summary.noChangeCount }}</span>
          </div>
        </div>

        <!-- Comparison Table (對比表格) -->
        <Card>
          <div class="card-header-padding">
            <h3>{{ $t('part_upload.preview_details') }}</h3>
          </div>
          <el-table :data="filteredRows" style="width: 100%" class="preview-table">
            <el-table-column type="expand">
              <template #default="scope">
                <div class="row-detail-comparison p-6 bg-gray-50 rounded-lg m-4 border border-gray-100">
                  <div v-if="scope.row.errors && scope.row.errors.length" class="error-list mb-6">
                    <el-alert
                      v-for="(err, idx) in scope.row.errors"
                      :key="idx"
                      :title="err"
                      type="error"
                      show-icon
                      :closable="false"
                      class="mb-2 shadow-sm"
                    />
                  </div>
                  
                  <!-- Beautified Comparison Grid (美化後的對比網格) -->
                  <div class="comparison-container">
                    <div class="comparison-grid-header">
                      <div class="grid-col-label">{{ $t('part_upload.comparison.field') }}</div>
                      <div class="grid-col-old">{{ $t('part_upload.comparison.original') }}</div>
                      <div class="grid-col-new">{{ $t('part_upload.comparison.new') }}</div>
                    </div>

                    <div v-for="field in comparisonFields" :key="field.key" class="comparison-row" :class="{ 'is-changed': isFieldChanged(scope.row.newData, scope.row.originalData, field.key) }">
                      <div class="grid-col-label">{{ field.label }}</div>
                      <div class="grid-col-old">
                        {{ scope.row.originalData ? (scope.row.originalData[field.key] ?? '-') : '-' }}
                      </div>
                      <div class="grid-col-new">
                        <span :class="{ 'highlight-change': isFieldChanged(scope.row.newData, scope.row.originalData, field.key) }">
                          {{ scope.row.newData[field.key] ?? '-' }}
                        </span>
                        <el-icon v-if="isFieldChanged(scope.row.newData, scope.row.originalData, field.key)" class="ml-2 text-warning"><Edit /></el-icon>
                      </div>
                    </div>
                  </div>
                </div>
              </template>
            </el-table-column>
            
            <el-table-column prop="rowIndex" label="Row" width="80" />
            <el-table-column prop="newData.partNo" label="Part No" width="180" />
            <el-table-column prop="newData.htsCode" label="HTS Code" width="150" />
            <el-table-column prop="rowStatus" label="Status" width="120">
              <template #default="scope">
                <el-tag :type="getRowStatusType(scope.row.rowStatus)" effect="dark">
                  {{ getStatusLabel(scope.row.rowStatus) }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="Summary">
              <template #default="scope">
                <!-- Only show error messages here (僅在此處顯示錯誤訊息) -->
                <span v-if="scope.row.rowStatus?.toUpperCase() === 'ERROR'" class="text-danger font-bold">
                  {{ scope.row.errors?.join(', ') || t('part_upload.summary.error') }}
                </span>
              </template>
            </el-table-column>
          </el-table>
        </Card>
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

h1 {
  font-size: 2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.upload-card {
  padding: 2.5rem;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.05);
}

.template-action {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: flex-end;
}

.customer-selection-row {
  background: #f8f9fe;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #e9ecef;
}

.upload-dragger {
  width: 100%;
}

:deep(.el-upload-dragger) {
  border: 2px dashed #dee2e6;
  border-radius: 16px;
  padding: 50px;
  transition: all 0.3s;
  background: #fafbfc;
}

:deep(.el-upload-dragger:hover) {
  border-color: var(--primary-color);
  background: white;
}

.summary-banner {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 1.5rem;
  background: white;
  padding: 1.5rem;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.05);
}

.summary-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  /* Remove border-right as requested (依要求移除右側線條) */
}

.summary-item .label {
  font-size: 0.8rem;
  color: #8898aa;
  text-transform: uppercase;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.summary-item .value {
  font-size: 1.75rem;
  font-weight: 700;
}

.summary-item.cursor-pointer:hover {
  background-color: #f8f9fe;
  transform: translateY(-2px);
}

.summary-item.is-active {
  background-color: #f0f7ff;
  border-radius: 8px;
  /* Remove border-bottom and box-shadow line (移除底線與陰影線條) */
}

/* Beautified Comparison Grid (美化的對比網格) */
.comparison-container {
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 2px 12px rgba(0,0,0,0.02);
  border: 1px solid #ebeef5;
}

.comparison-grid-header {
  display: grid;
  grid-template-columns: 240px 1fr 1fr;
  background: #f5f7fa;
  padding: 12px 20px;
  font-weight: 700;
  color: #303133;
  border-bottom: 1px solid #ebeef5;
  font-size: 0.9rem;
}

.comparison-row {
  display: grid;
  grid-template-columns: 240px 1fr 1fr;
  padding: 12px 20px;
  border-bottom: 1px solid #f1f3f5;
  font-size: 0.9rem;
  transition: background 0.2s;
}

.comparison-row:last-child { border-bottom: none; }
.comparison-row:hover { background: #fafafa; }

.comparison-row.is-changed {
  background: #fffdf5;
}

.grid-col-label {
  color: #909399;
  font-weight: 500;
}

.grid-col-old {
  color: #606266;
  padding-left: 20px;
  border-left: 1px solid #f1f3f5;
}

.grid-col-new {
  color: #303133;
  padding-left: 20px;
  border-left: 1px solid #f1f3f5;
  display: flex;
  align-items: center;
}

.highlight-change {
  color: #e6a23c;
  font-weight: 700;
  background: #fffbe6;
  padding: 2px 6px;
  border-radius: 4px;
}

.card-header-padding { 
  padding: 1.5rem 2rem; 
  border-bottom: 1px solid #f1f3f5;
  background: #fafbfc;
}

.card-header-padding h3 {
  margin: 0;
  font-size: 1.1rem;
  color: #303133;
}

.text-success { color: #67C23A; }
.text-warning { color: #E6A23C; }
.text-danger { color: #F56C6C; }
.text-info { color: #909399; }
.mb-6 { margin-bottom: 1.5rem; }
.mb-4 { margin-bottom: 1rem; }
.mt-6 { margin-top: 1.5rem; }
.mt-4 { margin-top: 1rem; }
.pb-6 { padding-bottom: 1.5rem; }
.flex { display: flex; }
.justify-center { justify-content: center; }
.ml-2 { margin-left: 0.5rem; }
.w-full { width: 100%; }
.max-w-md { max-width: 28rem; }
.block { display: block; }
.text-sm { font-size: 0.875rem; }
.text-gray-600 { color: #4b5563; }
.text-red-500 { color: #ef4444; }
</style>
