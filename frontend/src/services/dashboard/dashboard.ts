import { PartStatus, MOCK_PARTS } from "../part/part";

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
    [PartStatus.RETURNED]: '#F56C6C',
    [PartStatus.ACTIVE]: '#67C23A',
    [PartStatus.FLAGGED]: '#E6A23C',
    [PartStatus.SUPERSEDED]: '#909399'
  };
  return colors[status];
};

export const dashboardService = {
  /**
   * Get status summary (動態計算狀態摘要)
   * BR-28: Counts are derived from MOCK_PARTS.
   */
  async getStatusSummary(): Promise<StatusCount[]> {
    // 1. Initialize counts for all 7 states (初始化 7 種狀態的計數)
    const counts: Record<PartStatus, number> = {
      [PartStatus.UNKNOWN]: 0,
      [PartStatus.PENDING_CUSTOMER]: 0,
      [PartStatus.PENDING_REVIEW]: 0,
      [PartStatus.RETURNED]: 0,
      [PartStatus.ACTIVE]: 0,
      [PartStatus.FLAGGED]: 0,
      [PartStatus.SUPERSEDED]: 0
    };

    // 2. Count occurrences (計算出現次數)
    MOCK_PARTS.forEach(part => {
      counts[part.status]++;
    });

    // 3. Map to StatusCount array (映射至 StatusCount 陣列)
    return Object.values(PartStatus).map(status => ({
      status,
      count: counts[status],
      labelKey: `status.${status.toLowerCase()}`,
      color: getStatusColor(status)
    }));
  },

  /**
   * Get SLA countdown items (獲取 SLA 倒數項目)
   * Derived from parts that require attention.
   */
  async getSLAItems(): Promise<SLAItem[]> {
    const now = new Date();
    // Filter parts that are PENDING_CUSTOMER or RETURNED (篩選待處理項目)
    const pendingParts = MOCK_PARTS.filter(p => 
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
  }
};
