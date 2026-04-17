import { setActivePinia, createPinia } from 'pinia';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { useTabStore } from '../tabs';

// Better Mocking Strategy
const mockPush = vi.fn();
vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: mockPush,
  }),
  useRoute: () => ({
    path: '/',
  }),
}));

describe('Tab Store (標籤 Store 測試)', () => {
  beforeEach(() => {
    setActivePinia(createPinia());
    vi.clearAllMocks();
  });

  it('should add a new tab correctly (應能正確新增標籤)', () => {
    const tabStore = useTabStore();
    const mockTab = { title: 'Test Page', path: '/test', name: 'test' };

    tabStore.addTab(mockTab);

    expect(tabStore.openTabs).toHaveLength(1);
    expect(tabStore.openTabs[0]).toEqual(mockTab);
    expect(tabStore.activeTabPath).toBe('/test');
  });

  it('should redirect to home when last tab is removed (移除最後一個標籤時應導向首頁)', () => {
    const tabStore = useTabStore();
    const mockTab = { title: 'Test', path: '/test', name: 'test' };

    tabStore.addTab(mockTab);
    tabStore.removeTab('/test');

    expect(tabStore.openTabs).toHaveLength(0);
    expect(mockPush).toHaveBeenCalledWith('/');
  });

  it('should handle closeAll correctly (應能正確執行關閉全部)', () => {
    const tabStore = useTabStore();
    tabStore.addTab({ title: 'T1', path: '/1', name: 'n1' });
    tabStore.closeAll();

    expect(tabStore.openTabs).toHaveLength(0);
    expect(mockPush).toHaveBeenCalledWith('/');
  });

  it('should close others correctly (應能正確執行關閉其他)', () => {
    const tabStore = useTabStore();
    tabStore.addTab({ title: 'T1', path: '/1', name: 'n1' });
    tabStore.addTab({ title: 'T2', path: '/2', name: 'n2' });

    tabStore.closeOthers('/2');

    expect(tabStore.openTabs).toHaveLength(1);
    expect(tabStore.openTabs[0].path).toBe('/2');
  });
});
