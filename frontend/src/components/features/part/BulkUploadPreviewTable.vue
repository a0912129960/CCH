<script setup lang="ts">
import { Edit } from '@element-plus/icons-vue';
import { useI18n } from 'vue-i18n';
import { type BulkUploadPreviewRow } from '@src/services/part/part';
import { comparisonFields, isFieldChanged, getStatusLabel, getRowStatusType } from '@src/views/part/utils/bulkUploadHelper';

/**
 * Bulk Upload Preview Table Component (批量上傳預覽表格組件)
 * Created by Gemini AI on 2026-04-21 per Supreme Quality Mandate.
 * (INTERNAL-AI-20260421: 依最高品質授權建立。)
 */

// Vue 3.5+ Reactive Props Destructuring (原生響應式解構)
const { rows = [] } = defineProps<{
  rows: BulkUploadPreviewRow[];
}>();

const { t } = useI18n();
</script>

<template>
  <el-table :data="rows" style="width: 100%" class="preview-table">
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
          
          <div class="comparison-container">
            <div class="comparison-grid-header">
              <div class="grid-col-label">{{ t('part_upload.comparison.field') }}</div>
              <div class="grid-col-old">{{ t('part_upload.comparison.original') }}</div>
              <div class="grid-col-new">{{ t('part_upload.comparison.new') }}</div>
            </div>

            <div 
              v-for="field in comparisonFields" 
              :key="field.key" 
              class="comparison-row" 
              :class="{ 'is-changed': isFieldChanged(scope.row.newData, scope.row.originalData, field.key) }"
            >
              <div class="grid-col-label">{{ field.label }}</div>
              <div class="grid-col-old">
                {{ scope.row.originalData ? (scope.row.originalData[field.key] ?? '-') : '-' }}
              </div>
              <div class="grid-col-new">
                <span :class="{ 'highlight-change': isFieldChanged(scope.row.newData, scope.row.originalData, field.key) }">
                  {{ scope.row.newData[field.key] ?? '-' }}
                </span>
                <el-icon v-if="isFieldChanged(scope.row.newData, scope.row.originalData, field.key)" class="ml-2 text-warning">
                  <Edit />
                </el-icon>
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
          {{ getStatusLabel(scope.row.rowStatus, t) }}
        </el-tag>
      </template>
    </el-table-column>
    <el-table-column label="Summary">
      <template #default="scope">
        <span v-if="scope.row.rowStatus?.toUpperCase() === 'ERROR'" class="text-danger font-bold">
          {{ scope.row.errors?.join(', ') || t('part_upload.summary.error') }}
        </span>
      </template>
    </el-table-column>
  </el-table>
</template>

<style scoped>
/* Scoped styles preserved for component integrity (保留組件完整性的區域樣式) */
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

.comparison-row.is-changed { background: #fffdf5; }

.grid-col-label { color: #909399; font-weight: 500; }
.grid-col-old { color: #606266; padding-left: 20px; border-left: 1px solid #f1f3f5; }
.grid-col-new { color: #303133; padding-left: 20px; border-left: 1px solid #f1f3f5; display: flex; align-items: center; }

.highlight-change {
  color: #e6a23c;
  font-weight: 700;
  background: #fffbe6;
  padding: 2px 6px;
  border-radius: 4px;
}
.text-danger { color: #F56C6C; }
.text-warning { color: #E6A23C; }
.font-bold { font-weight: 700; }
.ml-2 { margin-left: 0.5rem; }
.mb-6 { margin-bottom: 1.5rem; }
</style>
