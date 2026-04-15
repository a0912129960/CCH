# CCH AI Operational Rules (AI 作業規範)

## ⚖️ 1. Core Mandates (核心授權)
- **Automatic Rule Adherence**: This file is the primary source of truth for all AI operations. (本文件為所有 AI 作業的主要事實來源。)
- **Bilingual Mandate (雙語指令)**: All technical explanations, plan summaries, and code remarks MUST be provided in both **Traditional Chinese (繁體中文)** and **English**. (所有技術解釋、計畫摘要與程式碼註解必須同時以繁體中文與英文提供。)
- **Application I18N Synchrony (程式多語系同步)**: 
  - **Zero Hardcoding**: NEVER hardcode UI strings in components or scripts. (嚴禁在組件或腳本中硬編碼 UI 字串。)
  - **Atomic Updates**: When modifying UI strings, the AI MUST update ALL supported language files in `src/locales/` (en.json, zh-TW.json, zh-CN.json) simultaneously. **Partial updates are strictly forbidden.** (修改 UI 字串時，必須同時更新所有語系檔，嚴禁局部更新。)
- **Surgical Integrity**: Maintain minimal code changes. Do not refactor unrelated logic. (保持最小程式碼變動。不重構無關邏輯。)

## 🛠️ 2. Commenting & Code Standards (註解與程式碼標準)
- **Legacy Code Preservation**: **NEVER DELETE** old code. You MUST comment it out using `/* ... */` for Scripts or `<!-- ... -->` for Templates. (絕不刪除舊程式碼，必須將其註解。)
- **Read-Only Remarks**: Existing `Update by...` comments are READ-ONLY. AI is strictly forbidden from deleting or overwriting previous remarks. (現有的 `Update by...` 註釋為唯讀，嚴禁刪除或覆蓋。)
- **Domain Categorization**: Every new file MUST be placed in its corresponding domain folder (e.g., `views/part/`, `services/auth/`). (新檔案必須放在對應的領域資料夾。)

## 🛡️ 3. Audit & Ticket Logic (稽核與票號邏輯)
- **Audit Gate**: Before calling `replace` or `write_file` for logic changes, present the audit block (User, Date, Ticket, Intent, Impact) and obtain explicit user approval. (邏輯變更前必須呈現稽核區塊並獲得核准。)
- **Ticket IDs**: Use provided Ticket ID or generate `INTERNAL-AI-[YYYYMMDD]`.

## 🧪 4. Testing & Verification (測試與驗證)
- **Verification is Truth**: No implementation is considered correct until validated by tests. (所有實作在通過測試驗證前皆不視為正確。)
- **Mandatory TDD**: Write or update tests for every logic or UI change. (每項邏輯或 UI 變更皆須撰寫或更新測試。)
- **Linting**: Ensure `npm run lint` passes before claiming "Done".

---
*Refactored on 2026-04-15 by Gemini AI (Unified Governance)*
