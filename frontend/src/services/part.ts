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

export interface PartHistory {
  id: string;
  status: PartStatus;
  updatedBy: string;
  updatedAt: string;
  remark?: string;
}

export interface Part {
  id: string;
  partNo: string;
  htsCode: string;
  status: PartStatus;
  supplier: string;
  lastUpdated: string;
  description?: string;
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
    history.push({ id: 'h-rev', status: PartStatus.PENDING_REVIEW, updatedBy: 'Customer A', updatedAt: '2026-01-10 14:00' });
  }

  history.push({ id: 'h-curr', status, updatedBy: 'Dimerco Admin', updatedAt: date, remark: 'Status updated via portal.' });
  return history;
};

/**
 * Expanded Mock data for Parts (擴充後的零件模擬資料)
 */
export const MOCK_PARTS: Part[] = [
  // 1-10
  { id: '1', partNo: 'PN-2024-001', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-08 14:30', description: '5G Comm Module', history: generateHistory(PartStatus.ACTIVE, '2026-04-08 14:30') },
  { id: '2', partNo: 'PN-2024-002', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-07 09:15', description: 'Portable Laptop Unit', history: generateHistory(PartStatus.ACTIVE, '2026-04-07 09:15') },
  { id: '3', partNo: 'PN-2024-003', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 10:00', description: 'Router Switch', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 10:00') },
  { id: '4', partNo: 'PN-2024-004', htsCode: '8471.50.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', lastUpdated: '2026-04-05 16:45', description: 'Server Blade', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-05 16:45') },
  { id: '5', partNo: 'PN-2024-005', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-01 11:20', description: 'LCD Monitor', history: generateHistory(PartStatus.ACTIVE, '2026-04-01 11:20') },
  { id: '6', partNo: 'PN-2024-006', htsCode: '8517.12.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', lastUpdated: '2026-04-09 08:00', description: 'Generic Phone Part', history: generateHistory(PartStatus.UNKNOWN, '2026-04-09 08:00') },
  { id: '7', partNo: 'PN-2024-007', htsCode: '8517.12.00', status: PartStatus.RETURNED, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 11:00', description: 'Network Adapter', dimercoRemark: 'Incorrect HTS classification.', replacementCode: '8517.62.00', history: generateHistory(PartStatus.RETURNED, '2026-04-09 11:00') },
  { id: '8', partNo: 'PN-2024-008', htsCode: '8517.62.00', status: PartStatus.FLAGGED, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 10:30', description: 'Wireless Receiver', history: generateHistory(PartStatus.FLAGGED, '2026-04-09 10:30') },
  { id: '9', partNo: 'PN-2024-009', htsCode: '8471.50.00', status: PartStatus.SUPERSEDED, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-30 11:00', description: 'Old Processor Model', history: generateHistory(PartStatus.SUPERSEDED, '2026-03-30 11:00') },
  { id: '10', partNo: 'PN-2024-010', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-03-28 10:00', description: 'Base Station Component', history: generateHistory(PartStatus.ACTIVE, '2026-03-28 10:00') },
  
  // 11-20
  { id: '11', partNo: 'PN-2024-011', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-25 15:30', history: generateHistory(PartStatus.ACTIVE, '2026-03-25 15:30') },
  { id: '12', partNo: 'PN-2024-012', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-20 09:00', history: generateHistory(PartStatus.ACTIVE, '2026-03-20 09:00') },
  { id: '13', partNo: 'PN-2024-013', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-03-15 14:20', history: generateHistory(PartStatus.ACTIVE, '2026-03-15 14:20') },
  { id: '14', partNo: 'PN-2024-014', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-03-10 11:45', history: generateHistory(PartStatus.ACTIVE, '2026-03-10 11:45') },
  { id: '15', partNo: 'PN-2024-015', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-03-05 16:10', history: generateHistory(PartStatus.ACTIVE, '2026-03-05 16:10') },
  { id: '16', partNo: 'PN-2024-016', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-03-01 10:30', history: generateHistory(PartStatus.ACTIVE, '2026-03-01 10:30') },
  { id: '17', partNo: 'PN-2024-017', htsCode: '8517.12.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-09 09:30', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 09:30') },
  { id: '18', partNo: 'PN-2024-018', htsCode: '8471.30.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Alpha Manufacturing', lastUpdated: '2026-04-09 08:45', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 08:45') },
  { id: '19', partNo: 'PN-2024-019', htsCode: '8528.52.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-09 07:15', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 07:15') },
  { id: '20', partNo: 'PN-2024-020', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-08 17:00', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-08 17:00') },

  // 21-30
  { id: '21', partNo: 'PN-2024-021', htsCode: '8517.12.00', status: PartStatus.PENDING_REVIEW, supplier: 'TechCorp Solutions', lastUpdated: '2026-04-08 11:20', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-08 11:20') },
  { id: '22', partNo: 'PN-2024-022', htsCode: '8471.30.00', status: PartStatus.PENDING_REVIEW, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-07 13:40', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-07 13:40') },
  { id: '23', partNo: 'PN-2024-023', htsCode: '8528.52.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', lastUpdated: '2026-04-06 15:55', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-06 15:55') },
  { id: '24', partNo: 'PN-2024-024', htsCode: '8517.62.00', status: PartStatus.RETURNED, supplier: 'Global Logistics Inc', lastUpdated: '2026-04-08 10:30', dimercoRemark: 'Supporting docs required.', history: generateHistory(PartStatus.RETURNED, '2026-04-08 10:30') },
  { id: '25', partNo: 'PN-2024-025', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-02-28 13:45', history: generateHistory(PartStatus.ACTIVE, '2026-02-28 13:45') },
  { id: '26', partNo: 'PN-2024-026', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-02-25 09:20', history: generateHistory(PartStatus.ACTIVE, '2026-02-25 09:20') },
  { id: '27', partNo: 'PN-2024-027', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', lastUpdated: '2026-02-20 15:15', history: generateHistory(PartStatus.ACTIVE, '2026-02-20 15:15') },
  { id: '28', partNo: 'PN-2024-028', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', lastUpdated: '2026-02-15 11:30', history: generateHistory(PartStatus.ACTIVE, '2026-02-15 11:30') },
  { id: '29', partNo: 'PN-2024-029', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', lastUpdated: '2026-02-10 14:00', history: generateHistory(PartStatus.ACTIVE, '2026-02-10 14:00') },
  { id: '30', partNo: 'PN-2024-030', htsCode: '8471.50.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', lastUpdated: '2026-04-07 09:00', history: generateHistory(PartStatus.UNKNOWN, '2026-04-07 09:00') }
];

export const partService = {
  async getParts(): Promise<Part[]> {
    return MOCK_PARTS;
  },
  async getPartById(id: string): Promise<Part | undefined> {
    return MOCK_PARTS.find(p => p.id === id);
  },
  async getSuppliers(): Promise<string[]> {
    return Array.from(new Set(MOCK_PARTS.map(p => p.supplier)));
  },
  async createPart(data: { partNo: string; description: string; htsCode: string }): Promise<Part> {
    const newPart: Part = {
      id: (MOCK_PARTS.length + 1).toString(),
      partNo: data.partNo,
      htsCode: data.htsCode,
      status: PartStatus.PENDING_REVIEW,
      supplier: 'Customer A', // Default for now
      lastUpdated: new Date().toISOString().replace('T', ' ').substring(0, 16),
      description: data.description,
      history: [
        {
          id: 'h-new',
          status: PartStatus.PENDING_REVIEW,
          updatedBy: 'Customer A',
          updatedAt: new Date().toISOString().replace('T', ' ').substring(0, 16),
          remark: 'Part created and submitted for review.'
        }
      ]
    };
    MOCK_PARTS.unshift(newPart);
    return newPart;
  },
  async updatePartStatus(id: string, newStatus: PartStatus, remark?: string): Promise<boolean> {
    const part = MOCK_PARTS.find(p => p.id === id);
    if (part) {
      part.status = newStatus;
      part.lastUpdated = new Date().toISOString().replace('T', ' ').substring(0, 16);
      if (part.history) {
        part.history.push({
          id: 'h' + Date.now(),
          status: newStatus,
          updatedBy: 'Customer A',
          updatedAt: part.lastUpdated,
          remark
        });
      }
      return true;
    }
    return false;
  }
};
