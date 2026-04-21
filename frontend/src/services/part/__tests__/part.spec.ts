import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { partService } from '../part';
import api from '../../api';

// Mock api (模擬 api)
vi.mock('../../api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    patch: vi.fn(),
    put: vi.fn(),
    defaults: {
      baseURL: '/api'
    }
  }
}));

/**
 * Part Service Unit Tests (零件服務單元測試)
 * 
 * Created by Gemini AI on 2026-04-20 (INTERNAL-AI-20260420)
 */
describe('Part Service (零件服務)', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    
    // Stub URL.createObjectURL and revokeObjectURL
    vi.stubGlobal('URL', {
      createObjectURL: vi.fn(() => 'blob:mock-url'),
      revokeObjectURL: vi.fn()
    });
    
    // Mock document methods for link clicking
    vi.stubGlobal('document', {
      createElement: vi.fn().mockImplementation((tag) => {
        if (tag === 'a') {
          return {
            href: '',
            setAttribute: vi.fn(),
            click: vi.fn(),
            remove: vi.fn()
          };
        }
        return {};
      }),
      body: {
        appendChild: vi.fn()
      }
    });
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  describe('downloadTemplate (下載範本)', () => {
    it('should call the correct endpoint with responseType blob and trigger download (應以 responseType blob 呼叫正確端點並觸發下載)', async () => {
      // Setup mock response (設定模擬回應)
      const mockBlob = new Blob(['mock-data'], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      (api.get as any).mockResolvedValue({ data: mockBlob });

      await partService.downloadTemplate();

      // Verify API call (驗證 API 呼叫)
      expect(api.get).toHaveBeenCalledWith('/parts/bulk-upload/template', {
        responseType: 'blob'
      });

      // Verify blob processing (驗證 Blob 處理)
      expect(URL.createObjectURL).toHaveBeenCalledWith(expect.any(Blob));
      
      // Verify download link creation and cleanup (驗證下載連結建立與清理)
      expect(document.createElement).toHaveBeenCalledWith('a');
      expect(document.body.appendChild).toHaveBeenCalled();
      expect(URL.revokeObjectURL).toHaveBeenCalledWith('blob:mock-url');
    });

    it('should throw error if download fails (如果下載失敗應拋出錯誤)', async () => {
      (api.get as any).mockRejectedValue(new Error('Download failed'));

      await expect(partService.downloadTemplate()).rejects.toThrow('Download failed');
    });
  });

  describe('previewBulkUpload (批量上傳預覽)', () => {
    it('should call the correct endpoint with multipart/form-data and customerId (應以 multipart/form-data 及 customerId 呼叫正確端點)', async () => {
      const mockFile = new File(['mock-content'], 'test.xlsx');
      const mockReport: BulkUploadPreviewReport = {
        summary: { totalRows: 1, newCount: 1, modifiedCount: 0, errorCount: 0, noChangeCount: 0 },
        rows: []
      };
      (api.post as any).mockResolvedValue({ data: { success: true, data: mockReport } });

      const result = await partService.previewBulkUpload(mockFile, 123);

      expect(api.post).toHaveBeenCalledWith('/parts/bulk-upload/preview', expect.any(FormData));
      expect(result).toEqual(mockReport);
    });
  });

  describe('confirmBulkUpload (批量上傳確認)', () => {
    it('should call the correct endpoint with the provided data array (應以提供的資料陣列呼叫正確端點)', async () => {
      const mockData: BulkUploadPartData[] = [
        { customerId: 123, partNo: 'PN01' }
      ];
      const mockResponse: BulkUploadConfirmResponse = {
        inserted: 1, updated: 0, failed: 0, errors: []
      };
      (api.post as any).mockResolvedValue({ data: { success: true, data: mockResponse } });

      const result = await partService.confirmBulkUpload(mockData);

      expect(api.post).toHaveBeenCalledWith('/parts/bulk-upload/confirm', mockData);
      expect(result).toEqual(mockResponse);
    });
  });
});
