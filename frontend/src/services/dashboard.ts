/**
 * Dashboard Service (儀表板服務)
 * BR-28: Show summary of all Part No statuses (7 states).
 * BR-29: Show SLA countdown for pending items.
 */

export enum PartStatus {
  UNKNOWN = 'UNKNOWN',
  PENDING_CUSTOMER = 'PENDING_CUSTOMER',
  PENDING_REVIEW = 'PENDING_REVIEW',
  RETURNED = 'RETURNED',
  ACTIVE = 'ACTIVE',
  FLAGGED = 'FLAGGED',
  SUPERSEDED = 'SUPERSEDED'
}

export interface StatusCount {
  status: PartStatus;
  count: number;
  labelKey: string;
  color: string;
}

export interface SLAItem {
  id: string;
  partNo: string;
  status: PartStatus;
  deadline: string; // ISO format
  remainingMinutes: number;
}

/**
 * Mock data for demonstration (展示用模擬資料)
 */
export const dashboardService = {
  /**
   * Get status summary (獲取狀態摘要)
   */
  async getStatusSummary(): Promise<StatusCount[]> {
    return [
      { status: PartStatus.UNKNOWN, count: 5, labelKey: 'status.unknown', color: '#909399' },
      { status: PartStatus.PENDING_CUSTOMER, count: 12, labelKey: 'status.pending_customer', color: '#E6A23C' },
      { status: PartStatus.PENDING_REVIEW, count: 8, labelKey: 'status.pending_review', color: '#409EFF' },
      { status: PartStatus.RETURNED, count: 3, labelKey: 'status.returned', color: '#F56C6C' },
      { status: PartStatus.ACTIVE, count: 145, labelKey: 'status.active', color: '#67C23A' },
      { status: PartStatus.FLAGGED, count: 2, labelKey: 'status.flagged', color: '#E6A23C' },
      { status: PartStatus.SUPERSEDED, count: 10, labelKey: 'status.superseded', color: '#909399' }
    ];
  },

  /**
   * Get SLA countdown items (獲取 SLA 倒數項目)
   */
  async getSLAItems(): Promise<SLAItem[]> {
    const now = new Date();
    return [
      { 
        id: '1', 
        partNo: 'PN-2024-001', 
        status: PartStatus.PENDING_CUSTOMER, 
        deadline: new Date(now.getTime() + 45 * 60000).toISOString(),
        remainingMinutes: 45
      },
      { 
        id: '2', 
        partNo: 'PN-2024-005', 
        status: PartStatus.RETURNED, 
        deadline: new Date(now.getTime() + 120 * 60000).toISOString(),
        remainingMinutes: 120
      }
    ];
  }
};
