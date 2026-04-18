# CCH AI Operational Rules (AI 作業規範)

## ⚖️ 1. Core Mandates (核心授權)
- **Automatic Initialization**: At the start of every session, AI MUST read and internalize `FRONTEND_GUIDELINES.md`, `STYLE_GUIDELINES.md`, and `TESTING_RULES.md`. (AI 在每次作業開始時，必須讀取並內化所有規格文件。)
- **Automatic Rule Adherence**: This file is the PRIMARY Source of Truth. AI MUST verify every action against these rules before execution. (本文件為主要事實來源。AI 在執行前必須針對這些規則驗證每項行動。)
- **Constitutional Awareness (規則感知)**: AI is forbidden from treating rules as optional suggestions. Violation of rules is a failure of the objective. (嚴禁將規則視為選配建議。違反規則即視為任務失敗。)

## 🛠️ 2. Change Tracking & Self-Audit (變更追蹤與自我稽核)
- **Mandatory Self-Audit**: After each modification, AI MUST perform a mental self-audit:
  1. Is it bilingual? (是否雙語？)
  2. Are all 3 i18n locales updated? (3 個語系是否同步？)
  3. Is there a corresponding test? (是否有對應測試？)
  4. Are core layout components explicitly imported? (核心佈局是否手動匯入？)
  5. Are Vue 3.5+ standards applied (Props destructuring, etc.)? (是否套用 Vue 3.5+ 標準？)
- **Zero Commented-Out Logic**: Redundant boilerplate MUST be removed. Surgical precision is required. (冗餘樣板必須移除。)
- **Bilingual Mandate (雙語指令)**: All technical explanations, plan summaries, and code remarks MUST be provided in both **Traditional Chinese (繁體中文)** and **English**. (所有技術解釋、計畫摘要與程式碼註解必須同時以繁體中文與英文提供。)
- **Application I18N Synchrony (程式多語系同步)**: 
  - **Zero Hardcoding**: NEVER hardcode UI strings in components or scripts. (嚴禁在組件或腳本中硬編碼 UI 字串。)
  - **Atomic Updates**: When modifying UI strings, the AI MUST update ALL supported language files in `src/locales/` (en.json, zh-TW.json, zh-CN.json) simultaneously. **Partial updates are strictly forbidden.** (修改 UI 字串時，必須同時更新所有語系檔，嚴禁局部更新。)
- **Surgical Integrity**: Maintain minimal code changes. Do not refactor unrelated logic. **When refactoring, surgical precision takes precedence over legacy preservation to ensure file cleanliness.** (保持最小程式碼變動。重構時，外科手術式的精準度優先於舊代碼保存，以確保檔案整潔。)

## 🛠️ 2. Commenting & Code Standards (註解與程式碼標準)
- **Legacy Code Preservation**: **NEVER DELETE** significant business logic. Comment it out using `/* ... */` (Scripts) or `<!-- ... -->` (Templates). **Redundant boilerplate (e.g., unused imports) SHOULD be removed to maintain clarity.** (絕不刪除重要業務邏輯。冗餘的樣板程式碼如未使用的匯入應被移除，以維持清晰度。)
- **Read-Only Remarks**: Existing `Update by...` comments are READ-ONLY. Append new remarks to history. (現有的 `Update by...` 註釋為唯讀。請將新備註附加至歷史。)
- **Domain Categorization**: Every new file MUST be placed in its corresponding domain folder (e.g., `views/part/`, `services/auth/`). (新檔案必須放在對應的領域資料夾。)

## 🛡️ 3. Audit & Ticket Logic (稽核與票號邏輯)
- **Audit Gate**: Before calling `replace` or `write_file` for logic changes, present the audit block.
- **Batching (批次處理)**: For non-logic or minor edits (documentation, styling, typos), the AI may batch multiple changes into a single audit block. (對於非邏輯或微小修正如文件、樣式、拼字，AI 可將多項變更合併於單一稽核區塊。)
- **Ticket IDs**: Use provided Ticket ID or generate `INTERNAL-AI-[YYYYMMDD]`.

## 🧪 4. Testing & Verification (測試與驗證)
- **Verification is Truth**: No implementation is considered correct until validated by tests. (所有實作在通過測試驗證前皆不視為正確。)
- **Mandatory TDD**: Write or update tests for every logic or UI change. (每項邏輯或 UI 變更皆須撰寫或更新測試。)
- **Linting**: Ensure `npm run lint` passes before claiming "Done".

---
*Refactored on 2026-04-15 by Gemini AI (Unified Governance)*
