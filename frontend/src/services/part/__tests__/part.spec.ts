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
});
