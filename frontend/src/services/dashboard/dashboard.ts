import api from '../api';
import { PartStatus, MOCK_PARTS, partService, type PartListItem } from "../part/part";

/**
 * Dashboard Service (儀表板服務)
 * BR-28: Show summary of all Part No statuses (7 states).
 * BR-29: Show SLA countdown for pending items.
 */

export interface StatusCount {
  status: PartStatus;
  count: number;
  labelKey: string;
  color: string;
}

/**
 * Response shape from GET /api/dashboard/part-status-summary.
 * data is a single-element array: [{ s01, s02, s03, s04, s05 }]
 * (ASP.NET Core serializes property names to camelCase by default.)
 * (後端 /api/dashboard/part-status-summary 的回傳格式。)
 */
export interface PartStatusSummaryItem {
  s01: number;
  s02: number;
  s03: number;
  s04: number;
  s05: number;
}

export interface PartStatusSummaryApiResponse {
  success: boolean;
  message: string;
  data: PartStatusSummaryItem[];
}

/**
 * Single item from GET /api/dashboard/pending-review.
 * Matches backend PendingReviewDto (camelCase after ASP.NET Core serialisation).
 * (後端 PendingReviewDto 的前端對應型別，欄位已轉 camelCase。)
 */
export interface PendingReviewItem {
  id: number;
  project: string;
  partNo: string;
  partDesc: string;
  htsCode: string;
  status: string;
  updatedBy: string;
  updatedDate: string;
  slaStatus: string;
}

export interface PendingReviewApiResponse {
  success: boolean;
  message: string;
  data: PendingReviewItem[];
}

export interface SLAItem {
  id: string;
  partNo: string;
  status: PartStatus;
  deadline: string; // ISO format
  remainingMinutes: number;
}

/**
 * Helper to get color for a status (狀態顏色輔助函式)
 */
const getStatusColor = (status: PartStatus): string => {
  const colors: Record<string, string> = {
    [PartStatus.UNKNOWN]: '#909399',
    [PartStatus.PENDING_CUSTOMER]: '#E6A23C',
    [PartStatus.PENDING_REVIEW]: '#409EFF',
    [PartStatus.ACTIVE]: '#67C23A',
    [PartStatus.FLAGGED]: '#E6A23C'
  };
  return colors[status] || '#909399';
};

/**
 * The 4 statuses displayed on the Employee Dashboard, in workflow order.
 * S05 (Flagged), Inactive, and legacy aliases (RETURNED / SUPERSEDED) are excluded.
 * (儀表板顯示的 4 個核心流程狀態，依流程順序排列。)
 */
const DASHBOARD_STATUSES = [
  PartStatus.UNKNOWN,          // S01 — draft / newly created
  PartStatus.PENDING_REVIEW,   // S02 — awaiting Dimerco/DCB review
  PartStatus.PENDING_CUSTOMER, // S03 — returned to customer
  PartStatus.ACTIVE,           // S04 — reviewed & accepted
  PartStatus.FLAGGED           // S05 — flagged for attention
] as const;

/** Maps API response field names (s01…s05) to status codes (S01…S05). */
const API_KEY_TO_STATUS: Record<keyof PartStatusSummaryItem, PartStatus> = {
  s01: PartStatus.UNKNOWN,
  s02: PartStatus.PENDING_REVIEW,
  s03: PartStatus.PENDING_CUSTOMER,
  s04: PartStatus.ACTIVE,
  s05: PartStatus.FLAGGED
};

