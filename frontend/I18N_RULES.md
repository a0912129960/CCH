# Frontend I18n Implementation Specification (前端多國語系實作規範)

## ⚖️ 1. Core Principles (核心原則)
- **Centralized Management (集中管理)**: All user-facing text **MUST** be extracted into language JSON files. (所有面向使用者的文字**必須**提取至語系 JSON 檔案中。)
- **Key Naming Convention (鍵名命名規範)**: 
  - Use `snake_case` for keys.
  - Structure: `[view/component].[context].[sub_context]`.
- **Instruction vs. Implementation (指令與實作之別)**: 
  - **Bilingual Instructions**: AI communication (plans, explanations) and code remarks are limited to **Traditional Chinese** and **English**. (溝通與註解僅限繁體中文與英文。)
  - **Trilingual Implementation**: ALL application i18n keys **MUST** sync across `en`, `zh-TW`, and `zh-CN`. (程式多語系必須同步英文、繁體中文與簡體中文。)
- **Completeness**: A feature is NOT "Done" if any supported locale file is missing the new keys. (若任一支援語系遺漏鍵名，即視為未完成。)

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
