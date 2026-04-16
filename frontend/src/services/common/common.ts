import api from '../api';

/**
 * Common Option Interface (通用選項介面)
 * Used for dropdowns from /api/common/
 */
export interface CommonOption {
  key: string;   // Option ID/Key
  value: string; // Option Display Value
}

/**
 * Customer Interface (客戶介面) - Alias for CommonOption
 */
export type CustomerOption = CommonOption;

/**
 * Status Interface (狀態介面) - Alias for CommonOption
 */
export type StatusOption = CommonOption;

/**
 * Supplier Interface (供應商介面) - Alias for CommonOption
 */
export type SupplierOption = CommonOption;

/**
 * Mock data for Customers (客戶模擬資料)
 */
/*
export const MOCK_CUSTOMERS: CustomerOption[] = [
  { key: 'customer001', value: 'Dimerco Electronics' },
  { key: 'customer002', value: 'Global Tech Solutions' },
  { key: 'customer003', value: 'Alpha Systems Corp' }
];
*/

/**
 * Mock data for Suppliers (供應商模擬資料)
 */
/*
export const MOCK_SUPPLIERS: SupplierOption[] = [
  { key: 'S001', value: 'TechCorp Solutions' },
  { key: 'S002', value: 'Global Logistics Inc' },
  { key: 'S003', value: 'Alpha Manufacturing' }
];
*/

/**
 * Mock data for Statuses (狀態模擬資料)
 */
/*
export const MOCK_STATUSES: StatusOption[] = Object.entries(PartStatus).map(([key, val]) => ({
  key: val,
  value: val.charAt(0) + val.slice(1).toLowerCase().replace('_', ' ')
}));
*/

/**
 * Common Service (通用服務)
 * Handles cross-domain lookup data. (處理跨領域的查閱資料。)
 */
export const commonService = {
  /**
   * Get Suppliers from Common API (從通用 API 獲取供應商)
   * (繁體中文) 從 /api/common/suppliers 獲取供應商清單。
   * @param {string} customerId - Filter by customer (按客戶篩選)
   */
  async getSuppliers(customerId: string = 'all'): Promise<SupplierOption[]> {
    try {
      const response = await api.get<{ success: boolean; message: string; data: SupplierOption[] }>('/common/suppliers', {
        params: { customerId }
      });
      if (response.data.success) {
        return response.data.data;
      }
      return []; // Removed mock fallback
    } catch (error) {
      console.error('API /common/suppliers failed. (API 失敗。)', error);
      return [];
    }
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
      return []; // Removed mock fallback
    } catch (error) {
      console.error('API /common/customers failed. (API 失敗。)', error);
      return [];
    }
  },

  /**
   * Get Status Options from Common API (從通用 API 獲取狀態選項)
   * (繁體中文) 從 /api/common/status 獲取狀態清單。
   */
  async getStatusOptions(): Promise<StatusOption[]> {
    try {
      const response = await api.get<{ success: boolean; message: string; data: StatusOption[] }>('/common/status');
      if (response.data.success) {
        return response.data.data;
      }
      return []; // Removed mock fallback
    } catch (error) {
      console.error('API /common/status failed. (API 失敗。)', error);
      return [];
    }
  }
};
