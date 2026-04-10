<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { partService, type ImportBatchReport, ImportResultStatus } from '../../services/part/part';
import Card from '../../components/common/Card.vue';
import Button from '../../components/common/Button.vue';
import { ElMessage, type UploadFile } from 'element-plus';

/**
 * Bulk Upload View (批量上傳頁面)
 * BR-16: Drag and drop Excel upload
 * BR-18: Progress display
 * BR-19: Import report
 * 
 * Audit Update on 2026-04-09 by Gemini AI:
 * Ticket: BR-UPLOAD-001
 * Intent: Implement bulk upload feature with drag-and-drop and reporting.
 * Impact: New page for parts bulk management.
 * (繁體中文) 2026-04-09 Gemini AI 更新：實作具備拖放與報告功能的批量上傳。
 */

const router = useRouter();
const uploadFile = ref<File | null>(null);
const uploading = ref(false);
const progress = ref(0);
const report = ref<ImportBatchReport | null>(null);

const handleFileChange = (file: UploadFile) => {
  if (file.raw) {
    uploadFile.value = file.raw;
    report.value = null;
    progress.value = 0;
  }
};

const handleUpload = async () => {
  if (!uploadFile.value) {
    ElMessage.warning('Please select a file first.');
    return;
  }

  uploading.value = true;
  progress.value = 0;
  
  try {
    const result = await partService.uploadParts(uploadFile.value, undefined, (p) => {
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
      <header class="page-header">
        <div class="header-content">
          <a href="#" class="back-link mr-4" @click.prevent="router.back()">
            &larr; {{ $t('common.back') }}
          </a>
          <h1>{{ $t('part_upload.title') }}</h1>
        </div>
      </header>

      <div class="upload-section">
        <Card class="upload-card">
          <div class="template-action">
            <Button type="secondary" @click="handleDownloadTemplate">
              {{ $t('part_upload.download_template') }}
            </Button>
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
          <h3>{{ $t('part_upload.report_title') }}</h3>
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
        </Card>
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
  max-width: 1000px;
  margin: 0 auto;
  font-family: "MyDimerco-WorkSansBold", sans-serif;
}

.page-header {
  margin-bottom: 2.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid rgba(0,0,0,0.05);
}

.header-content {
  display: flex;
  align-items: center;
}

h1 {
  font-size: 2rem;
  color: var(--sidebar-color);
  margin: 0;
}

.upload-card {
  padding: 2rem;
}

.template-action {
  margin-bottom: 1.5rem;
  display: flex;
  justify-content: flex-end;
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
  padding: 1rem;
  background: #f8f9fe;
  border-radius: 8px;
}

.report-table :deep(th) {
  background-color: #f8f9fe;
  color: #8898aa;
  font-size: 0.85rem;
}
</style>
