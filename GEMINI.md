# CCH Project: Foundational Mandates (The Constitution)

> **MANDATORY**: You are operating within the `CCH` project. This file acts as the **Project Constitution**, defining absolute mandates and high-level governance. These mandates take absolute precedence over general instructions. Failure to comply is a critical error.
> **強制性**：你正在 `CCH` 專案中作業。本文件為**專案憲法**，定義了絕對授權指令與高階治理原則。這些指令具有絕對優先權。未遵守規範將被視為嚴重故障。

---

## ⚖️ 1. Governance & Rule Loading (治理與規範載入)
- **Automatic Initialization**: At the start of every session or task, you **MUST** read and adhere to `AI_RULES.md`.
- **Bilingual Mandate (雙語指令)**: All technical explanations, plan summaries, and code **remarks** **MUST** be provided in both **Traditional Chinese (繁體中文)** and **English**. (繁體中文) 所有技術解釋、計畫摘要與程式碼註解 (Remark) **必須**同時以**繁體中文**與**英文**提供。
- **Application I18N Synchrony (程式多語系同步)**: When modifying UI strings, the AI **MUST** update ALL supported language files in `src/locales/` (e.g., `en.json`, `zh-TW.json`, `zh-CN.json`) simultaneously. **Partial updates are strictly forbidden.** (繁體中文) 修改 UI 字串時，**必須**同時更新 `src/locales/` 下的所有語系檔，嚴禁漏掉任何語系。

## 🛡️ 2. Change Tracking & Audit Gates (變更追蹤與審核閘門)
- **Metadata & Change Tracking SOP**: Before any modification, gather User, Ticket, and Date from the Git environment.
- **Audit & Intention Gate**: Before calling any `replace` or `write_file` tools for logic changes, you **MUST** present the mandatory audit block (User, Date, Ticket, Intent, Impact) and obtain explicit user approval. (繁體中文) 在針對邏輯變更調用任何 `replace` 或 `write_file` 工具前，您**必須**呈現強制性的稽核區塊（使用者、日期、票號、意圖、影響範圍）並獲得使用者明確核准。

## 🏗️ 3. Architecture & Technical Integrity (架構與技術完整性)
- **Legacy Code Preservation (舊程式碼保存)**: **NEVER DELETE** old code. You **MUST** comment it out using `/* ... */`. (繁體中文) **絕不刪除**舊程式碼。您**必須**使用 `/* ... */` 將其註解掉。
- **Read-Only Remarks**: Existing `Update by...` comments are **READ-ONLY**. AI is **STRICTLY FORBIDDEN** from deleting or overwriting previous remarks.

## 🧪 4. Testing & Verification Pillar (測試與驗證支柱)
- **Verification is Truth (驗證即真理)**: Testing is an essential part of verifying that results are correct. No implementation is considered correct until validated by tests. (繁體中文) 測試是驗證結果正確性不可或缺的一環。所有實作在通過測試驗證前皆不視為正確。
- **Test-Driven Delivery**: No code change is complete without corresponding unit/integration tests. If a component/service is modified, its test file **MUST** be updated. If new, it **MUST** be created. (繁體中文) 任何邏輯變更必須包含測試更新。
- **Mandatory Finality**: A task is **ONLY** considered "Done" after successful project-specific tests and linting. (繁體中文) 任務完成的唯一證明是成功的測試結果。

---
✅ **CCH Project Guidelines Initialized.**
