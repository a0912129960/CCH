# CCH Project AI Rules & SOP (基礎規則與標準程序)

> **SINGLE SOURCE OF TRUTH**: This document defines the technical protocol, architecture, and coding standards.
> **單一事實來源**：本文件定義了技術協議、架構與編碼標準。

---

## 0. AI Core Protocol (核心協議)

### 0.1 🔴 七大嚴禁行為 (Anti-Patterns)
1. **憑空猜測 (No Guessing)**: 嚴禁在沒有事實依據前使用「應該是」、「推測」。
2. **甩鍋用戶 (No Burden Shifting)**: 嚴禁要求用戶自行檢查，必須先使用工具自行查證。
3. **只出嘴不出手 (Action Oriented)**: 嚴禁僅給出方向性建議而不提供具體實作草稿。
4. **有工具不用 (Use Tools)**: 嚴禁盲目依靠內建記憶，必須使用 `read_file`, `grep_search` 等工具。
5. **空手提問 (Inquiry with Evidence)**: 提問前必須先附上已查到的進度與數據。
6. **盲目重試 (Fail Fast & Pivot)**: 連續失敗 2 次必須停下來換方向。
7. **未驗證即交付 (Verify Before Delivery)**: 修改後必經驗證（編譯、測試、Lint）始得回報完成。
8. **同步更新測試 (Sync Test Updates)**: 任何邏輯或 UI 變更必須同步更新對應的 `.spec` 或測試檔，嚴禁讓程式碼與測試脫節。

### 0.2 🛡️ Surgical Update Protocol (手術級更新協議)
- **Read-Before-Edit**: 修改前必須重新讀取檔案內容，確保資訊不失真。
- **Precision Replacement**: `replace` 時應僅包含變動核心區塊，避免冗餘碼包入。
- **Atomic Verification Chain (原子驗證鏈)**: AI 執行 `replace` 後，**必須**執行以下鏈路：
  1. **Visual Source Audit (視覺化源碼審計)**：強制調用 `read_file` 重新讀取修改後的區域，確認標籤成對性（如 `el-select` 內部必須全是 `el-option`）與語法正確性。
  2. **Automated Testing**：執行相關測試指令，確保邏輯通過。
  3. **Linting**：確保無格式錯誤。
  不得延遲驗證。
- **No Comment Stacking**: 嚴禁將已註解程式碼再次包入新的 `/* ... */` 註解中。

---

## 1. Technical Standards (技術規範)

### 1.1 Architecture & Layering (架構與分層)
- **Layered Structure**: Adhere to N-Tier (Controller -> Service -> Repository).
- **Service Responsibility**: Business logic stays in Services. Controllers are just gateways.
- **Naming Convention**:
  - `PascalCase` for Public Methods/Classes.
  - `camelCase` for variables and private fields.

### 1.2 Change Tracking SOP (強制性變更追蹤)
- **Metadata Template**:
  `/// Update by [User] at [YYYY/MM/DD] [Ticket-Number] [Summary]`
- **Audit Gate Block (MANDATORY BEFORE EXECUTION)**:
  > ### 🛡️ Pre-Execution Audit (執行前稽核)
  > - **User (執行者)**: ...
  > - **Date (日期)**: ...
  > - **Ticket (票號)**: ...
  > - **Intent (變更意圖)**: ...
  > - **Impact (影響範圍)**: ...
  > **Confirm to proceed? (確認執行？)**

---

## 2. Refactoring & Testing (重構與測試)

### 2.1 Refactoring Triage (重構分類)
- **Unauthorized Refactoring**: AI is strictly forbidden from unauthorized refactoring of legacy code.
- **Dependency Limit**: Maximum 6 dependencies for legacy services, 4 for new services. Exceeding triggers mandatory refactoring plan.

### 2.2 Testing Requirements
- **Mandatory Test Cases**: Happy Path, Field Validation, Boundary, Side Effects.
- **Anti-Over-Stubbing (嚴禁過度模擬)**: 對於關鍵的交互組件（如語言切換、表單提交），應減少對 UI 框架組件的 Stub，改用整合測試或增加對子組件類型的斷言，防止測試通過但 UI 報銷。
- **Test Integrity**: Never modify business logic to "make tests pass."

---
✅ **CCH Technical Rules Initialized.**
