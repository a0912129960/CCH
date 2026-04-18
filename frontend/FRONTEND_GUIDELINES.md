# CCH Frontend Development Guidelines (CCH 前端開發規範)

## 📁 2. Directory Structure & Categorization (目錄結構與分類)

All files MUST be organized by **Business Domain** to ensure scalability. (所有檔案必須依據「業務領域」進行組織。)

### 2.1 Views (`src/views/`)
Group views by their primary business context:
- `auth/`: Login, Registration, Password reset.
- `part/`: All part-related management pages.
- `customer/`: Customer-specific dashboards.
- `employee/`: Employee-specific dashboards.

### 2.2 Services (`src/services/`)
Match the structure of views:
- `auth/`: Authentication logic.
- `part/`: Part-related API calls.
- `dashboard/`: Analytics and summary data.

### 2.3 Components (`src/components/`)
- `common/`: Highly reusable UI elements (Button, Card, Dot).
- `features/[domain]/`: UI components specific to a domain (e.g., `features/part/PartFilter.vue`).

---

## ⚖️ 3. Core Philosophy (核心哲學)
除了遵循 SOLID 原則外，我們強調以下前端核心價值：
In addition to SOLID principles, we emphasize the following frontend core values:

- **S - Single Responsibility (單一職責)**: 一個組件只做一件事。UI 歸 UI，邏輯歸 Hooks/Services。 (One component does one thing. UI is UI, logic belongs to Hooks/Services.)
- **O - Open/Closed (開閉原則)**: 使用 `Slots` 與 `Props` 擴充組件功能，而非直接修改內部原始碼。 (Use `Slots` and `Props` to extend component functionality instead of modifying internal source code.)
- **D - Dependency Inversion (依賴倒置)**: 組件應依賴介面或抽象服務，而非具體的 API 實作（使用 Services 層隔離）。 (Components should depend on interfaces or abstract services, not concrete API implementations.)

## 🛡️ 4. Component Import & Automation Control (組件匯入與自動化控管)
為了平衡開發速度與系統穩定性，我們對 `unplugin` 插件的使用採取「分層策略」：
To balance development speed and system stability, we adopt a "layered strategy" for using `unplugin` plugins:

