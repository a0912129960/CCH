---
name: cch-business-governance
description: Enforces CCH project's Business Logic, Architecture, and AI SOP. Use this skill when making any code changes, refactoring, or adding features to ensure strict adherence to the 7-State Classification Lifecycle and the "Three-No Ironclad Covenant".
---

# CCH Project Business Governance (CCH 專案業務治理)

> **MANDATORY**: You MUST adhere to this skill for all CCH project tasks. Failure to comply is a breach of the Project Constitution.
> **強制性**：在所有 CCH 專案任務中必須遵循此技能。未遵守即違反專案憲法。

## 1. Core Mandates (核心授權)

### 1.1 The "Three-No Ironclad Covenant" (三不重回鋼鐵契約)
1.  **No Unauthorized Refactoring (不准擅自重構)**: Surgical edits only. Do not rewrite existing working logic unless explicitly directed. (僅限外科手術式修改。除非明確指示，否則不准重寫現有的運作邏輯。)
2.  **No Overwriting Historical Remarks (不准覆蓋歷史 Remark)**: Existing `Update by...` comments are READ-ONLY. Append new remarks only. (現有的 `Update by...` 註釋為唯讀。僅限附加新的註釋。)
3.  **No Reverting User-Corrected Code (不准回滾使用者修正過的程式碼)**: Current code on disk is the "Source of Truth". (磁碟上的現狀程式碼即為「事實來源」。)

### 1.2 Instruction Mandate (指令規範)
- **Bilingual Communication (雙語溝通)**: All technical explanations, plan summaries, and code comments MUST be provided in both **Traditional Chinese (繁體中文)** and **English**. This applies to AI-Human communication and code remarks ONLY. (溝通與註解僅限繁體中文與英文。)
- **Trilingual Implementation (三語實作)**: ALL application i18n keys (locales/*.json) MUST sync across `en`, `zh-TW`, and `zh-CN`. (程式多語系必須同步英文、繁體中文與簡體中文。)

---

## 2. Change Tracking & Audit Gates (變更追蹤與審核閘門)

Before calling any `replace` or `write_file` tools for logic changes, you MUST present the mandatory audit block and obtain explicit user approval. (在針對邏輯變更調用任何 `replace` 或 `write_file` 工具前，您必須呈現強制性的稽核區塊並獲得使用者明確核准。)

**Audit Block Template:**
```markdown
### 🛡️ Audit Gate (稽核閘門)
- **User (執行者)**: [Your Name/AI]
- **Date (日期)**: [YYYY-MM-DD]
- **Ticket/Issue (票號)**: [Ticket ID]
- **Intent (意圖)**: [Brief description of the change]
- **Impact (影響範圍)**: [List of affected components/services]
```

---

## 3. Business Logic: 7-State Classification Lifecycle (業務邏輯：七階歸類生命週期)

Ensure all classification-related logic respects these states:
1.  **UNKNOWN**: New part detected.
2.  **PENDING_CUSTOMER**: Waiting for customer HTS submission.
3.  **PENDING_REVIEW**: Waiting for Dimerco review.
4.  **RETURNED**: Rejected by Dimerco with reasons.
5.  **ACTIVE**: Agreed and ready for customs.
6.  **FLAGGED**: Targeted for re-evaluation.
7.  **SUPERSEDED**: Replaced or soft-deleted.

---

## 4. Engineering Standards (工程標準)

### 4.1 Legacy Code Preservation (舊程式碼保存)
**NEVER DELETE** old logic. Comment it out using `/* ... */` and add a remark. (絕不刪除舊邏輯。使用 `/* ... */` 將其註解掉並加上註釋。)

### 4.2 N-Tier Architecture Enforcement
- **Backend**: C# 14 / .NET 10. Follow Controller -> Service -> Repository pattern.
- **Frontend**: Vue 3 + TypeScript + Pinia. Use `common/` library components first.

---

## 5. Verification (驗證)
- **Constitutional Verification (憲法級驗證)**: Testing is a MANDATORY part of verifying the correctness of the results. (測試是驗證結果正確性不可或缺的強制性環節。)
- **Test-Driven Delivery**: No code change is complete without corresponding unit/integration tests.
- **New Component Mandate**: For every new view, component, or service, a corresponding test file MUST be created. (針對新視圖/組件/服務必須建立對應的測試檔案。)
- **Linting**: Run `npm run lint` (frontend) or `dotnet build` (backend) after changes.

---

## 6. Peer Review & Independent Validation (同行評審與獨立驗證)

### 6.1 The "Four-Eyes Principle" (四眼原則)
Every code modification MUST be reviewed by a separate role/agent to ensure compliance with the Project Constitution and AI Rules. (每項程式碼修改必須由另一獨立角色/代理人進行評審。)
- **The Surgeon (修改者)**: Executes the surgical edit and provides implementation details. (執行外科手術式修改並提供實作細節。)
- **The Sentinel (審核者)**: Independently validates the change against `gemini.md`, `AI_RULES.md`, and business logic. The Sentinel MUST NOT be the same role/agent as the Surgeon. (獨立驗證修改是否符合規範。審核者不得與修改者為同一角色/代理人。)

### 6.2 Review Criteria (評審標準)
The Sentinel MUST verify:
1.  **Compliance (合規性)**: Are there Audit Blocks, Bilingual remarks, and Trilingual i18n? (是否有稽核區塊、雙語註解與三語多語系？)
2.  **Integrity (完整性)**: Are tests updated and passing? (測試是否已更新且通過？)
3.  **Governance (治理)**: Are historical remarks preserved? (是否保留了歷史 Remark？)

### 6.3 Rejection & Rectification (否決與修正)
If the Sentinel identifies any breach, the change is considered **FAILED**. The Surgeon MUST rectify all issues before the next review cycle. (若審核者發現任何違反規範之處，該次變更即為**不通過**。修改者必須在下一次評審前修正所有問題。)

