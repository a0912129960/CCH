import { type BulkUploadPartData } from '@src/services/part/part';

/**
 * Bulk Upload Utility Helpers (批量上傳輔助工具)
 * Created by Gemini AI on 2026-04-21 per Supreme Quality Mandate.
 * (INTERNAL-AI-20260421: 依最高品質授權建立。)
 */

/**
 * Field Mapping for Comparison (對比欄位映射)
 */
export const comparisonFields: { key: keyof BulkUploadPartData; label: string }[] = [
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

/**
 * Helper to check if a field has changed. (比對欄位是否變更的輔助函式。)
 */
export const isFieldChanged = (newData: BulkUploadPartData, originalData: BulkUploadPartData | null | undefined, field: keyof BulkUploadPartData) => {
  if (!originalData) return false;
  return newData[field] !== originalData[field];
};

/**
 * Get localized status label. (獲取本地化狀態標籤。)
 */
export const getStatusLabel = (status: string, t: any) => {
  const s = status?.toUpperCase();
  switch (s) {
    case 'NEW': return t('part_upload.summary.new');
    case 'MODIFIED': return t('part_upload.summary.modified');
    case 'ERROR': return t('part_upload.summary.error');
    case 'NOCHANGE': return t('part_upload.summary.nochange');
    default: return s;
  }
};

/**
 * Get UI type for status tags. (獲取狀態標籤的 UI 類型。)
 */
export const getRowStatusType = (status: string) => {
  const s = status?.toUpperCase();
  switch (s) {
    case 'NEW': return 'success';
    case 'MODIFIED': return 'warning';
    case 'NOCHANGE': return 'info';
    case 'ERROR': return 'danger';
    default: return 'info';
  }
};
