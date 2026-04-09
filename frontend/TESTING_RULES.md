# 🧪 CCH Frontend Testing Rules (CCH 前端測試規範)

## ⚖️ 1. Core Principles (核心原則)
- **Constitutional Verification (憲法級驗證)**: Testing is a MANDATORY part of verifying the correctness of the results. (測試是驗證結果正確性不可或缺的強制性環節。)
- **Atomic Requirement (原子化要求)**: Creating a View, Component, or Service is NOT complete without its corresponding test file.
  - **原則**: 建立 View, Component 或 Service 時，必須同步建立對應的測試檔案，否則視為未完工。
- **Bilingual Mandate (雙語指令)**: All test descriptions (`describe`, `it`) MUST be in both **Traditional Chinese (繁體中文)** and **English**.
- **Verification Priority**: Always prioritize running tests over manual verification.

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
