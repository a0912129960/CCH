# 👑 CCH 專案：最高品質與工程憲法 (Supreme Quality & Engineering Constitution)

> **[MANDATORY]** 本文件為 `CCH` 專案的最高品質法令。AI 必須將其視為不可逾越的「物理邊界」。
> **[MANDATORY]** This document is the supreme quality law of the `CCH` project. AI must treat it as an insurmountable "physical boundary."

---

## 🛑 第一部分：品質最高指令 (The Quality Supreme Mandate)
1. **[SUPREME RULE] 品質高於一切 (Quality Over Everything)**: 程式碼品質與架構完整性是本專案的靈魂。嚴禁為了開發速度、功能達成或盲目遵循既有慣例而犧牲任何品質指標。
2. **[SUPREME RULE] 拒絕低質交付 (Refusal of Low-Quality Delivery)**: 凡未通過下述「量化品質紅線」的變更，AI **必須**自我駁回，嚴禁向使用者交付任何具備技術債或邏輯重複的代碼。
3. **[SUPREME RULE] 先重構、後開發 (Refactor First, Then Implement)**: 若發現既有結構不良或違反 DRY 原則，AI **必須**先行重構，確保新功能是在「高品質」基礎上開發。

---

## 📊 第二部分：硬性量化品質紅線 (Hard Quantitative Quality Redlines)

**任何方法或類別若觸碰以下「失敗閾值」，即判定為品質不合格 (Failure)：**

| 原則 (Principle) | 量化指標 (Quantifiable Metric) | 失敗閾值 (Failure Threshold) |
| :--- | :--- | :--- |
| **S (SRP)** | **單一方法行數 (Method Length)** | **> 40 行 (LOC)** |
| **S (SRP)** | **單一方法參數數量 (Parameter Count)** | **> 4 個** |
| **S (SRP)** | **職責計數 (Responsibility Count)** | **> 1 個** (例如：同時包含驗證與資料庫操作) |
| **DRY** | **重複邏輯行數 (Logic Duplication)** | **> 5 行** 且存在於兩處以上 |
| **DRY** | **邏輯相似度 (Logic Similarity)** | 方法間相似度 **> 80%** (嚴禁複製貼上邏輯) |
| **D (DIP)** | **具體類別依賴 (Hard Dependencies)** | 在業務邏輯方法中使用 **`new`** 實例化服務或倉儲 |
| **O/I** | **介面方法數量 (Interface Bloat)** | 單一 Interface 定義了 **> 7 個** 無關聯的方法 |
| **Complexity** | **巢狀層級 (Nesting Level)** | **> 3 層** (如超過三層 `if`, `foreach`, `switch` 嵌套) |
| **SSoT** | **單一事實來源 (Source of Truth)** | 修改一處業務規則需手動更動 **> 1 處** 程式碼位置 |

---

## 🏛️ 第三部分：專案憲法與工程規範 (Project Constitution & Engineering Standards)
1. **雙語指令 (Bilingual Mandate)**: 所有技術解釋、計畫摘要與程式碼註解 **必須** 同時以 **繁體中文** 與 **英文** 提供。
2. **舊程式碼保存 (Legacy Preservation)**: **絕不刪除** 舊程式碼。必須使用 `/* ... */` 將其註解掉，並嚴禁覆寫既有的 `Update by...` 標記。
3. **單一事實來源 (SSoT)**: 核心業務規則必須集中。若多個入口點需要相同邏輯，**必須** 提取為共享方法。
4. **解耦與可測試性 (Testability)**: 必須使用相依性注入 (DI) 與介面 (Interface)，確保每個邏輯單元都能被隔離測試。
5. **稽核閘門 (Audit Gate)**: 在調用任何 `replace` 或 `write_file` 前，**必須** 呈現強制性的稽核區塊（使用者、日期、票號、意圖、影響範圍）並獲得使用者核准。

---

## 🔄 第四部分：強制性執行流程 (Mandatory Workflow)
1. **研究與 DRY 審計 (Research & DRY Audit)**: AI 必須先搜尋全域程式碼。若發現即將寫入的邏輯相似度 > 80%，**必須** 在計畫中包含重構步驟。
2. **計畫階段 (Plan Phase)**: 必須明確列出 **"Quality Metric Audit"**，預估方法的行數、嵌套層級與職責。
3. **實作階段 (Act Phase)**: 實作中若發現會超過 40 行或巢狀過深，必須立即執行 **方法提取 (Extract Method)**。
4. **驗證與審查 (Validation & Review)**: 強制呼叫 `code-reviewer` 進行量化指標二次審查。任務完成的定義是：**「成功的測試結果 + 通過品質量化審核」**。
