<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { authService, UserRole } from '../../services/auth/auth';
import { partService, type ImportBatchReport, ImportResultStatus } from '../../services/part/part';
import Card from '../../components/common/Card.vue';
import Button from '../../components/common/Button.vue';
import { ElMessage, type UploadFile } from 'element-plus';

/**
 * Bulk Upload View (批量上傳頁面)
 * BR-16: Drag and drop Excel upload
 * BR-18: Progress display
 * BR-19: Import report
 * Updated: Mandatory Customer ID for Employees and Breadcrumb Navigation.
 */

const router = useRouter();
const { role, customerId: userCustomerId } = authService.state;
const isEmployee = role && role !== UserRole.CUSTOMER;

const uploadFile = ref<File | null>(null);
const uploading = ref(false);
const progress = ref(0);
const report = ref<ImportBatchReport | null>(null);

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

const handleFileChange = (file: UploadFile) => {
  if (file.raw) {
    uploadFile.value = file.raw;
    report.value = null;
    progress.value = 0;
  }
};

const handleUpload = async () => {
  if (isEmployee && !selectedCustomerId.value) {
    ElMessage.warning('Please select a customer first.');
    return;
  }
  if (!uploadFile.value) {
    ElMessage.warning('Please select a file first.');
    return;
  }

  uploading.value = true;
  progress.value = 0;
  
  try {
    const result = await partService.uploadParts(uploadFile.value, selectedCustomerId.value, (p) => {
      progress.value = p;
    });
    report.value = result;
    ElMessage.success('Upload completed successfully.');
  } catch (error) {
    ElMessage.error('Upload failed. Please try again.');
  } finally {
    uploading.value = false;
  }
};

const handleDownloadTemplate = () => {
  partService.downloadTemplate();
};

const getStatusType = (status: ImportResultStatus) => {
  switch (status) {
    case ImportResultStatus.NEW: return 'success';
    case ImportResultStatus.UPDATED: return 'warning';
    case ImportResultStatus.UNCHANGED: return 'info';
    case ImportResultStatus.REJECTED: return 'danger';
    default: return 'info';
  }
};
</script>

<template>
  <div class="page-wrapper">
    <div class="page-container">
      <!-- Breadcrumb Navigation -->
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
          <div class="template-action">
            <Button type="secondary" @click="handleDownloadTemplate">
              {{ $t('part_upload.download_template') }}
            </Button>
          </div>

          <!-- Customer Selection (Employee Only) - Mandatory -->
          <div v-if="isEmployee" class="customer-selection-row mb-6">
            <label class="block text-sm text-gray-600 mb-2">{{ $t('employee.customer_select') }} <span class="text-red-500">*</span></label>
            <el-select 
              v-model="selectedCustomerId" 
              :placeholder="$t('employee.customer_select')"
              class="w-full max-w-md"
              filterable
            >
              <el-option
                v-for="c in customers"
                :key="c.id"
                :label="c.name"
                :value="c.id"
              />
            </el-select>
          </div>

          <el-upload
            class="upload-dragger"
            drag
            action="#"
            :auto-upload="false"
            :on-change="handleFileChange"
            :limit="1"
            accept=".csv,.xls,.xlsx"
          >
            <i class="el-icon-upload"></i>
            <div class="el-upload__text">
              {{ $t('part_upload.dragger_text') }}
            </div>
            <template #tip>
              <div class="el-upload__tip">
                {{ $t('part_upload.dragger_hint') }}
              </div>
            </template>
          </el-upload>

          <div v-if="uploadFile" class="action-footer">
            <Button :loading="uploading" @click="handleUpload">
              {{ $t('part_upload.upload_button') }}
            </Button>
          </div>

          <div v-if="uploading" class="progress-wrapper">
            <label>{{ $t('part_upload.progress') }}</label>
            <el-progress :percentage="progress" />
          </div>
        </Card>
      </div>

      <div v-if="report" class="report-section mt-6">
        <Card>
          <div class="card-header-padding">
            <h3>{{ $t('part_upload.report_title') }}</h3>
          </div>
          <div class="card-body-padding">
            <div class="summary-banner">
              <el-tag type="info">Total: {{ report.totalRows }}</el-tag>
              <el-tag type="success">New: {{ report.newCount }}</el-tag>
              <el-tag type="warning">Updated: {{ report.updatedCount }}</el-tag>
              <el-tag type="info" effect="plain">Unchanged: {{ report.unchangedCount }}</el-tag>
              <el-tag type="danger">Rejected: {{ report.rejectedCount }}</el-tag>
            </div>

            <el-table :data="report.rows" style="width: 100%" class="report-table mt-4">
              <el-table-column prop="partNo" :label="$t('part_upload.table.part_no')" width="180" />
              <el-table-column prop="htsCode" :label="$t('part_upload.table.hts_code')" width="180" />
              <el-table-column prop="status" :label="$t('part_upload.table.status')" width="120">
                <template #default="scope">
                  <el-tag :type="getStatusType(scope.row.status)">
                    {{ $t('part_upload.result.' + scope.row.status.toLowerCase()) }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="message" :label="$t('part_upload.table.message')" />
            </el-table>
          </div>
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
  max-width: 1200px;
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

.page-header {
  margin-bottom: 2.5rem;
}

h1 {
  font-size: 2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.upload-card {
  padding: 2rem;
  border-radius: 12px;
}

.template-action {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: flex-end;
}

.customer-selection-row {
  background: #f8f9fe;
  padding: 1.5rem;
  border-radius: 10px;
  border: 1px solid #e9ecef;
}

.upload-dragger {
  width: 100%;
}

:deep(.el-upload-dragger) {
  border: 2px dashed #dee2e6;
  border-radius: 12px;
  padding: 40px;
  transition: border-color 0.3s;
}

:deep(.el-upload-dragger:hover) {
  border-color: var(--primary-color);
}

.action-footer {
  margin-top: 2rem;
  display: flex;
  justify-content: center;
}

.progress-wrapper {
  margin-top: 2rem;
}

.progress-wrapper label {
  display: block;
  margin-bottom: 0.5rem;
  font-size: 0.9rem;
  color: #8898aa;
}

.summary-banner {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  padding: 1.2rem;
  background: #f8f9fe;
  border-radius: 10px;
  border: 1px solid #e9ecef;
}

.report-table :deep(th) {
  background-color: #f8f9fe;
  color: #8898aa;
  font-size: 0.85rem;
}

.card-body-padding { padding: 1.5rem 2rem; }
.card-header-padding { padding: 1.2rem 2rem; border-bottom: 1px solid #f1f3f5; }

.mb-6 { margin-bottom: 1.5rem; }
.mb-2 { margin-bottom: 0.5rem; }
.mt-6 { margin-top: 1.5rem; }
.mt-4 { margin-top: 1rem; }
.mr-4 { margin-right: 1rem; }
.w-full { width: 100%; }
.max-w-md { max-width: 28rem; }
.block { display: block; }
.text-sm { font-size: 0.875rem; }
.text-gray-600 { color: #4b5563; }
.text-red-500 { color: #ef4444; }
</style>
