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
 * Project Interface (專案介面) - Alias for CommonOption
 */
export type ProjectOption = CommonOption;

/**
 * Status Interface (狀態介面) - Alias for CommonOption
 */
export type StatusOption = CommonOption;

/**
 * Supplier Interface (供應商介面) - Alias for CommonOption
 */
export type SupplierOption = CommonOption;

export type CountryOption = CommonOption;

/**
 * Mock data for Projects (專案模擬資料)
 */
/*
export const MOCK_PROJECTS: ProjectOption[] = [
  { key: 'project001', value: 'Dimerco Electronics' },
  { key: 'project002', value: 'Global Tech Solutions' },
  { key: 'project003', value: 'Alpha Systems Corp' }
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
   * @param {string} projectId - Filter by project (按專案篩選)
   */
  async getSuppliers(projectId: string = 'all'): Promise<SupplierOption[]> {
    try {
      const response = await api.get<{ success: boolean; message: string; data: SupplierOption[] }>('/common/suppliers', {
        params: { projectId }
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
   * Get Projects from Common API (從通用 API 獲取專案)
   * (繁體中文) 從 /api/common/projects 獲取專案清單。
   */
  async getProjects(): Promise<ProjectOption[]> {
    try {
      const response = await api.get<{ success: boolean; message: string; data: ProjectOption[] }>('/common/projects');
      if (response.data.success) {
        return response.data.data;
      }
      return []; // Removed mock fallback
    } catch (error) {
      console.error('API /common/projects failed. (API 失敗。)', error);
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
  },

  async getCountries(): Promise<CountryOption[]> {
    try {
      const response = await api.get<{ success: boolean; message: string; data: CountryOption[] }>('/common/countries');
      if (response.data.success) {
        return response.data.data;
      }
      return []; // Removed mock fallback
    } catch (error) {
      console.error('API /common/countries failed. (API 失敗。)', error);
      return [];
    }
  },

  /**
   * Format ISO Date string to local YYYY-MM-DD HH:mm
   * (將 ISO 日期字串格式化為本地時區的 YYYY-MM-DD HH:mm)
   * @param {string} isoString - ISO formatted date string
   * @returns {string} Formatted local datetime
   */
  formatDateTime(isoString?: string): string {
    if (!isoString) return '-';
    try {
      const date = new Date(isoString);
      if (isNaN(date.getTime())) return isoString;

      const Y = date.getFullYear();
      const M = String(date.getMonth() + 1).padStart(2, '0');
      const D = String(date.getDate()).padStart(2, '0');
      const h = String(date.getHours()).padStart(2, '0');
      const m = String(date.getMinutes()).padStart(2, '0');

      return `${Y}-${M}-${D} ${h}:${m}`;
    } catch (e) {
      return isoString;
    }
  }
};