export const dashboardService = {
  /**
   * Get status summary from real API: GET /api/dashboard/part-status-summary
   * Response: { success, message, data: [{ s01, s02, s03, s04, s05 }] }
   * Falls back to MOCK_PARTS counts if the API call fails.
   * (從真實 API 取得各狀態零件數量摘要；API 失敗時 fallback 至 mock 資料。)
   * @param {string} [projectId] - Customer filter; "all" or omit for all customers.
   */
  async getStatusSummary(projectId?: string): Promise<StatusCount[]> {
    try {
      const params: Record<string, string> = {};
      if (projectId && projectId !== 'all') params.projectId = projectId;

      const response = await api.get<PartStatusSummaryApiResponse>(
        '/dashboard/part-status-summary',
        { params }
      );

      if (response.data.success && response.data.data?.length) {
        const raw = response.data.data[0]; // single-element array per spec
        return DASHBOARD_STATUSES.map(status => {
          const apiKey = (Object.keys(API_KEY_TO_STATUS) as Array<keyof PartStatusSummaryItem>)
            .find(k => API_KEY_TO_STATUS[k] === status)!;
          return {
            status,
            count: raw[apiKey] ?? 0,
            labelKey: `status.${status.toLowerCase()}`,
            color: getStatusColor(status)
          };
        });
      }
    } catch (error) {
      console.error('GET /api/dashboard/part-status-summary failed, using fallback.', error);
    }

    // Fallback: derive counts from MOCK_PARTS (API 呼叫失敗時使用 mock 計數)
    const counts: Record<string, number> = {
      [PartStatus.UNKNOWN]: 0,
      [PartStatus.PENDING_REVIEW]: 0,
      [PartStatus.PENDING_CUSTOMER]: 0,
      [PartStatus.ACTIVE]: 0,
      [PartStatus.FLAGGED]: 0
    };
    const filteredParts = projectId && projectId !== 'all'
      ? MOCK_PARTS.filter(p => p.projectId === projectId)
      : MOCK_PARTS;
    filteredParts.forEach(part => {
      if (part.status in counts) counts[part.status]++;
    });
    return DASHBOARD_STATUSES.map(status => ({
      status,
      count: counts[status] ?? 0,
      labelKey: `status.${status.toLowerCase()}`,
      color: getStatusColor(status)
    }));
  },

  /**
   * Get SLA countdown items (獲取 SLA 倒數項目)
   * Derived from parts that require attention.
   * @param {string} [projectId] - Filter by customer (按客戶篩選)
   */
  async getSLAItems(projectId?: string): Promise<SLAItem[]> {
    const now = new Date();
    
    // Filter parts by customer if provided (如果有提供則按客戶篩選)
    const baseParts = projectId && projectId !== 'all' 
      ? MOCK_PARTS.filter(p => p.projectId === projectId)
      : MOCK_PARTS;

    // Filter parts that are PENDING_CUSTOMER or RETURNED (篩選待處理項目)
    const pendingParts = baseParts.filter(p => 
      p.status === PartStatus.PENDING_CUSTOMER || p.status === PartStatus.RETURNED
    ).slice(0, 3); // Just show top 3 for demo

    return pendingParts.map((p, index) => {
      const offset = (index + 1) * 45; // Mock different deadlines
      return {
        id: p.id,
        partNo: p.partNo,
        status: p.status,
        deadline: new Date(now.getTime() + offset * 60000).toISOString(),
        remainingMinutes: offset
      };
    });
  },

  /**
   * Get parts pending review from real API: GET /api/dashboard/pending-review
   * Response: { success, message, data: [{ id, customer, partNo, partDesc, htsCode, status, updatedBy, updatedDate, slaStatus }] }
   * Falls back to empty array on error.
   * role = "CUSTOMER" → backend returns S01 + S03; omit / other → S02.
   * (從真實 API 取得待審核零件清單；API 失敗時回傳空陣列。)
   * @param {string} [projectId] - Customer filter; "all" or omit for all customers.
   * @param {string} [role]       - Caller role; "CUSTOMER" for customer portal.
   */
  async getPendingReviewParts(projectId?: string, role?: string): Promise<PendingReviewItem[]> {
    try {
      const params: Record<string, string> = {};
      if (projectId && projectId !== 'all') params.projectId = projectId;
      if (role) params.role = role;

      const response = await api.get<PendingReviewApiResponse>(
        '/dashboard/pending-review',
        { params }
      );

      if (response.data.success && Array.isArray(response.data.data)) {
        return response.data.data;
      }
    } catch (error) {
      console.error('GET /api/dashboard/pending-review failed.', error);
    }
    return [];
  }
};
