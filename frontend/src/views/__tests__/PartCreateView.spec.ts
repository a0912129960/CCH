import { describe, it, expect, vi, beforeEach } from 'vitest';
import { mount } from '@vue/test-utils';
import PartCreateView from '../PartCreateView.vue';
import { partService } from '../../services/part';

/**
 * Part Creation View Component Tests (新增零件組件測試)
 * BR-08: HTS Format | BR-21: Description Quality
 */
vi.mock('../../services/part', () => ({
  partService: {
    createPart: vi.fn().mockResolvedValue({ id: '999', partNo: 'TEST-PN' })
  }
}));

const pushSpy = vi.fn();
vi.mock('vue-router', () => ({
  useRouter: () => ({ 
    push: pushSpy,
    back: vi.fn(),
    app: { config: { globalProperties: { $t: (key: string) => key } } }
  })
}));

describe('PartCreateView.vue', () => {
  const globalConfig = {
    global: {
      mocks: { $t: (key: string) => key },
      stubs: {
        Card: { template: '<div class="card"><slot name="header"></slot><slot></slot></div>' }
      }
    }
  };

  beforeEach(() => {
    vi.clearAllMocks();
    pushSpy.mockClear();
  });

  it('renders correctly (正確渲染)', () => {
    const wrapper = mount(PartCreateView, globalConfig);
    expect(wrapper.find('h1').text()).toBe('part_create.title');
  });

  it('validates HTS Code format - BR-08 (驗證 HTS Code 格式)', async () => {
    const wrapper = mount(PartCreateView, {
      ...globalConfig,
      global: {
        ...globalConfig.global,
        stubs: {
          ...globalConfig.global.stubs,
          Button: { template: '<button><slot></slot></button>' }
        }
      }
    });
    const htsInput = wrapper.find('[data-test="hts-code-input"]');
    
    // Test filtering: should remove letters and format dots
    await htsInput.setValue('1234abcd56ef7890');
    expect((htsInput.element as HTMLInputElement).value).toBe('1234.56.7890');

    // Test truncation: should not exceed 10 digits
    await htsInput.setValue('12345678901234');
    expect((htsInput.element as HTMLInputElement).value).toBe('1234.56.7890');

    // Test partial input
    await htsInput.setValue('123');
    expect((htsInput.element as HTMLInputElement).value).toBe('123');
    // Error for hts format should be gone
    expect(wrapper.find('[data-test="hts-code-error"]').exists()).toBe(false);
    });

    it('submits form successfully (成功提交表單)', async () => {
    // Mock global alert
    const alertMock = vi.fn();
    vi.stubGlobal('alert', alertMock);
    
    const wrapper = mount(PartCreateView, globalConfig);
    
    await wrapper.find('[data-test="part-no-input"]').setValue('PN-001');
    await wrapper.find('[data-test="hts-code-input"]').setValue('1234.56.7890');
    await wrapper.find('[data-test="description-input"]').setValue('This is a strong description with enough words for the validation to pass.');
    
    await wrapper.find('form').trigger('submit.prevent');
    
    expect(partService.createPart).toHaveBeenCalled();
    expect(pushSpy).toHaveBeenCalledWith('/parts');
    expect(alertMock).toHaveBeenCalled();
    
    vi.unstubAllGlobals();
  });
});
