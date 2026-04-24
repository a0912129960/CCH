import api from '../api';
import { commonService } from '../common/common';

/**
 * Part Status (零件狀態)
 */
export const PartStatus = {
  UNKNOWN: 'S01',
  PENDING_REVIEW: 'S02',
  PENDING_CUSTOMER: 'S03',
  ACTIVE: 'S04',
  FLAGGED: 'S05',
  INACTIVE: '',
  // Legacy mappings for compatibility during transition (保留舊名稱以維持過渡期相容性)
  RETURNED: 'S03',
  SUPERSEDED: ''
} as const;
export type PartStatus = typeof PartStatus[keyof typeof PartStatus];

export interface PartHistory {
  id: string;
  status: PartStatus;
  updatedBy: string;
  updatedAt: string;
  remark?: string;
}

export interface Part {
  id: string;
  division: string;
  partNo: string;
  countryOfOrigin?: string;
  partDescription?: string;
  usHtsCode?: string;
  generalDutyRate?: number | string;
  htsCode301?: string;
  rate301?: string;
  htsCodeIeepa?: string;
  rateIeepa?: string;
  htsCode232Aluminum?: string;
  rate232Aluminum?: string;
  htsCodeReciprocalTariff?: string;
  rateReciprocalTariff?: string;
  remark?: string;
  description?: string;
  htsCode: string;
  status: PartStatus;
  updatedBy: string;
  lastUpdated: string;
  slaDeadline?: string; // ISO format
  supplier?: string; // Added to match mock data
  
  // Expanded Duties
  duty301?: { code: string; rate: string };
  dutyIEEPA?: { code: string; rate: string };
  duty232Aluminum?: { code: string; rate: string };
  dutyReciprocal?: { code: string; rate: string };

  projectId: string;
  projectName: string;
  dimercoRemark?: string;
  replacementCode?: string;
  history?: PartHistory[];
}

/**
 * Internal Helper to generate random history (生成隨機歷史記錄的輔助函式)
 */
const generateHistory = (status: PartStatus, date: string): PartHistory[] => {
  const history: PartHistory[] = [
    { id: 'h-init', status: PartStatus.UNKNOWN, updatedBy: 'System', updatedAt: '2026-01-01 09:00' }
  ];
  
  if (status !== PartStatus.UNKNOWN) {
    history.push({ id: 'h-pend', status: PartStatus.PENDING_CUSTOMER, updatedBy: 'Supplier API', updatedAt: '2026-01-05 10:00' });
  }
  
  if (status === PartStatus.ACTIVE || status === PartStatus.RETURNED || status === PartStatus.PENDING_REVIEW) {
    history.push({ id: 'h-rev', status: PartStatus.PENDING_REVIEW, updatedBy: 'Project A', updatedAt: '2026-01-10 14:00' });
  }

  history.push({ id: 'h-curr', status, updatedBy: 'Dimerco Admin', updatedAt: date, remark: 'Status updated via portal.' });
  return history;
};

/**
 * Expanded Mock data for Parts (擴充後的零件模擬資料)
 */
