import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import BulkUploadView from '../BulkUploadView.vue';
import Button from '@src/components/common/Button.vue';
import { partService } from '@src/services/part/part';

// Mock services (模擬服務)
/**
 * BulkUploadView Unit Tests (批量上傳頁面單元測試)
 * Update on 2026-04-23: Refactored from Customer to Project focus.
 */
vi.mock('@src/services/part/part', () => ({
  partService: {
    getProjects: vi.fn().mockResolvedValue([]),
    downloadTemplate: vi.fn().mockResolvedValue(undefined),
    uploadParts: vi.fn(),
    previewBulkUpload: vi.fn(),
    confirmBulkUpload: vi.fn()
  },
  ImportResultStatus: {
    NEW: 'NEW',
    UPDATED: 'UPDATED',
    UNCHANGED: 'UNCHANGED',
    REJECTED: 'REJECTED'
  }
}));

vi.mock('@src/services/auth/auth', () => ({
  authService: {
    state: {
      role: 'CUSTOMER',
      projectId: 'P001'
    }
  },
  UserRole: {
    CUSTOMER: 'CUSTOMER',
    DIMERCO: 'DIMERCO'
  }
}));

// Mock useRouter (模擬 useRouter)
vi.mock('vue-router', () => ({
  useRouter: () => ({
    back: vi.fn()
  })
}));

/**
 * BulkUploadView Unit Tests (批量上傳頁面單元測試)
 * 
 * Created by Gemini AI on 2026-04-20 (INTERNAL-AI-20260420)
 */
describe('BulkUploadView (批量上傳頁面)', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('should call partService.downloadTemplate when download button is clicked (點擊下載按鈕時應呼叫 partService.downloadTemplate)', async () => {
    const wrapper = mount(BulkUploadView, {
      global: {
        stubs: {
          'el-upload': true,
          'el-select': true,
          'el-option': true,
          'el-progress': true,
          'el-tag': true,
          'el-table': true,
          'el-table-column': true,
          BulkUploadPreviewTable: true
        },
        mocks: {
          $t: (msg: string) => msg
        }
      }
    });

    // Find the download template button (找到下載範本按鈕)
    const downloadBtn = wrapper.findAllComponents(Button).find(b => 
      b.text().includes('part_upload.download_template')
    );
    
    expect(downloadBtn).toBeDefined();
    
    if (downloadBtn) {
      await downloadBtn.trigger('click');
      expect(partService.downloadTemplate).toHaveBeenCalledTimes(1);
    }
  });
});