### 4.1 Layout Components (佈局層組件)
核心佈局組件（如 `App.vue` 中的 `Sidebar`, `AppHeader`, `AppFooter`, `AppTabs`, `Loading`）**必須手動顯式匯入 (MUST use Explicit Import)**。
- **原因 (Rationale)**: 確保應用的地基不受自動化工具失效、緩存或配置偵測問題影響。 (Ensure the application's foundation is unaffected by automation tool failures, caching, or configuration detection issues.)

### 4.2 Atomic Components (原子層組件)
通用 UI 組件（如 `el-button`, `AppCard`, `AppInput`）與 Vue/Pinia 函式（如 `ref`, `computed`, `storeToRefs`）**建議使用自動匯入 (Recommended use of Auto Import)**。
- **原因 (Rationale)**: 減少冗餘代碼，提升開發效率。 (Reduce redundant code and improve development efficiency.)

### 4.3 Naming Conventions (命名規範)
- **Component Files**: Use `PascalCase` (e.g., `Sidebar.vue`).
- **Template Tags**: Use `PascalCase` for component tags to distinguish them from standard HTML elements (e.g., `<Sidebar />`).
- **i18n Keys**: Use `snake_case` (e.g., `common.save_success`).

### 4.4 Path Alias Usage (路徑別名使用)
- **規範**: 跨目錄的匯入（深度超過一層 `../`）**必須使用路徑別名** `@src/` 或 `@/`。
- **Rationale**: 提升程式碼在目錄重構時的魯棒性與可讀性。
  - **Bad**: `import { x } from '../../services/auth/auth';`
  - **Good**: `import { x } from '@src/services/auth/auth';`

### 4.5 Zero Commented-Out Imports (禁止保留被註解的匯入)
嚴禁在程式碼中保留被註解掉的 `/* import ... */`。這會干擾插件偵測並誤導維護者。
- **規範**: 如果不再需要該匯入，請直接刪除；如果需要，請取消註解。 (If an import is no longer needed, delete it directly; if it is needed, uncomment it.)

## 🌐 2. i18n & Text Mandate (多國語系規範)
- **Zero Hardcoding**: 所有的模板文字 **絕不允許** 硬編碼。必須使用 `$t()` 或 `t()`。 (All template text **MUST NOT** be hardcoded. Use `$t()` or `t()`.)
- **Trilingual Synchronicity (三語同步)**: 所有的 i18n 鍵名必須 **同時** 更新於 `en.json`, `zh-TW.json`, 與 `zh-CN.json`。缺少任一語系即視為開發未完成。 (All i18n keys **MUST** be updated simultaneously in `en.json`, `zh-TW.json`, and `zh-CN.json`. Missing any language is considered incomplete development.)

## 🏗️ 3. Vue 3.5+ Modern Standards (Vue 3.5+ 現代化標準)
我們遵循 2026 年最新的 Vue 3.5 最佳實作以確保極致效能與開發體驗：
We follow the latest 2026 Vue 3.5 best practices for peak performance and DX:

### 3.1 Reactive Props Destructuring (Props 響應式解構)
- **規範**: 應直接從 `defineProps` 中解構屬性，並利用原生的 JS 預設值。
- **Rationale**: Vue 3.5 已支援響應式解構，不再需要 `withDefaults`。
  ```typescript
  const { title = 'Default', count = 0 } = defineProps<{ title?: string, count?: number }>();
  ```

### 3.2 Template Refs (`useTemplateRef`)
- **規範**: 禁止使用 `ref(null)` 來引用 DOM 元素，必須使用 `useTemplateRef()`。
- **Rationale**: 區分響應式數據與 DOM 引用，提升類型安全性。
  ```typescript
  const inputEl = useTemplateRef<HTMLInputElement>('my-input');
  ```

### 3.3 Large Dataset Optimization (`shallowRef`)
- **規範**: 針對從 API 獲取的巨量、不可變數據（如 `parts.value`），必須使用 `shallowRef` 替代 `ref`。
- **Rationale**: Vue 3.5 對大型陣列的處理效能提升，配合 `shallowRef` 可減少 50% 以上的記憶體開銷。

### 3.4 Unique ID Generation (`useId`)
- **規範**: 所有的表單元素與無障礙 ID 必須使用 `useId()` 產生。
- **Rationale**: 確保 SSR 水合一致性與全域唯一性。

## 🧪 4. Testing SOP (測試標準作業程序)
- **Atomic Delivery (原子化交付)**: 建立任何頁面 (`Views`) 或組件 (`Components`) 時，**必須同時** 建立對應的 `.spec.ts` 測試檔案。 (When creating any page (`Views`) or component (`Components`), a corresponding `.spec.ts` test file **MUST** be created at the same time.)
- **Testing Rules**: 詳細規則請參閱 `TESTING_RULES.md`。 (Refer to `TESTING_RULES.md` for detailed rules.)

## 🎨 5. Style Guidelines (視覺風格規範)
- **Brand Consistency**: 視覺風格必須符合 MyDimerco 品牌語言。 (Visual style must comply with MyDimerco brand language.)
- **Detailed Style Guide**: 詳細規格請參閱 `STYLE_GUIDELINES.md`。 (Refer to `STYLE_GUIDELINES.md` for detailed specifications.)

## 📁 6. Directory Responsibility (目錄職責)
- `src/views/`: 只負責頁面佈局與調度 Hooks。 (Only responsible for page layout and dispatching hooks.)
- `src/services/`: 負責資料處理、API 呼叫、狀態轉換（不含 UI）。 (Responsible for data processing, API calls, and state transitions - no UI.)
- `src/components/common/`: 高度抽象、無副作用的共用組件。 (Highly abstract, side-effect-free shared components.)

---
*遵循此規範以確保 CCH 專案的長期維護性。*
*Follow these guidelines to ensure long-term maintainability of the CCH project.*