export const MOCK_PARTS: Part[] = [
  // 1-10 (Project 001)
  { id: '1', partNo: 'PN-2024-001', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-08 14:30', description: '5G Comm Module', history: generateHistory(PartStatus.ACTIVE, '2026-04-08 14:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '2', partNo: 'PN-2024-002', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-07 09:15', description: 'Portable Laptop Unit', history: generateHistory(PartStatus.ACTIVE, '2026-04-07 09:15'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '3', partNo: 'PN-2024-003', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-09 10:00', description: 'Router Switch', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 10:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '4', partNo: 'PN-2024-004', htsCode: '8471.50.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-05 16:45', description: 'Server Blade', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-05 16:45'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '5', partNo: 'PN-2024-005', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-01 11:20', description: 'LCD Monitor', history: generateHistory(PartStatus.ACTIVE, '2026-04-01 11:20'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '6', partNo: 'PN-2024-006', htsCode: '8517.12.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-09 08:00', description: 'Generic Phone Part', history: generateHistory(PartStatus.UNKNOWN, '2026-04-09 08:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '7', partNo: 'PN-2024-007', htsCode: '8517.12.00', status: PartStatus.RETURNED, supplier: 'TechCorp Solutions', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-09 11:00', description: 'Network Adapter', dimercoRemark: 'Incorrect HTS classification.', replacementCode: '8517.62.00', history: generateHistory(PartStatus.RETURNED, '2026-04-09 11:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '8', partNo: 'PN-2024-008', htsCode: '8517.62.00', status: PartStatus.FLAGGED, supplier: 'TechCorp Solutions', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-04-09 10:30', description: 'Wireless Receiver', history: generateHistory(PartStatus.FLAGGED, '2026-04-09 10:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '9', partNo: 'PN-2024-009', htsCode: '8471.50.00', status: PartStatus.SUPERSEDED, supplier: 'Alpha Manufacturing', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-03-30 11:00', description: 'Old Processor Model', history: generateHistory(PartStatus.SUPERSEDED, '2026-03-30 11:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '10', partNo: 'PN-2024-010', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', projectId: 'project001', projectName: 'Dimerco Electronics', lastUpdated: '2026-03-28 10:00', description: 'Base Station Component', history: generateHistory(PartStatus.ACTIVE, '2026-03-28 10:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  
  // 11-20 (Project 002)
  { id: '11', partNo: 'PN-G-011', htsCode: '8471.50.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-03-25 15:30', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-03-25 15:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '12', partNo: 'PN-G-012', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-03-20 09:00', history: generateHistory(PartStatus.ACTIVE, '2026-03-20 09:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '13', partNo: 'PN-G-013', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-03-15 14:20', history: generateHistory(PartStatus.ACTIVE, '2026-03-15 14:20'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '14', partNo: 'PN-G-014', htsCode: '8471.30.00', status: PartStatus.PENDING_REVIEW, supplier: 'TechCorp Solutions', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-03-10 11:45', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-03-10 11:45'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '15', partNo: 'PN-G-015', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-03-05 16:10', history: generateHistory(PartStatus.ACTIVE, '2026-03-05 16:10'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '16', partNo: 'PN-G-016', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-03-01 10:30', history: generateHistory(PartStatus.ACTIVE, '2026-03-01 10:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '17', partNo: 'PN-G-017', htsCode: '8517.12.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-04-09 09:30', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 09:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '18', partNo: 'PN-G-018', htsCode: '8471.30.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Alpha Manufacturing', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-04-09 08:45', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 08:45'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '19', partNo: 'PN-G-019', htsCode: '8528.52.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-04-09 07:15', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 07:15'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '20', partNo: 'PN-G-020', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', projectId: 'project002', projectName: 'Global Tech Solutions', lastUpdated: '2026-04-08 17:00', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-08 17:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },

  // 21-30 (Project 003)
  { id: '21', partNo: 'PN-A-021', htsCode: '8517.12.00', status: PartStatus.PENDING_REVIEW, supplier: 'TechCorp Solutions', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-04-08 11:20', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-08 11:20'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '22', partNo: 'PN-A-022', htsCode: '8471.30.00', status: PartStatus.PENDING_REVIEW, supplier: 'Global Logistics Inc', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-04-07 13:40', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-07 13:40'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '23', partNo: 'PN-A-023', htsCode: '8528.52.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-04-06 15:55', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-06 15:55'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '24', partNo: 'PN-A-024', htsCode: '8517.62.00', status: PartStatus.RETURNED, supplier: 'Global Logistics Inc', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-04-08 10:30', dimercoRemark: 'Supporting docs required.', history: generateHistory(PartStatus.RETURNED, '2026-04-08 10:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '25', partNo: 'PN-A-025', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-02-28 13:45', history: generateHistory(PartStatus.ACTIVE, '2026-02-28 13:45'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '26', partNo: 'PN-A-026', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-02-25 09:20', history: generateHistory(PartStatus.ACTIVE, '2026-02-25 09:20'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '27', partNo: 'PN-A-027', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-02-20 15:15', history: generateHistory(PartStatus.ACTIVE, '2026-02-20 15:15'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '28', partNo: 'PN-A-028', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-02-15 11:30', history: generateHistory(PartStatus.ACTIVE, '2026-02-15 11:30'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '29', partNo: 'PN-A-029', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-02-10 14:00', history: generateHistory(PartStatus.ACTIVE, '2026-02-10 14:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' },
  { id: '30', partNo: 'PN-A-030', htsCode: '8471.50.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', projectId: 'project003', projectName: 'Alpha Systems Corp', lastUpdated: '2026-04-07 09:00', history: generateHistory(PartStatus.UNKNOWN, '2026-04-07 09:00'), division: '', countryOfOrigin: '', generalDutyRate: 0, updatedBy: '' }
];

// INTERNAL-AI-20260417: New interfaces for the list view matching the latest backend response.
// (INTERNAL-AI-20260417: 對應最新後端回應的零件清單介面。)
export interface PartListItem {
  id: number;
  project: string;
  partNo: string;
  partDesc: string;
  country: string;
  htsCode: string;
  rate: number;
  supplier: string;
  status: string; // S01 | S02 | S03 | S04 | Inactive
  updatedBy: string;
  updatedDate: string;
  slaStatus: string;
  htsCode1?: string | null;
  rate1?: number | null;
  htsCode2?: string | null;
  rate2?: number | null;
  htsCode3?: string | null;
  rate3?: number | null;
  htsCode4?: string | null;
  rate4?: number | null;
}

/**
 * Paginated response for GET /api/parts.
 * (GET /api/parts 的分頁回應結構。)
 */
export interface PartListApiResponse {
  total: number;
  page: number;
  data: PartListItem[];
}

/**
 * Map backend status codes to i18n keys for display.
 * (將後端狀態碼對應至前端 i18n 顯示鍵值。)
 */
export const statusToI18nKey = (status: string): string => {
  const map: Record<string, string> = {
    S01: 'unknown',
    S02: 'pending_review',
    S03: 'pending_customer',
    S04: 'active',
    Inactive: 'superseded',
  };
  return map[status] ?? 'unknown';
};

/**
 * Map backend status codes to display colors.
 * (將後端狀態碼對應至顯示顏色。)
 */
export const statusToColor = (status: string): string => {
  const map: Record<string, string> = {
    S01: '#909399',
    S02: '#409EFF',
    S03: '#E6A23C',
    S04: '#67C23A',
    Inactive: '#909399',
  };
  return map[status] ?? '#909399';
};

/**
 * Fetch paginated part list from the real backend API GET /api/parts.
 * (從後端 GET /api/parts 取得分頁零件清單。)
 *
 * INTERNAL-AI-20260416
 */
export async function searchParts(params?: {
  projectId?: string;
  status?: string;
  partNo?: string;
  supplier?: string;
  page?: number;
  pageSize?: number;
}): Promise<PartListApiResponse> {
  const response = await api.get<{ success: boolean; data: PartListApiResponse }>('/parts', { params });
  return response.data.data;
}

// INTERNAL-AI-20260416: New interfaces for the real GET /api/parts/{partId} response.
// (INTERNAL-AI-20260416: 新增對應後端 GET /api/parts/{partId} 回應的 TypeScript 介面。)

/**
 * Part detail field set matching backend PartDetailDto.
 * (對應後端 PartDetailDto 的零件詳細欄位集合。)
 */
export interface PartDetailFields {
  project?: string;
  partNo: string;
  countryId?: number | null;
  country: string;
  division: string;
  supplier: string;
  partDesc: string;
  htsCode: string;
  rate: number;
  htsCode1?: string | null;
  rate1?: number | null;
  htsCode2?: string | null;
  rate2?: number | null;
  htsCode3?: string | null;
  rate3?: number | null;
  htsCode4?: string | null;
  rate4?: number | null;
  remark: string;
  updatedBy: string;
  updatedDate: string;
  isHTSExists?: boolean | null;
}

/**
 * Response for GET /api/parts/{partId} matching backend PartDetailResponseDto.
 * (對應後端 PartDetailResponseDto 的零件詳細回應結構。)
 */
export interface PartDetailResponse {
  // INTERNAL-AI-20260416: Added status field for status badge display in the detail view.
  // (INTERNAL-AI-20260416: 新增 status 欄位供詳細頁面顯示狀態標籤使用。)
  status: string;
  // INTERNAL-AI-20260420: Added slaStatus for SLA-based badge color in the detail view.
  // (INTERNAL-AI-20260420: 新增 slaStatus 供詳細頁面以 SLA 狀態套用標籤顏色。)
  slaStatus: string;
  before: PartDetailFields;
  modified: PartDetailFields;
}

/**
 * Fetch part detail from the real backend API.
 * (從後端 API 取得零件詳細資料。)
 * Returns null when the part is not found (404). (零件不存在時回傳 null。)
 *
 * INTERNAL-AI-20260416
 */
export async function getPartDetail(partId: number): Promise<PartDetailResponse | null> {
  try {
    const response = await api.get<{ success: boolean; data: PartDetailResponse }>(`/parts/${partId}`);
    return response.data.data;
  } catch (error: any) {
    // 404 means part does not exist; caller should handle redirect. (404 表示零件不存在，由呼叫方處理跳轉。)
    if (error?.response?.status === 404) return null;
    throw error;
  }
}

// INTERNAL-AI-20260416: Payload type for PUT /api/parts/{partId}.
// (INTERNAL-AI-20260416: PUT /api/parts/{partId} 的請求資料型別。)

/**
 * Payload for saving/updating a part (PUT /api/parts/{partId}).
 * (儲存/更新零件的請求資料，對應後端 PartSaveRequest。)
 */
export interface PartSavePayload {
  partNo: string;
  countryId: number | null;
  division: string;
  // INTERNAL-AI-20260420: Changed back to supplier string (free-text) per spec; backend now accepts string.
  // (INTERNAL-AI-20260420: 依規格改回供應商字串（自由輸入），後端已接受字串格式。)
  /* supplierId: number | null; */
  supplier: string;
  partDesc: string;
  htsCode: string;
  rate: number;
  htsCode1?: string | null;
  rate1?: number | null;
  htsCode2?: string | null;
  rate2?: number | null;
  htsCode3?: string | null;
  rate3?: number | null;
  htsCode4?: string | null;
  rate4?: number | null;
  remark?: string;
  isHTSExists?: boolean | null;
}

/**
 * Call PUT /api/parts/{partId} to save part data.
 * (呼叫 PUT /api/parts/{partId} 儲存零件資料。)
 * Throws on validation error (400) or other errors. (驗證失敗 400 或其他錯誤時拋出例外。)
 *
 * INTERNAL-AI-20260416
 */
export async function updatePart(partId: number, payload: PartSavePayload): Promise<void> {
  await api.put(`/parts/${partId}`, payload);
}

/**
 * Call POST /api/parts/{partId}/submit to save and send part to Dimerco for review.
 * (呼叫 POST /api/parts/{partId}/submit 儲存並送審給 Dimerco。)
 * Returns { partId, status: 'S02' } on success. (成功時回傳 { partId, status: 'S02' }。)
 *
 * INTERNAL-AI-20260416
 */
export async function submitPart(partId: number, payload: PartSavePayload): Promise<void> {
  await api.post(`/parts/${partId}/submit`, payload);
}

// INTERNAL-AI-20260416: Milestone types and API functions for the detail view timeline.
// (INTERNAL-AI-20260416: 詳細頁面時間軸所需的里程碑型別與 API 函式。)

/**
 * Single milestone item matching backend MilestoneDto.
 * (對應後端 MilestoneDto 的里程碑項目。)
 */
export interface Milestone {
  action: string;
  updatedBy: string;
  updatedDate: string;
  remark: string;
}

/**
 * Fetch milestones for a part from GET /api/parts/{partId}/milestones.
 * (從後端 GET /api/parts/{partId}/milestones 取得里程碑清單。)
 */
export async function getMilestones(partId: number): Promise<Milestone[]> {
  const response = await api.get<{ success: boolean; data: Milestone[] }>(`/parts/${partId}/milestones`);
  return response.data.data;
}

/**
 * Accept multiple parts classification via POST /api/parts/batch-accept.
 * (透過 POST /api/parts/batch-accept 批量接受零件分類。)
 * 
 * INTERNAL-AI-20260417
 */
export async function batchAcceptParts(partIds: number[]): Promise<{ success: boolean, message: string, data: { partId: string, errorMessage: string }[] }> {
  const response = await api.post<{ success: boolean, message: string, data: { partId: string, errorMessage: string }[] }>(`/parts/batch-accept`, partIds);
  return response.data;
}

/**
 * Accept a part classification via POST /api/parts/{partId}/accept (DCB only).
 * (DCB 角色接受零件分類，呼叫 POST /api/parts/{partId}/accept。)
 */
export async function acceptPart(partId: number): Promise<void> {
  await api.post(`/parts/${partId}/accept`);
}

/**
 * Return a part to the customer via POST /api/parts/{partId}/return (DCB only).
 * (DCB 角色退回零件給客戶，呼叫 POST /api/parts/{partId}/return。)
 */
// INTERNAL-AI-20260420: Changed body field from { reason } to { returnReason } per API spec.
// (INTERNAL-AI-20260420: 依 API 規格將請求主體欄位由 reason 改為 returnReason。)
export async function returnPart(partId: number, reason: string): Promise<void> {
  await api.post(`/parts/${partId}/return`, { returnReason: reason });
}

// INTERNAL-AI-20260420: Added inactivatePart for Customer role Inactive button.
// (INTERNAL-AI-20260420: 新增 inactivatePart 供 Customer 角色的停用按鈕使用。)
/**
 * Mark a part as inactive via POST /api/parts/{partId}/inactive (Customer only).
 * (Customer 角色將零件設為停用，呼叫 POST /api/parts/{partId}/inactive。)
 */
export async function inactivatePart(partId: number): Promise<void> {
  await api.post(`/parts/${partId}/inactive`);
}

// INTERNAL-AI-20260421: S04 → S03 for Dimerco/Customer: save + set Pending Customer Review.
// (INTERNAL-AI-20260421: S04 → S03，Dimerco/Customer 儲存後將狀態改為 Pending Customer Review。)
/**
 * Save part data and transition to Pending Customer Review (S03) via POST /api/parts/{partId}/send-to-customer-review.
 * (儲存零件資料並將狀態改為 S03 Pending Customer Review。)
 */
export async function sendToCustomerReview(partId: number, payload: PartSavePayload): Promise<void> {
  await api.post(`/parts/${partId}/send-to-customer-review`, payload);
}

export interface HtsRecommendationResult {
  input_hts_code: string;
  matched_keyword: string | null;
  fallback_used: boolean;
  message?: string;
  data?: {
    general: string | null;
    special: string | null;
    other: string | null;
    description: string | null;
  };
}

/**
 * Query USITC HTS recommendation for a given HTS code (10-digit or 8-digit fallback).
 * Returns null on any error so the caller can silently skip.
 * (依 HTS Code 查詢 USITC 關稅建議；任何錯誤都回傳 null，讓呼叫方靜默跳過。)
 */
export async function getHtsRecommendation(formattedHtsCode: string): Promise<HtsRecommendationResult | null> {
  try {
    const rawCode = formattedHtsCode.replace(/\./g, '');
    const response = await api.get<{ success: boolean; data: HtsRecommendationResult }>(`/hts-recommendation/${rawCode}`);
    return response.data.data ?? null;
  } catch {
    return null;
  }
}

/**
 * Fetch snapshot history for a part (GET /api/parts/{partId}/history).
 * Returns an array of PartDetailFields ordered newest-first.
 * (取得零件的快照歷史，依最新到最舊排序。)
 */
export async function getHistory(partId: number): Promise<PartDetailFields[]> {
  try {
    const response = await api.get<{ success: boolean; data: PartDetailFields[] }>(`/parts/${partId}/history`);
    return response.data.data ?? [];
  } catch {
    return [];
  }
}

/**
 * Import Result Status (匯入結果狀態)
 */
export const ImportResultStatus = {
  NEW: 'NEW',
  UPDATED: 'UPDATED',
  UNCHANGED: 'UNCHANGED',
  REJECTED: 'REJECTED'
} as const;
export type ImportResultStatus = typeof ImportResultStatus[keyof typeof ImportResultStatus];

export interface ImportRowReport {
  partNo: string;
  htsCode: string;
  status: ImportResultStatus;
  message?: string;
}

export interface ImportBatchReport {
  batchId: string;
  totalRows: number;
  newCount: number;
  updatedCount: number;
  unchangedCount: number;
  rejectedCount: number;
  rows: ImportRowReport[];
}

/**
 * Create Part API Request Body — POST /api/parts ([Save] button).
 * Only partNo and countryId are required; all other fields are optional.
 * (新增零件 API 請求參數 — [儲存] 按鈕。只有 partNo 與 countryId 必填。)
 */
export interface CreatePartRequest {
  projectId?: string;
  partNo: string;           // Required / 必填
  countryId: string | number; // Required / 必填
  division?: string;
  supplier?: string;
  partDesc?: string;
  htsCode?: string;
  rate?: number;
  htsCode1?: string;
  rate1?: number;
  htsCode2?: string;
  rate2?: number;
  htsCode3?: string;
  rate3?: number;
  htsCode4?: string;
  rate4?: number;
  remark?: string;
}

/**
 * Submit Part API Request Body — POST /api/parts/submit ([Save & Submit] button).
 * partNo, countryId, division, supplier, partDesc, and htsCode are all required.
 * (送審零件 API 請求參數 — [儲存並送審] 按鈕。六個核心欄位全部必填。)
 */
export interface SubmitPartRequest {
  projectId?: string;
  partNo: string;           // Required / 必填
  countryId: number;        // Required / 必填
  division: string;         // Required / 必填
  supplier: string;         // Required / 必填
  partDesc: string;         // Required / 必填
  htsCode: string;          // Required / 必填
  rate?: number;
  htsCode1?: string;
  rate1?: number;
  htsCode2?: string;
  rate2?: number;
  htsCode3?: string;
  rate3?: number;
  htsCode4?: string;
  rate4?: number;
  remark?: string;
}

/**
 * Create Part API Response Data (新增零件 API 回傳資料)
 */
export interface CreatePartResponse {
  success: boolean;
  message: string;
  data: {
    partId: string;
    partNo: string;
    status: string;
  };
}

/**
 * Submit Part API Response Data (提交零件 API 回傳資料)
 */
export interface SubmitPartResponse {
  success: boolean;
  message: string;
  data: {
    partId: string;
    partNo: string;
    status: 'S02';
  };
}

export interface PartListResponse {
  total: number;
  page: number;
  data: PartListItem[];
}

/**
 * Bulk Upload Preview Types (批量上傳預覽型別)
 */
export interface BulkUploadPartData {
  id?: number;
  projectId: number;
  partNo: string;
  countryId?: number;
  country?: string;
  division?: string;
  supplier?: string;
  supplierId?: number;
  partDesc?: string;
  htsCode?: string;
  rate?: number;
  htsCode1?: string;
  rate1?: number;
  htsCode2?: string;
  rate2?: number;
  htsCode3?: string;
  rate3?: number;
  htsCode4?: string;
  rate4?: number;
  remark?: string;
  status?: string;
}

export interface BulkUploadPreviewRow {
  rowIndex: number;
  rowStatus: string; // e.g., 'NEW', 'MODIFIED', 'ERROR', 'NO_CHANGE'
  errors: string[];
  newData: BulkUploadPartData;
  originalData?: BulkUploadPartData | null;
}

export interface BulkUploadPreviewReport {
  summary: {
    totalRows: number;
    newCount: number;
    modifiedCount: number;
    errorCount: number;
    noChangeCount: number;
  };
  rows: BulkUploadPreviewRow[];
}

export const partService = {
  /**
   * Bulk Upload Preview (批量上傳預覽)
   * INTERNAL-AI-20260420: Call preview API and return comparison report.
   * (INTERNAL-AI-20260420: 呼叫預覽 API 並回傳比對報告。)
   */
  async previewBulkUpload(file: File, projectId: number): Promise<BulkUploadPreviewReport> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('projectId', projectId.toString());

    const response = await api.post<{ success: boolean; data: BulkUploadPreviewReport; message?: string }>(
      '/parts/bulk-upload/preview',
      formData
    );

    if (response.data.success) {
      return response.data.data;
    }
    throw new Error(response.data.message || 'Preview failed');
  },

  /**
   * Bulk Upload Confirm (批量上傳確認)
   * INTERNAL-AI-20260421: Call confirm API to save previewed data.
   * (INTERNAL-AI-20260421: 呼叫確認 API 以儲存預覽資料。)
   */
  async confirmBulkUpload(data: BulkUploadPartData[]): Promise<any> {
    const response = await api.post<{ success: boolean; data: any; message?: string }>(
      '/parts/bulk-upload/confirm',
      data
    );

    if (response.data.success) {
      return response.data.data;
    }
    throw new Error(response.data.message || 'Bulk upload failed');
  },

  /**
   * Fetch parts list with pagination and search (獲取分頁與搜尋的零件清單)
   * Updated by Gemini AI on 2026-04-17 (INTERNAL-AI-20260417)
   */
  async getParts(params?: {
    projectId?: string;
    status?: string;
    partNo?: string;
    supplier?: string;
    page?: number;
    pageSize?: number;
  }): Promise<PartListResponse> {
    try {
      const response = await api.get<{ success: boolean; data: PartListResponse }>('/parts', { params });
      if (response.data.success) {
        return response.data.data;
      }
      return { total: 0, page: 1, data: [] };
    } catch (error) {
      console.error('API /parts failed. (API /parts 失敗。)', error);
      return { total: 0, page: 1, data: [] };
    }
  },

  /**
   * Get Projects (獲取專案清單)
   * (繁體中文) 從通用服務獲取專案清單並轉換格式。
   */
  async getProjects(): Promise<{ id: string; name: string }[]> {
    const projects = await commonService.getProjects();
    return projects.map(p => ({ id: p.key, name: p.value }));
  },

  async getPartById(id: string): Promise<Part | undefined> {
    try {
      const response = await api.get<{ success: boolean; data: Part }>(`/parts/${id}`);
      return response.data.success ? response.data.data : undefined;
    } catch (error) {
      console.error(`API /parts/${id} failed.`, error);
      return undefined;
    }
  },
  async getSuppliers(): Promise<string[]> {
    // INTERNAL-AI-20260416: Redirect to commonService or real parts supplier API if exists
    return [];
  },
  async createPart(data: {
    partNo: string;
    countryOfOrigin?: string;
    division?: string;
    partDescription?: string;
    usHtsCode?: string;
    generalDutyRate?: number | string;
    htsCode301?: string;
    rate301?: string;
    htsCodeIeepa?: string;
    rateIeepa?: string;
    htsCode232Aluminum?: string;
    rate232Aluminum?: string;
    htsCodeReciprocalTariff?: string;
    rateReciprocalTariff?: string;
    remark?: string;
    description?: string;
    htsCode?: string;
    supplier: string;
    projectId?: string;
    projectName?: string;
    status?: PartStatus;
  }): Promise<Part> {
    const newPart: Part = {
      id: (MOCK_PARTS.length + 1).toString(),
      partNo: data.partNo,
      countryOfOrigin: data.countryOfOrigin,
      division: data.division || '',
      partDescription: data.partDescription,
      usHtsCode: data.usHtsCode,
      generalDutyRate: data.generalDutyRate,
      htsCode301: data.htsCode301,
      rate301: data.rate301,
      htsCodeIeepa: data.htsCodeIeepa,
      rateIeepa: data.rateIeepa,
      htsCode232Aluminum: data.htsCode232Aluminum,
      rate232Aluminum: data.rate232Aluminum,
      htsCodeReciprocalTariff: data.htsCodeReciprocalTariff,
      rateReciprocalTariff: data.rateReciprocalTariff,
      remark: data.remark,
      htsCode: data.htsCode || '',
      status: data.status || PartStatus.PENDING_REVIEW,
      supplier: data.supplier || 'Unknown Source',
      projectId: data.projectId || 'project001',
      projectName: data.projectName || 'Dimerco Electronics',
      updatedBy: '',
      lastUpdated: new Date().toISOString().replace('T', ' ').substring(0, 16),
      description: data.description,
      history: [
        {
          id: 'h-new',
          status: data.status || PartStatus.PENDING_REVIEW,
          updatedBy: data.status === PartStatus.ACTIVE ? 'Dimerco Employee' : 'Project A',
          updatedAt: new Date().toISOString().replace('T', ' ').substring(0, 16),
          remark: data.status === PartStatus.ACTIVE ? 'Part created and auto-approved by employee.' : 'Part created and submitted for review.'
        }
      ]
    };
    MOCK_PARTS.unshift(newPart);
    return newPart;
  },
  /**
   * Create Part via real API  POST /api/parts
   */
  /**
   * Check whether a PartNo + CountryId combination already exists for the given customer.
   * Calls GET /api/parts/check-duplicate
   * (檢查指定客戶下 PartNo + CountryId 組合是否重複)
   */
  async checkDuplicate(customerId: string | number, partNo: string, countryId: string | number): Promise<boolean> {
    try {
      const response = await api.get<{ success: boolean; data: { isDuplicate: boolean } }>(
        '/parts/check-duplicate',
        { params: { customerId, partNo, countryId } }
      );
      return response.data?.data?.isDuplicate === true;
    } catch {
      // On error, allow the save to proceed — the backend will reject it anyway.
      return false;
    }
  },

  async createPartApi(body: CreatePartRequest): Promise<CreatePartResponse> {
    const response = await api.post<CreatePartResponse>('/parts', body);
    return response.data;
  },

  /**
   * Submit Part to Dimerco via real API  POST /api/parts/submit
   * Requires all 6 core fields: partNo, countryId, division, supplier, partDesc, htsCode.
   * (送審零件至 Dimerco。六個核心欄位必填。)
   */
  async submitPartApi(body: SubmitPartRequest): Promise<SubmitPartResponse> {
    const response = await api.post<SubmitPartResponse>('/parts/submit', body);
    return response.data;
  },

  async updatePartStatus(id: string, newStatus: PartStatus, remark?: string): Promise<boolean> {
    const response = await api.patch<{ success: boolean }>(`/parts/${id}/status`, { status: newStatus, remark });
    return response.data.success;
  },
  
  /**
   * Bulk Upload Methods (批量上傳方法)
   */
  async downloadTemplate(): Promise<void> {
    /* window.location.href = `${api.defaults.baseURL}/parts/template`; */
    try {
      // INTERNAL-AI-20260420: Call new bulk-upload template endpoint and handle blob response.
      // (INTERNAL-AI-20260420: 呼叫新的批量上傳範本端點並處理 Blob 回應。)
      const response = await api.get('/parts/bulk-upload/template', {
        responseType: 'blob'
      });

      // Create download link (建立下載連結)
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', 'PartBulkUploadTemplate.xlsx');
      document.body.appendChild(link);
      link.click();
      
      // Cleanup (清理)
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Template download failed (範本下載失敗):', error);
      throw error;
    }
  },

  async uploadParts(file: File, projectId?: string, onProgress?: (percent: number) => void): Promise<ImportBatchReport> {
    const formData = new FormData();
    formData.append('file', file);
    if (projectId) formData.append('projectId', projectId);

    const response = await api.post<{ success: boolean; data: ImportBatchReport }>('/parts/upload', formData, {
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(percent);
        }
      }
    });

    if (response.data.success) {
      return response.data.data;
    }
    throw new Error('Upload failed');
  },

  /**
   * Export Parts to Excel (匯出零件至 Excel)
   * (繁體中文) 根據當前篩選條件呼稱後端匯出 API。
   */
  /**
   * Export Parts to Excel (匯出零件至 Excel)
   * (繁體中文) 使用 Blob 處理二進位流並觸發瀏覽器下載。
   */
  async exportPartsToExcel(params?: {
    projectId?: string;
    status?: string;
    partNo?: string;
    supplier?: string;
  }): Promise<void> {
    try {
      const response = await api.get('/parts/export', {
        params,
        responseType: 'blob' // 重要：指定回傳格式為二進位 (Binary)
      });

      // 建立下載連結 (Create download link)
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `PartList_${new Date().toISOString().slice(0, 10)}.xlsx`);
      document.body.appendChild(link);
      link.click();
      
      // 清理 (Cleanup)
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Excel export failed (Excel 匯出失敗):', error);
      throw error;
    }
  }
};
