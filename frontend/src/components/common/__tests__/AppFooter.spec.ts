import { describe, it, expect } from 'vitest';
import { mount } from '@vue/test-utils';
import AppFooter from '../AppFooter.vue';

const globalMocks = {
  $t: (key: string) => key
};

describe('AppFooter.vue', () => {
  it('renders copyright text', () => {
    const wrapper = mount(AppFooter, {
      global: { mocks: globalMocks }
    });
    expect(wrapper.text()).toContain('common.copyright');
  });
});
