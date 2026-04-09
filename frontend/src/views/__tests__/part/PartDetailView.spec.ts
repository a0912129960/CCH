import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import PartDetailView from '../../part/PartDetailView.vue';
import { partService, PartStatus } from '../../../services/part/part';

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string, params?: any) => {
      if (params && params.min) return `${key}:${params.min}`;
      return key;
    }
  })
}));

// Mock vue-router
const mockBack = vi.fn();
const mockPush = vi.fn();
vi.mock('vue-router', () => ({
  useRoute: () => ({ params: { id: '7' } }),
  useRouter: () => ({
    back: mockBack,
    push: mockPush,
    app: { config: { globalProperties: { $t: (k: string) => k } } }
  })
}));

// Mock partService
vi.mock('../../../services/part/part', async () => {
  const actual = await vi.importActual('../../../services/part/part') as any;
  return {
    ...actual,
    partService: {
      getPartById: vi.fn().mockResolvedValue({
        id: '7',
        partNo: 'PN-2024-007',
        htsCode: '8517.12.00',
        status: 'RETURNED',
        supplier: 'TechCorp',
        lastUpdated: '2026-04-09',
        dimercoRemark: 'Reason text',
        history: [{ id: 'h1', status: 'RETURNED', updatedBy: 'Admin', updatedAt: '2026-04-09' }]
      }),
      updatePartStatus: vi.fn().mockResolvedValue(true)
    }
  };
});

describe('PartDetailView.vue', () => {
  const globalConfig = {
    global: {
      mocks: { $t: (key: string) => key },
      stubs: {
        Card: { template: '<div class="card"><slot name="header"></slot><slot></slot></div>' },
        Dot: { template: '<div class="dot"></div>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders part details correctly (正確渲染零件詳情)', async () => {
    const wrapper = mount(PartDetailView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));

    expect(wrapper.find('h1').text()).toBe('PN-2024-007');
    expect(wrapper.find('.code-box').text()).toBe('8517.12.00');
  });

  it('shows Dimerco feedback when status is RETURNED (狀態為退回時顯示反饋資訊)', async () => {
    const wrapper = mount(PartDetailView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));

    expect(wrapper.find('.feedback-card').exists()).toBe(true);
    expect(wrapper.find('.reason-group p').text()).toBe('Reason text');
  });

  it('handles resubmit action (處理重新提交操作)', async () => {
    const wrapper = mount(PartDetailView, globalConfig);
    await new Promise(resolve => setTimeout(resolve, 50));

    const textarea = wrapper.find('textarea');
    await textarea.setValue('Updated info');
    
    const resubmitBtn = wrapper.find('.btn-primary');
    await resubmitBtn.trigger('click');

    expect(partService.updatePartStatus).toHaveBeenCalledWith('7', 'PENDING_REVIEW', 'Updated info');
    expect(mockPush).toHaveBeenCalledWith('/parts');
  });
});
