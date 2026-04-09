# CCH Frontend Development Guidelines (CCH 前端開發規範)

## ⚖️ 1. Core Philosophy (核心哲學)
除了遵循 SOLID 原則外，我們強調以下前端核心價值：
In addition to SOLID principles, we emphasize the following frontend core values:

- **S - Single Responsibility (單一職責)**: 一個組件只做一件事。UI 歸 UI，邏輯歸 Hooks/Services。 (One component does one thing. UI is UI, logic belongs to Hooks/Services.)
- **O - Open/Closed (開閉原則)**: 使用 `Slots` 與 `Props` 擴充組件功能，而非直接修改內部原始碼。 (Use `Slots` and `Props` to extend component functionality instead of modifying internal source code.)
- **D - Dependency Inversion (依賴倒置)**: 組件應依賴介面或抽象服務，而非具體的 API 實作（使用 Services 層隔離）。 (Components should depend on interfaces or abstract services, not concrete API implementations.)

## 🌐 2. i18n & Text Mandate (多國語系規範)
- **Zero Hardcoding**: 所有的模板文字 **絕不允許** 硬編碼。必須使用 `$t()` 或 `t()`。 (All template text **MUST NOT** be hardcoded. Use `$t()` or `t()`.)
- **Bilingual Context**: 所有的 i18n 鍵名必須同步更新 `en.json`, `zh-TW.json`, `zh-CN.json`。 (All i18n keys must be updated synchronously in `en.json`, `zh-TW.json`, and `zh-CN.json`.)

## 🏗️ 3. Vue 3 Composition API Best Practices
- **Script Setup**: 優先使用 `<script setup>` 語法。 (Prioritize using `<script setup>` syntax.)
- **Composable Extraction**: 超過 30 行的業務邏輯必須提取至單獨的 `useXXX.ts` (Hooks)。 (Business logic exceeding 30 lines must be extracted into a separate `useXXX.ts` hook.)
- **Props/Emits Definition**: 必須使用 `defineProps<{...}>` 與 `defineEmits<{...}>` 進行強型別定義。 (Must use `defineProps` and `defineEmits` for strong typing.)

## 🧪 4. Testing SOP (測試標準作業程序)
- **Coverage Requirement**: 每個頁面 (`Views`) 與複雜組件 (`Components`) 必須擁有對應的 `.spec.ts` 測試檔案。 (Every page and complex component must have a corresponding `.spec.ts` test file.)
- **Testing Tools**: 使用 `Vitest` 進行單元測試，`Vue Test Utils` 進行組件掛載測試。 (Use `Vitest` for unit tests and `Vue Test Utils` for component mounting tests.)

## 📁 5. Directory Responsibility (目錄職責)
- `src/views/`: 只負責頁面佈局與調度 Hooks。 (Only responsible for page layout and dispatching hooks.)
- `src/services/`: 負責資料處理、API 呼叫、狀態轉換（不含 UI）。 (Responsible for data processing, API calls, and state transitions - no UI.)
- `src/components/common/`: 高度抽象、無副作用的共用組件。 (Highly abstract, side-effect-free shared components.)

---
*遵循此規範以確保 CCH 專案的長期維護性。*
*Follow these guidelines to ensure long-term maintainability of the CCH project.*
