# 🧪 CCH Frontend Testing Rules (CCH 前端測試規範)

## ⚖️ 1. Core Principles (核心原則)
- **Atomic Requirement (原子化要求)**: Creating a View or Component is NOT complete without its corresponding test. 
  - **原則**: 建立 View 或 Component 時，若無對應測試即視為未完工。
- **Bilingual Mandate (雙語指令)**: All test descriptions (`describe`, `it`) MUST be in both **Traditional Chinese (繁體中文)** and **English**.
- **Isolation**: Each test should be independent. Use `vi.mock` for external services.

## 📁 2. File Naming & Location (檔案命名與位置)
- **Location**: Test files MUST be placed in a `__tests__` directory at the same level as the source file.
  - **位置**: 測試檔案必須放置在與原始檔同級的 `__tests__` 資料夾中。
- **Naming**: `[FileName].spec.ts`
  - **命名**: `[原始檔名].spec.ts`
- **Example Structure (範例結構)**:
  ```text
  src/views/
  ├── LoginView.vue
  └── __tests__/
      └── LoginView.spec.ts
  ```

## 🛠️ 3. Testing Stack (測試技術棧)
- **Runner**: Vitest
- **Library**: @vue/test-utils
- **Mocking**: `vi.mock()`, `vi.fn()`

## 📝 4. Test Case Examples (測試案例範例)
- **Rendering**: Verify initial UI state and i18n keys.
- **Logic**: Verify input changes, button clicks, and service calls.
- **Routing**: Verify redirection on success/failure.

---
*Updated on 2026-04-09 by Gemini CLI (Mandatory Testing Policy)*
