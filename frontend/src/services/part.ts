/**
 * Part Status Enum (零件狀態枚舉)
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

/**
 * Part Interface (零件介面)
 */
export interface Part {
  id: string;
  partNo: string;
  htsCode: string;
  status: PartStatus;
  supplier: string;
  lastUpdated: string;
}

/**
 * Expanded Mock data for Parts (擴充後的零件模擬資料)
 * Total: 30 items
 */
export const MOCK_PARTS: Part[] = [
  // ACTIVE (15 items)
  { id: '1', partNo: 'PN-2024-001', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-08 14:30' },
  { id: '2', partNo: 'PN-2024-002', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-07 09:15' },
  { id: '5', partNo: 'PN-2024-005', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-01 11:20' },
  { id: '10', partNo: 'PN-2024-010', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-03-28 10:00' },
  { id: '11', partNo: 'PN-2024-011', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-25 15:30' },
  { id: '12', partNo: 'PN-2024-012', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-20 09:00' },
  { id: '13', partNo: 'PN-2024-013', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-03-15 14:20' },
  { id: '14', partNo: 'PN-2024-014', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-03-10 11:45' },
  { id: '15', partNo: 'PN-2024-015', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-03-05 16:10' },
  { id: '16', partNo: 'PN-2024-016', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-01 10:30' },
  { id: '25', partNo: 'PN-2024-025', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-02-28 13:45' },
  { id: '26', partNo: 'PN-2024-026', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-02-25 09:20' },
  { id: '27', partNo: 'PN-2024-027', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-02-20 15:15' },
  { id: '28', partNo: 'PN-2024-028', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-02-15 11:30' },
  { id: '29', partNo: 'PN-2024-029', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-02-10 14:00' },

  // PENDING_CUSTOMER (5 items)
  { id: '3', partNo: 'PN-2024-003', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 10:00' },
  { id: '17', partNo: 'PN-2024-017', htsCode: '8517.12.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-09 09:30' },
  { id: '18', partNo: 'PN-2024-018', htsCode: '8471.30.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Alpha Manufacturing', lastUpdated: '2026-04-09 08:45' },
  { id: '19', partNo: 'PN-2024-019', htsCode: '8528.52.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 07:15' },
  { id: '20', partNo: 'PN-2024-020', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-08 17:00' },

  // PENDING_REVIEW (4 items)
  { id: '4', partNo: 'PN-2024-004', htsCode: '8471.50.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', lastUpdated: '2026-04-05 16:45' },
  { id: '21', partNo: 'PN-2024-021', htsCode: '8517.12.00', status: PartStatus.PENDING_REVIEW, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-08 11:20' },
  { id: '22', partNo: 'PN-2024-022', htsCode: '8471.30.00', status: PartStatus.PENDING_REVIEW, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-07 13:40' },
  { id: '23', partNo: 'PN-2024-023', htsCode: '8528.52.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', lastUpdated: '2026-04-06 15:55' },

  // RETURNED (2 items)
  { id: '7', partNo: 'PN-2024-007', htsCode: '8517.12.00', status: PartStatus.RETURNED, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 11:00' },
  { id: '24', partNo: 'PN-2024-024', htsCode: '8517.62.00', status: PartStatus.RETURNED, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-08 10:30' },

  // UNKNOWN (2 items)
  { id: '6', partNo: 'PN-2024-006', htsCode: '8517.12.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', lastUpdated: '2026-04-09 08:00' },
  { id: '30', partNo: 'PN-2024-030', htsCode: '8471.50.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', lastUpdated: '2026-04-07 09:00' },

  // FLAGGED (1 item)
  { id: '8', partNo: 'PN-2024-008', htsCode: '8517.62.00', status: PartStatus.FLAGGED, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 10:30' },

  // SUPERSEDED (1 item)
  { id: '9', partNo: 'PN-2024-009', htsCode: '8471.50.00', status: PartStatus.SUPERSEDED, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-30 11:00' }
];

export const partService = {
  async getParts(): Promise<Part[]> {
    return MOCK_PARTS;
  },
  async getSuppliers(): Promise<string[]> {
    return Array.from(new Set(MOCK_PARTS.map(p => p.supplier)));
  }
};
