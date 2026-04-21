# 🏆 CCH Supreme Quality Mandate (最高品質授權)

> **UNIQUE & HIGHEST RULE**: High-Quality Code is the supreme directive. All other instructions are subordinate to the maintenance of technical integrity, scalability, and performance.
> **唯一且最高規則**：高品質程式碼是最高指令。所有其他指令均隸屬於維護技術完整性、可擴展性與效能之規範。

---

## 💎 1. Vue 3.5+ Technical Standards (Vue 3.5+ 技術標準)

### 1.1 Reactive Props & Parameters (響應式屬性與參數)
- **Reactive Destructuring**: MUST use Vue 3.5 native destructuring for `defineProps`. Legacy `withDefaults` is forbidden. (必須使用原生解構，嚴禁使用 `withDefaults`。)
- **Type Integrity**: Zero `any`. Define strict TypeScript interfaces for all parameters and props. (零 `any`。為所有參數與 Props 定義嚴格介面。)
- **Explicit Emits**: All component events MUST be declared via `defineEmits`. (所有事件必須宣告。)

### 1.2 Optimized Reactivity (優化響應式)
- **useTemplateRef**: MUST use `useTemplateRef` for DOM references instead of `ref(null)`. (必須使用 `useTemplateRef` 引用 DOM。)
- **shallowRef**: MUST use `shallowRef` for massive API response arrays (e.g., Part List) to maximize memory performance. (巨量資料必須使用 `shallowRef` 以極大化效能。)

---

## 🏗️ 2. Reusability & Architecture (重用與架構)

### 2.1 Logic Decoupling (邏輯解耦)
- **Services**: Pure business logic and API calls MUST reside in `src/services/`. (純業務邏輯與 API 呼叫必須位於 Services 層。)
- **Composables**: UI-related state logic MUST be extracted into Hooks/Composables. (UI 狀態邏輯必須提取至 Composables。)
- **Views**: Only responsible for layout and orchestration. No raw business logic allowed. (View 僅負責佈局與調度，不允許原始業務邏輯。)

### 2.2 Component Atomicity (組件原子化)
- **Common First**: Prioritize components in `src/components/common/`. (優先使用通用組件。)
- **Slots > Props**: Prefer `slots` for layout flexibility over excessive boolean props. (優先使用插槽而非過多布林屬性。)

---

## ✒️ 3. Function & Coding Standards (函式與程式碼標準)

### 3.1 Function Rigor (函式嚴謹性)
- **Naming**: 
  - `handleXxx`: For event handlers (事件處理).
  - `fetchXxx` / `getXxx`: For data retrieval (資料獲取).
  - `isXxx` / `hasXxx`: For boolean checks (布林檢查).
- **Single Responsibility**: One function, one purpose. Max **30 lines** per function. (單一職責，每項函式最高 **30 行**。)

### 3.2 Surgical Cleanliness (外科手術式整潔)
- **Zero Redundancy**: Immediately remove unused imports, dead boilerplate, and unnecessary logs. (立即移除未使用的匯入、樣板與日誌。)
- **Surgical Precision**: Minimal code changes. Do not refactor unrelated logic. (最小化變更，不重構無關邏輯。)

---

## 🌐 4. Cultural & Linguistic Integrity (文化與語系完整性)

### 4.1 The Trilingual Ironclad Rule (三語鋼鐵準則)
- **Zero Hardcoding**: Every user-facing string MUST use `$t()` or `t()`. (所有使用者文字必須使用 i18n。)
- **Atomic Sync**: Update `en.json`, `zh-TW.json`, and `zh-CN.json` simultaneously. Partial updates are a failure. (同步更新英、繁、簡三語。局部更新即視為失敗。)

### 4.2 Bilingual Communication (雙語溝通)
- **Bilingual Mandate**: All code remarks (`Update by...`, `INTERNAL-AI...`) and AI summaries MUST be in **Traditional Chinese** and **English**. (所有註解與摘要必須同時使用繁體中文與英文。)

---

## 🧪 5. Verification & Audit (驗證與稽核)

### 5.1 Verification is Truth (驗證即真理)
- **Mandatory TDD**: No change is complete without a corresponding `.spec.ts` test that passes 100%. (若無 100% 通過的測試，變更不算完成。)
- **Linting**: Run `npm run lint` or `vue-tsc` before completion. (完工前執行檢查。)

### 5.2 Legacy Preservation (舊代碼保存)
- **Comment, Don't Delete**: Comment out business logic using `/* ... */` or `<!-- ... -->`. (業務邏輯僅限註解，不可刪除。)
- **Read-Only Remarks**: Existing history remarks are immutable. (現有歷史備註不可變更。)

---
*Initialized on 2026-04-21: The Supreme Quality Mandate takes effect immediately.*
