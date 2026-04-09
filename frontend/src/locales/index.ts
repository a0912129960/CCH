import { createI18n } from 'vue-i18n';
import en from './en.json';
import zhTW from './zh-TW.json';
import zhCN from './zh-CN.json';

/**
 * Get initial language (獲取初始語系)
 * Priority: localStorage -> default 'en'
 */
const getLocale = (): string => {
  const savedLocale = localStorage.getItem('user-locale');
  return savedLocale || 'en';
};

const i18n = createI18n({
  legacy: false, // Use Composition API (使用組合式 API)
  locale: getLocale(),
  fallbackLocale: 'en',
  messages: {
    'en': en,
    'zh-TW': zhTW,
    'zh-CN': zhCN
  }
});

/**
 * Switch language and save to storage (切換語系並儲存至儲存空間)
 * @param {string} locale - Target locale (目標語系)
 */
export const switchLanguage = (locale: string) => {
  i18n.global.locale.value = locale;
  localStorage.setItem('user-locale', locale);
};

export default i18n;
