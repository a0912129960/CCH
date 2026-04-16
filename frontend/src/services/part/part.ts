import api from '../api';

/**
 * Part Status (零件狀態)
 */
export const PartStatus = {
  UNKNOWN: 'UNKNOWN',
  PENDING_CUSTOMER: 'PENDING_CUSTOMER',
  PENDING_REVIEW: 'PENDING_REVIEW',
  RETURNED: 'RETURNED',
  ACTIVE: 'ACTIVE',
  FLAGGED: 'FLAGGED',
  SUPERSEDED: 'SUPERSEDED'
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
  partNo: string;
  htsCode: string;
  status: PartStatus;
  supplier: string;
  customerId: string;
  customerName: string;
  lastUpdated: string;
  description?: string;
  dimercoRemark?: string;
  replacementCode?: string;
  history?: PartHistory[];
}

/**
 * Customer Interface (客戶介面)
 * From /api/common/customers
 */
export interface CustomerOption {
  key: string;   // Customer ID
  value: string; // Customer Name
}

/**
 * Mock data for Customers (客戶模擬資料)
 */
export const MOCK_CUSTOMERS: CustomerOption[] = [
  { key: 'customer001', value: 'Dimerco Electronics' },
  { key: 'customer002', value: 'Global Tech Solutions' },
  { key: 'customer003', value: 'Alpha Systems Corp' }
];

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
  // 1-10 (Customer 001)
  { id: '1', partNo: 'PN-2024-001', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-08 14:30', description: '5G Comm Module', history: generateHistory(PartStatus.ACTIVE, '2026-04-08 14:30') },
  { id: '2', partNo: 'PN-2024-002', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-07 09:15', description: 'Portable Laptop Unit', history: generateHistory(PartStatus.ACTIVE, '2026-04-07 09:15') },
  { id: '3', partNo: 'PN-2024-003', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-09 10:00', description: 'Router Switch', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 10:00') },
  { id: '4', partNo: 'PN-2024-004', htsCode: '8471.50.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-05 16:45', description: 'Server Blade', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-05 16:45') },
  { id: '5', partNo: 'PN-2024-005', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-01 11:20', description: 'LCD Monitor', history: generateHistory(PartStatus.ACTIVE, '2026-04-01 11:20') },
  { id: '6', partNo: 'PN-2024-006', htsCode: '8517.12.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-09 08:00', description: 'Generic Phone Part', history: generateHistory(PartStatus.UNKNOWN, '2026-04-09 08:00') },
  { id: '7', partNo: 'PN-2024-007', htsCode: '8517.12.00', status: PartStatus.RETURNED, supplier: 'TechCorp Solutions', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-09 11:00', description: 'Network Adapter', dimercoRemark: 'Incorrect HTS classification.', replacementCode: '8517.62.00', history: generateHistory(PartStatus.RETURNED, '2026-04-09 11:00') },
  { id: '8', partNo: 'PN-2024-008', htsCode: '8517.62.00', status: PartStatus.FLAGGED, supplier: 'TechCorp Solutions', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-04-09 10:30', description: 'Wireless Receiver', history: generateHistory(PartStatus.FLAGGED, '2026-04-09 10:30') },
  { id: '9', partNo: 'PN-2024-009', htsCode: '8471.50.00', status: PartStatus.SUPERSEDED, supplier: 'Alpha Manufacturing', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-03-30 11:00', description: 'Old Processor Model', history: generateHistory(PartStatus.SUPERSEDED, '2026-03-30 11:00') },
  { id: '10', partNo: 'PN-2024-010', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', customerId: 'customer001', customerName: 'Dimerco Electronics', lastUpdated: '2026-03-28 10:00', description: 'Base Station Component', history: generateHistory(PartStatus.ACTIVE, '2026-03-28 10:00') },
  
  // 11-20 (Customer 002)
  { id: '11', partNo: 'PN-G-011', htsCode: '8471.50.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-03-25 15:30', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-03-25 15:30') },
  { id: '12', partNo: 'PN-G-012', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-03-20 09:00', history: generateHistory(PartStatus.ACTIVE, '2026-03-20 09:00') },
  { id: '13', partNo: 'PN-G-013', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-03-15 14:20', history: generateHistory(PartStatus.ACTIVE, '2026-03-15 14:20') },
  { id: '14', partNo: 'PN-G-014', htsCode: '8471.30.00', status: PartStatus.PENDING_REVIEW, supplier: 'TechCorp Solutions', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-03-10 11:45', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-03-10 11:45') },
  { id: '15', partNo: 'PN-G-015', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-03-05 16:10', history: generateHistory(PartStatus.ACTIVE, '2026-03-05 16:10') },
  { id: '16', partNo: 'PN-G-016', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-03-01 10:30', history: generateHistory(PartStatus.ACTIVE, '2026-03-01 10:30') },
  { id: '17', partNo: 'PN-G-017', htsCode: '8517.12.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-04-09 09:30', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 09:30') },
  { id: '18', partNo: 'PN-G-018', htsCode: '8471.30.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Alpha Manufacturing', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-04-09 08:45', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 08:45') },
  { id: '19', partNo: 'PN-G-019', htsCode: '8528.52.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'TechCorp Solutions', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-04-09 07:15', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-09 07:15') },
  { id: '20', partNo: 'PN-G-020', htsCode: '8517.62.00', status: PartStatus.PENDING_CUSTOMER, supplier: 'Global Logistics Inc', customerId: 'customer002', customerName: 'Global Tech Solutions', lastUpdated: '2026-04-08 17:00', history: generateHistory(PartStatus.PENDING_CUSTOMER, '2026-04-08 17:00') },

  // 21-30 (Customer 003)
  { id: '21', partNo: 'PN-A-021', htsCode: '8517.12.00', status: PartStatus.PENDING_REVIEW, supplier: 'TechCorp Solutions', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-04-08 11:20', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-08 11:20') },
  { id: '22', partNo: 'PN-A-022', htsCode: '8471.30.00', status: PartStatus.PENDING_REVIEW, supplier: 'Global Logistics Inc', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-04-07 13:40', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-07 13:40') },
  { id: '23', partNo: 'PN-A-023', htsCode: '8528.52.00', status: PartStatus.PENDING_REVIEW, supplier: 'Alpha Manufacturing', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-04-06 15:55', history: generateHistory(PartStatus.PENDING_REVIEW, '2026-04-06 15:55') },
  { id: '24', partNo: 'PN-A-024', htsCode: '8517.62.00', status: PartStatus.RETURNED, supplier: 'Global Logistics Inc', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-04-08 10:30', dimercoRemark: 'Supporting docs required.', history: generateHistory(PartStatus.RETURNED, '2026-04-08 10:30') },
  { id: '25', partNo: 'PN-A-025', htsCode: '8517.12.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-02-28 13:45', history: generateHistory(PartStatus.ACTIVE, '2026-02-28 13:45') },
  { id: '26', partNo: 'PN-A-026', htsCode: '8471.30.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-02-25 09:20', history: generateHistory(PartStatus.ACTIVE, '2026-02-25 09:20') },
  { id: '27', partNo: 'PN-A-027', htsCode: '8528.52.00', status: PartStatus.ACTIVE, supplier: 'Alpha Manufacturing', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-02-20 15:15', history: generateHistory(PartStatus.ACTIVE, '2026-02-20 15:15') },
  { id: '28', partNo: 'PN-A-028', htsCode: '8517.62.00', status: PartStatus.ACTIVE, supplier: 'Global Logistics Inc', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-02-15 11:30', history: generateHistory(PartStatus.ACTIVE, '2026-02-15 11:30') },
  { id: '29', partNo: 'PN-A-029', htsCode: '8471.50.00', status: PartStatus.ACTIVE, supplier: 'TechCorp Solutions', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-02-10 14:00', history: generateHistory(PartStatus.ACTIVE, '2026-02-10 14:00') },
  { id: '30', partNo: 'PN-A-030', htsCode: '8471.50.00', status: PartStatus.UNKNOWN, supplier: 'Unknown Source', customerId: 'customer003', customerName: 'Alpha Systems Corp', lastUpdated: '2026-04-07 09:00', history: generateHistory(PartStatus.UNKNOWN, '2026-04-07 09:00') }
];

/**
 * Expanded Mock data for Suppliers (供應商模擬資料)
 */
export const MOCK_SUPPLIERS = [
  'TechCorp Solutions',
  'Global Logistics Inc',
  'Alpha Manufacturing',
  'Beta Electronics',
  'Delta Systems',
  'Omega Industrial'
];

export const partService = {
  async getParts(): Promise<Part[]> {
    return MOCK_PARTS;
  },
  async getPartById(id: string): Promise<Part | undefined> {
    return MOCK_PARTS.find(p => p.id === id);
  },
  async getSuppliers(): Promise<string[]> {
    return MOCK_SUPPLIERS;
  },
  /**
   * Get Customers from Common API (從通用 API 獲取客戶)
   * (繁體中文) 從 /api/common/customers 獲取客戶清單。
   */
  async getCustomers(): Promise<CustomerOption[]> {
    try {
      const response = await api.get<{ success: boolean; message: string; data: CustomerOption[] }>('/common/customers');
      if (response.data.success) {
        return response.data.data;
      }
      return MOCK_CUSTOMERS; // Fallback to mock (回退至模擬資料)
    } catch (error) {
      console.warn('API /common/customers failed, using mock data. (API 失敗，使用模擬資料。)');
      return MOCK_CUSTOMERS;
    }
  },
  async createPart(data: { 
    partNo: string; 
    description: string; 
    htsCode: string; 
    supplier: string; 
    customerId?: string; 
    customerName?: string;
    status?: PartStatus;
  }): Promise<Part> {
    const newPart: Part = {
      id: (MOCK_PARTS.length + 1).toString(),
      partNo: data.partNo,
      htsCode: data.htsCode,
      status: data.status || PartStatus.PENDING_REVIEW,
      supplier: data.supplier || 'Unknown Source',
      customerId: data.customerId || 'customer001',
      customerName: data.customerName || 'Dimerco Electronics',
      lastUpdated: new Date().toISOString().replace('T', ' ').substring(0, 16),
      description: data.description,
      history: [
        {
          id: 'h-new',
          status: data.status || PartStatus.PENDING_REVIEW,
          updatedBy: data.status === PartStatus.ACTIVE ? 'Dimerco Employee' : 'Customer A',
          updatedAt: new Date().toISOString().replace('T', ' ').substring(0, 16),
          remark: data.status === PartStatus.ACTIVE ? 'Part created and auto-approved by employee.' : 'Part created and submitted for review.'
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
  },
  
  /**
   * Bulk Upload Methods (批量上傳方法)
   */
  async downloadTemplate(): Promise<void> {
    // Mock template download (模擬範本下載)
    const headers = ['Part No', 'HTS Code', 'Description', 'Supplier'];
    const csvContent = "data:text/csv;charset=utf-8," + headers.join(",");
    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "part_upload_template.csv");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },

  async uploadParts( customerId?: string, onProgress?: (percent: number) => void): Promise<ImportBatchReport> {
    // Simulate progress (模擬進度)
    if (onProgress) {
      for (let i = 0; i <= 100; i += 20) {
        onProgress(i);
        await new Promise(resolve => setTimeout(resolve, 200));
      }
    }

    // Mock processing logic (模擬處理邏輯)
    // If customerId is provided (from Employee), parts should be NEW/UPDATED and ACTIVE
    const defaultStatus = customerId ? 'ACTIVE' : 'PENDING_REVIEW';
    const mockRows: ImportRowReport[] = [
      { partNo: 'PN-NEW-001', htsCode: '8517.12.00', status: ImportResultStatus.NEW, message: `Successfully imported as ${defaultStatus}` },
      { partNo: 'PN-2024-001', htsCode: '8517.12.00', status: ImportResultStatus.UNCHANGED, message: 'No changes detected' },
      { partNo: 'PN-2024-002', htsCode: '9999.99.99', status: ImportResultStatus.UPDATED, message: `HTS Code updated, status set to ${defaultStatus}` },
      { partNo: 'PN-ERR-999', htsCode: 'INVALID', status: ImportResultStatus.REJECTED, message: 'Invalid HTS Code format' }
    ];

    return {
      batchId: 'batch-' + Date.now(),
      totalRows: mockRows.length,
      newCount: 1,
      updatedCount: 1,
      unchangedCount: 1,
      rejectedCount: 1,
      rows: mockRows
    };
  }
};

// INTERNAL-AI-20260416: New interfaces for the real GET /api/parts/{partId} response.
// (INTERNAL-AI-20260416: 新增對應後端 GET /api/parts/{partId} 回應的 TypeScript 介面。)

/**
 * Part detail field set matching backend PartDetailDto.
 * (對應後端 PartDetailDto 的零件詳細欄位集合。)
 */
export interface PartDetailFields {
  partNo: string;
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
}

/**
 * Response for GET /api/parts/{partId} matching backend PartDetailResponseDto.
 * (對應後端 PartDetailResponseDto 的零件詳細回應結構。)
 */
export interface PartDetailResponse {
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
