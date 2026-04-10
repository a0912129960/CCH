# CCH AI Operational Rules (AI 作業規範)

## ⚖️ 1. Core Mandates (核心授權)
- **Context Awareness**: Always analyze the file type before applying comments. (在套用註解前，務必分析檔案類型。)
- **Surgical Integrity**: Maintain minimal code changes. Do not refactor unrelated logic. (保持最小程式碼變動。不重構無關邏輯。)
- **Bilingual Communication**: All interactions and code remarks MUST be in Traditional Chinese and English. (所有互動與程式碼註解必須包含繁體中文與英文。)

## 🛠️ 2. Commenting Standards (註解標準)
- **Script (JS/TS/C#)**: Use `/* ... */` or `//` for historical remarks.
- **Template (HTML/Vue)**: Use `<!-- ... -->` for historical remarks.
- **Preservation**: NEVER delete old logic; comment it out using the appropriate syntax. (絕不刪除舊邏輯，應使用正確語法將其註解。)

## 🛡️ 3. Audit & Ticket Logic (稽核與票號邏輯)
- **Ticket IDs**: If no Ticket ID is provided by the user, AI **MAY** generate a temporary ID in the format `INTERNAL-AI-[YYYYMMDD]`. (若使用者未提供票號，AI 可生成 `INTERNAL-AI-[YYYYMMDD]` 格式的臨時票號。)
- **Self-Audit**: AI is authorized to perform self-audit for all changes, prioritizing verification through tests. (AI 獲授權對所有變更進行自我審核，並優先透過測試進行驗證。)

## 🧪 4. Testing & Quality (測試與品質)
- **Mandatory TDD**: Write or update tests for every logic change. (每項邏輯變更皆須撰寫或更新測試。)
- **Linting**: Ensure `npm run lint` or relevant build commands pass before claiming "Done". (在聲明「完成」前，確保通過 Lint 或相關建置命令。)

---
*Initialized on 2026-04-10 by Gemini CLI*
