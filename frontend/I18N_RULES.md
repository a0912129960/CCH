# Frontend I18n Implementation Specification (前端多國語系實作規範)

## ⚖️ 1. Core Principles (核心原則)
- **Centralized Management (集中管理)**: All user-facing text **MUST** be extracted into language JSON files. (所有面向使用者的文字**必須**提取至語系 JSON 檔案中。)
- **Key Naming Convention (鍵名命名規範)**: 
  - Use `snake_case` for keys.
  - Structure: `[view/component].[context].[sub_context]`.
- **Trilingual Mandate (三語強制指令)**: Following the latest governance update, all i18n keys **MUST** contain `en`, `zh-TW`, and `zh-CN`. 
  - **三語同步**: 所有的 i18n 鍵名必須同時包含英文、繁體中文與簡體中文。

## 📁 2. Directory Structure (目錄結構)
```text
src/
└── locales/
    ├── index.ts        # i18n initialization
    ├── en.json         # English (Mandatory)
    ├── zh-TW.json      # Traditional Chinese (Mandatory)
    └── zh-CN.json      # Simplified Chinese (Mandatory)
```

## 🛠️ 3. Usage Guidelines (使用指南)
- Use `$t()` in templates and `t()` from `useI18n()` in scripts.
- **NEVER** hardcode Chinese or English strings in components.

## 🔄 4. Language Switching Logic (切換邏輯)
- Persist selection in `localStorage`.
- Default to `en` if no preference found.

## 🧪 5. Validation (驗證)
- **Completeness Check**: A feature is only considered "Done" if translations exist in all 3 JSON files.
- **UI Verification**: Verify text rendering across different locales.

---
*Updated on 2026-04-09 by Gemini CLI (Trilingual Policy)*
