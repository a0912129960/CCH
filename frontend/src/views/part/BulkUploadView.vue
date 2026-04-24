<script setup lang="ts">
import { authService, UserRole } from '@src/services/auth/auth';
import { partService, type BulkUploadPreviewReport } from '@src/services/part/part';
import Card from '@src/components/common/Card.vue';
import Button from '@src/components/common/Button.vue';
import { ElMessage, type UploadFile } from 'element-plus';
import { useI18n } from 'vue-i18n';
import BulkUploadPreviewTable from '@src/components/features/part/BulkUploadPreviewTable.vue';

/**
 * Bulk Upload View (批量上傳頁面)
 * Updated by Gemini AI on 2026-04-21 per Supreme Quality Mandate.
 * (INTERNAL-AI-20260421: 依最高品質授權重構：1. 使用 shallowRef 2. 邏輯解耦 3. UI 原子化。)
 * Update on 2026-04-23: Refactored from Customer to Project focus.
 */

const { t } = useI18n();
const router = useRouter();
const { role, projectId: userProjectId } = authService.state;
const isEmployee = role && role !== UserRole.CUSTOMER;

const uploadFile = ref<File | null>(null);
const previewing = ref(false);
const confirming = ref(false);
const isCompleted = ref(false);

/**
 * MANDATORY: Using shallowRef for massive array data to optimize performance (Rule 1.2).
 * (針對巨量陣列資料使用 shallowRef 以優化效能。)
 */
const previewData = shallowRef<BulkUploadPreviewReport | null>(null);
const filterStatus = ref<string | null>(null); // 'NEW', 'MODIFIED', 'ERROR', 'NOCHANGE' or null

const filteredRows = computed(() => {
  if (!previewData.value) return [];
  if (!filterStatus.value) return previewData.value.rows;
  return previewData.value.rows.filter(row => {
    const s = row.rowStatus?.toUpperCase();
    return s === filterStatus.value;
  });
});

const handleFilter = (status: string | null) => {
  if (filterStatus.value === status) {
    filterStatus.value = null; // Toggle off
  } else {
    filterStatus.value = status;
  }
};

const projects = ref<{ id: string; name: string }[]>([]);
const selectedProjectId = ref(isEmployee ? '' : userProjectId || '');

onMounted(async () => {
  try {
    projects.value = await partService.getProjects();
  } catch (error) {
    console.error('Failed to load projects:', error);
  }
});

const handlePreview = async () => {
  if (!selectedProjectId.value) {
    ElMessage.warning(t('employee.project_select'));
    return;
  }
  if (!uploadFile.value) return;

  previewing.value = true;
  try {
    const result = await partService.previewBulkUpload(uploadFile.value, Number(selectedProjectId.value));
    previewData.value = result;
    ElMessage.success('Preview loaded.');
  } catch (error: any) {
    ElMessage.error(error.message || 'Preview failed.');
  } finally {
    previewing.value = false;
  }
};

const handleFileChange = (file: UploadFile) => {
  if (file.raw) {
    uploadFile.value = file.raw;
    previewData.value = null;
    handlePreview();
  }
};

const handleConfirm = async () => {
  if (!previewData.value || !previewData.value.rows.length) return;

  const validData = previewData.value.rows
    .filter(row => row.rowStatus?.toUpperCase() !== 'ERROR')
    .map(row => row.newData);

  if (validData.length === 0) {
    ElMessage.warning(t('part_upload.confirm_failed'));
    return;
  }

  confirming.value = true;
  try {
    const result = await partService.confirmBulkUpload(validData);
    ElMessage.success(`${t('part_upload.confirm_success')} (Inserted: ${result.inserted}, Updated: ${result.updated}, Failed: ${result.failed})`);

    if (result.failed === 0) {
      isCompleted.value = true;
    }
  } catch (error: any) {
    ElMessage.error(error.message || t('part_upload.confirm_failed'));
  } finally {
    confirming.value = false;
  }
};

const handleReset = () => {
  isCompleted.value = false;
  previewData.value = null;
  uploadFile.value = null;
  filterStatus.value = null;
  if (isEmployee) selectedProjectId.value = '';
};

