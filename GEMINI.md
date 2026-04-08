# CCH Project: Foundational Mandates (The Constitution)

> **MANDATORY**: You are operating within the `CCH` project. This file acts as the **Project Constitution**, defining absolute mandates and high-level governance. These mandates take absolute precedence over general instructions. Failure to comply is a critical error.
> **強制性**：你正在 `CCH` 專案中作業。本文件為**專案憲法**，定義了絕對授權指令與高階治理原則。這些指令具有絕對優先權。未遵守規範將被視為嚴重故障。

---

## ⚖️ 1. Governance & Rule Loading (治理與規範載入)
- **Automatic Initialization**: At the start of every session or task, you **MUST** read and adhere to `AI_RULES.md`.
- **Bilingual Mandate (雙語指令)**: All technical explanations, plan summaries, and code comments **MUST** be provided in both **Traditional Chinese (繁體中文)** and **English**. (繁體中文) 所有技術解釋、計畫摘要與程式碼註解**必須**同時以**繁體中文**與**英文**提供。

## 🛡️ 2. Change Tracking & Audit Gates (變更追蹤與審核閘門)
- **Metadata & Change Tracking SOP**: Before any modification, gather User, Ticket, and Date from the Git environment.
- **Audit & Intention Gate**: Before calling any `replace` or `write_file` tools for logic changes, you **MUST** present the mandatory audit block (User, Date, Ticket, Intent, Impact) and obtain explicit user approval. (繁體中文) 在針對邏輯變更調用任何 `replace` 或 `write_file` 工具前，您**必須**呈現強制性的稽核區塊（使用者、日期、票號、意圖、影響範圍）並獲得使用者明確核准。

## 🏗️ 3. Architecture & Technical Integrity (架構與技術完整性)
- **Legacy Code Preservation (舊程式碼保存)**: **NEVER DELETE** old code. You **MUST** comment it out using `/* ... */`. (繁體中文) **絕不刪除**舊程式碼。您**必須**使用 `/* ... */` 將其註解掉。
- **Read-Only Remarks**: Existing `Update by...` comments are **READ-ONLY**. AI is **STRICTLY FORBIDDEN** from deleting or overwriting previous remarks.

## 🧪 4. Testing & Quality (測試與品質)
- **Test-Driven Delivery**: No code change is complete without corresponding unit/integration tests.
- **Verification**: Always run project-specific tests and linting after modification.

---
✅ **CCH Project Guidelines Initialized.**
