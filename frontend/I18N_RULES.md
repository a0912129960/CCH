# Frontend I18n Implementation Specification (前端多國語系實作規範)

## ⚖️ 1. Core Principles (核心原則)
- **Centralized Management (集中管理)**: All user-facing text **MUST** be extracted into language JSON files. (所有面向使用者的文字**必須**提取至語系 JSON 檔案中。)
- **Key Naming Convention (鍵名命名規範)**: 
  - Use `snake_case` for keys. (鍵名使用 `snake_case`。)
  - Structure by views/components: `[view/component].[context].[sub_context]`.
  - Example: `login.form.username_label` (登入頁.表單.帳號標籤)。
- **Bilingual Mandate (雙語指令)**: Following the CCH Constitution, all i18n keys **MUST** contain at least `zh-TW` and `en`. (遵循 CCH 憲法，所有 i18n 鍵名**必須**至少包含繁體中文與英文。)

## 📁 2. Directory Structure (目錄結構)
```text
src/
└── locales/
    ├── index.ts        # i18n initialization (初始化配置)
    ├── en.json         # English (英文)
    ├── zh-TW.json      # Traditional Chinese (繁體中文)
    └── zh-CN.json      # Simplified Chinese (簡體中文)
```

## 🛠️ 3. Usage Guidelines (使用指南)

### In Vue Template (在模板中使用)
```html
<template>
  <label>{{ $t('login.form.username') }}</label>
</template>
```

### In Script Setup (在腳本中使用)
```typescript
import { useI18n } from 'vue-i18n';
const { t } = useI18n();
const message = t('login.messages.success');
```

## 🔄 4. Language Switching Logic (切換邏輯)
- **Persistency (持久化)**: Current language **MUST** be stored in `localStorage`. (當前語系**必須**儲存於 `localStorage`。)
- **Initial Load (初始載入)**: Priority order: `localStorage` -> `navigator.language` -> `en`.

## 🧪 5. Validation (驗證)
- Every new feature **MUST** verify text display in all three supported languages before delivery. (每個新功能在交付前**必須**驗證所有三種支援語系的文字顯示。)

---
*Created by Gemini CLI - 2026/04/08 (AUTH-I18N)*