const handleDownloadTemplate = async () => {
  try {
    await partService.downloadTemplate();
  } catch (error) {
    console.error('Template download failed:', error);
  }
};
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

            <div class="customer-selection-row mb-6">
              <label class="block text-sm text-gray-600 mb-2">{{ $t('employee.project_select') }} <span class="text-red-500">*</span></label>
              <el-select 
                v-model="selectedProjectId" 
                :placeholder="$t('employee.project_select')"
                class="w-full max-w-md"
                filterable
                :disabled="!!previewData || previewing"
              >
                <el-option v-for="p in projects" :key="p.id" :label="p.name" :value="p.id" />
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
                <div class="el-upload__text">{{ $t('part_upload.dragger_text') }}</div>
              </el-upload>
            </div>

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
              <el-result icon="success" :title="$t('part_upload.success_title')" :sub-title="$t('part_upload.success_message')">
                <template #extra>
                  <Button @click="handleReset">{{ $t('part_upload.re_upload') }}</Button>
                </template>
              </el-result>
            </div>
          </template>
        </Card>
      </div>

      <!-- Preview Section (預覽區塊) -->
      <div v-if="previewData" class="report-section mt-6">
        <div class="summary-banner mb-4">
          <div v-for="s in ['TOTAL', 'NEW', 'MODIFIED', 'ERROR', 'NOCHANGE']" 
            :key="s"
            class="summary-item cursor-pointer transition-all"
            :class="[
              { 'is-active': filterStatus === (s === 'TOTAL' ? null : s) },
              s === 'NEW' ? 'text-success' : s === 'MODIFIED' ? 'text-warning' : s === 'ERROR' ? 'text-danger' : s === 'NOCHANGE' ? 'text-info' : ''
            ]"
            @click="handleFilter(s === 'TOTAL' ? null : s)"
          >
            <span class="label">{{ $t(`part_upload.summary.${s.toLowerCase()}`) }}</span>
            <span class="value">
              {{ s === 'TOTAL' ? previewData.summary.totalRows : 
                 s === 'NEW' ? previewData.summary.newCount : 
                 s === 'MODIFIED' ? previewData.summary.modifiedCount : 
                 s === 'ERROR' ? previewData.summary.errorCount : previewData.summary.noChangeCount }}
            </span>
          </div>
        </div>

        <Card>
          <div class="card-header-padding">
            <h3>{{ $t('part_upload.preview_details') }}</h3>
          </div>
          <!-- Refactored: Extracted Table Component (Rule 2.2) -->
          <BulkUploadPreviewTable :rows="filteredRows" />
        </Card>
      </div>
    </div>
  </div>
</template>

<style scoped>
.page-wrapper { background-color: #f4f7fc; min-height: 100vh; padding: 2rem 0; }
.page-container { padding: 0 3rem; max-width: 1400px; margin: 0 auto; }
.breadcrumb { display: flex; align-items: center; gap: 0.5rem; margin-bottom: 2rem; font-size: 0.9rem; }
.breadcrumb a { color: var(--primary-color); text-decoration: none; }
.breadcrumb .separator { color: #adb5bd; }
.breadcrumb .current { color: #6c757d; }
.page-header { margin-bottom: 2.5rem; }
h1 { font-size: 2rem; color: var(--sidebar-color); margin: 0; }
.upload-card { padding: 2.5rem; border-radius: 16px; box-shadow: 0 4px 20px rgba(0,0,0,0.05); }
.template-action { margin-bottom: 1.5rem; display: flex; justify-content: flex-end; }
.customer-selection-row { background: #f8f9fe; padding: 1.5rem; border-radius: 12px; border: 1px solid #e9ecef; }
.upload-dragger { width: 100%; }
:deep(.el-upload-dragger) { border: 2px dashed #dee2e6; border-radius: 16px; padding: 50px; transition: all 0.3s; background: #fafbfc; }
:deep(.el-upload-dragger:hover) { border-color: var(--primary-color); background: white; }
.summary-banner { display: grid; grid-template-columns: repeat(5, 1fr); gap: 1.5rem; background: white; padding: 1.5rem; border-radius: 16px; box-shadow: 0 4px 12px rgba(0,0,0,0.05); }
.summary-item { display: flex; flex-direction: column; align-items: center; }
.summary-item .label { font-size: 0.8rem; color: #8898aa; text-transform: uppercase; margin-bottom: 0.5rem; font-weight: 600; }
.summary-item .value { font-size: 1.75rem; font-weight: 700; }
.summary-item.cursor-pointer:hover { background-color: #f8f9fe; transform: translateY(-2px); }
.summary-item.is-active { background-color: #f0f7ff; border-radius: 8px; }
.card-header-padding { padding: 1.5rem 2rem; border-bottom: 1px solid #f1f3f5; background: #fafbfc; }
.card-header-padding h3 { margin: 0; font-size: 1.1rem; color: #303133; }
.text-success { color: #67C23A; }
.text-warning { color: #E6A23C; }
.text-danger { color: #F56C6C; }
.text-info { color: #909399; }
.mb-6 { margin-bottom: 1.5rem; }
.mb-4 { margin-bottom: 1rem; }
.mt-6 { margin-top: 1.5rem; }
.flex { display: flex; }
.justify-center { justify-content: center; }
.w-full { width: 100%; }
.max-w-md { max-width: 28rem; }
.block { display: block; }
.text-sm { font-size: 0.875rem; }
.text-gray-600 { color: #4b5563; }
.text-red-500 { color: #ef4444; }
.text-center { text-center: center; }
.py-10 { padding: 2.5rem 0; }
</style>
