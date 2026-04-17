import { describe, it, expect } from 'vitest';
import { dashboardService } from "../../dashboard/dashboard";
import { PartStatus, MOCK_PARTS } from "../../part/part";

/**
 * Dashboard Service Unit Tests (儀表板服務單元測試)
 */
describe('dashboardService', () => {
  it('calculates status summary correctly from MOCK_PARTS (從模擬資料正確計算狀態摘要)', async () => {
    const summary = await dashboardService.getStatusSummary();
    
    const activeCount = MOCK_PARTS.filter(p => p.status === PartStatus.ACTIVE).length;
    const activeSummary = summary.find(s => s.status === PartStatus.ACTIVE);
    expect(activeSummary?.count).toBe(activeCount);

    const pendingCount = MOCK_PARTS.filter(p => p.status === PartStatus.PENDING_CUSTOMER).length;
    const pendingSummary = summary.find(s => s.status === PartStatus.PENDING_CUSTOMER);
    expect(pendingSummary?.count).toBe(pendingCount);

    expect(summary.length).toBe(7);
  });

  it('generates SLA items from pending parts (從待處理零件生成 SLA 項目)', async () => {
    const slaItems = await dashboardService.getSLAItems();
    
    expect(slaItems.length).toBeLessThanOrEqual(3);

    slaItems.forEach(item => {
      expect([PartStatus.PENDING_CUSTOMER, PartStatus.RETURNED]).toContain(item.status);
    });
  });
});
