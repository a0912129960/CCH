# CCH (Customs Compliance Hub) 技術實作分析文件
**來源文件：** CCH BRS v1.6 | Dimerco Express Group  
**Tech Stack：** .NET 8 C# / Vue 3 + TypeScript / MS SQL Server / IIS  
**分析目的：** 分段 AI 輔助程式碼建置指南

---

## 目錄
1. [前端頁面清單與操作流程](#1-前端頁面清單)
2. [後端 API 清單與商業邏輯](#2-後端-api-清單)
3. [Database Table 與 Schema](#3-database-schema)
4. [背景排程工作 (Background Jobs)](#4-背景排程工作)
5. [外部整合清單](#5-外部整合)
6. [AI 輔助程式碼建置分段計畫](#6-ai-輔助建置分段計畫)

---

## 1. 前端頁面清單

系統共有兩大使用者族群：**內部員工**（OIDC SSO 登入）與**外部客戶**（Azure AD B2C 登入），部分頁面共用，部分頁面獨立。

### 1.1 共用 / 認證頁面

| # | 頁面名稱 | 路由 | 說明 |
|---|---------|------|------|
| P01 | 登入頁（內部員工） | `/login` | 導向 OIDC SSO 提供者（同 DimFlow） |
| P02 | 登入頁（外部客戶） | `/customer/login` | Azure AD B2C 登入，含自助註冊入口 |
| P03 | 等待審核頁 | `/pending-approval` | B2C 新帳號等待 Superadmin 啟用時顯示 |
| P04 | 無權限頁 | `/forbidden` | RBAC 攔截後的友善提示 |
| P05 | 系統維護頁 | `/maintenance` | 計畫性維護視窗使用 |

### 1.2 客戶Portal頁面（Customer 角色）

| # | 頁面名稱 | 路由 | 對應 BR | 操作流程 |
|---|---------|------|---------|---------|
| P06 | 客戶首頁 / Dashboard | `/portal/dashboard` | BR-28, BR-29 | 顯示自己帳號下所有 Part No 的狀態摘要（依 7 種狀態分類計數）；顯示待回應項目的 SLA 倒數 |
| P07 | Part No 清單頁 | `/portal/parts` | BR-15, BR-28, BR-29 | 可搜尋、篩選（狀態/Supplier）、排序的 Part No 列表；顯示 HTS Code、狀態、最後更新時間 |
| P08 | Part No 詳細頁 | `/portal/parts/:id` | BR-14, BR-29, BR-30 | 顯示完整分類歷史時間軸；若狀態為 RETURNED，顯示 Dimerco 的理由與替代碼；提供「接受」或「重新提交」按鈕 |
| P09 | 新增 Part No 表單 | `/portal/parts/new` | BR-08, BR-21, BR-29 | 輸入 Part No、產品描述、建議 USHTS Code；含格式驗證（NNNN.NN.NNNN）；顯示描述品質評分（Weak/Adequate/Strong）- Track B |
| P10 | 批量上傳頁 | `/portal/upload` | BR-16, BR-17, BR-18, BR-19 | 拖放 Excel 上傳；顯示上傳進度；完成後顯示逐行匯入報告（新增/更新/不變/拒絕） |
| P11 | 重新提交頁 | `/portal/parts/:id/resubmit` | BR-11, BR-30 | 針對 RETURNED 的 Part No，可修改 HTS Code 並附上支持理由重新提交 |
| P12 | 通知中心 | `/portal/notifications` | BR-27 | 查看所有系統通知（審查完成、代碼被退回、SLA 提醒等） |

**客戶 Portal 操作流程：**
```
登入 → Dashboard 
  ↓
查看待回應項目（PENDING_CUSTOMER / RETURNED）
  ↓
點入 Part No 詳細頁 → 查看 Dimerco 審查意見
  ↓
選擇：[接受替代碼] 或 [重新提交 + 說明]
  ↓
或：點「新增 Part No」→ 填寫表單 → 查看描述評分 → 提交
  ↓
或：批量上傳 Excel → 查看匯入報告
```

### 1.3 內部員工頁面（DCB Filing Staff 角色）

| # | 頁面名稱 | 路由 | 對應 BR | 操作流程 |
|---|---------|------|---------|---------|
| P13 | 查詢主頁 | `/internal/search` | BR-15, BR-23 | 依 Supplier / Part No / USHTS Code / HS Code 搜尋；支援多值逗號分隔批量查詢；結果可匯出 Excel |
| P14 | Part No 詳細頁（唯讀）| `/internal/parts/:id` | BR-14, BR-15 | 顯示 ACTIVE 的 HTS Code 及完整分類歷史；供 DCB 人員在 Descartes 作業前確認使用 |
| P15 | 標記未分類 Part No | `/internal/parts/:id/flag` | BR-26, Flow C | DCB 人員在填報時發現 UNKNOWN 的 Part No，可在此頁面將其標記為 PENDING_CUSTOMER 並觸發通知給客戶 |
| P16 | 批量上傳頁（代客戶上傳）| `/internal/upload` | BR-17 | 與 P10 相同功能，但由 Dimerco 員工代替客戶操作，需先選擇客戶帳號 |

### 1.4 Trade Compliance Reviewer 頁面

| # | 頁面名稱 | 路由 | 對應 BR | 操作流程 |
|---|---------|------|---------|---------|
| P17 | 審查佇列 | `/reviewer/queue` | BR-09, BR-12, BR-36 | 待審查清單（依提交日期+SLA倒數排序）；標示 Express/Standard/Enhanced 三種路由（Track B）；顯示已指派/未指派狀態 |
| P18 | 審查詳細頁 | `/reviewer/queue/:id` | BR-09, BR-10, BR-14 | 左欄：客戶提交資料 + 產品描述；中欄：AI 建議 HTS Code + 信心分數 + 引用理由（Track B）；右欄：CBP HTS 章節說明；底部：「接受」/ 「退回 + 說明 + 替代碼」表單 |
| P19 | Part No 分類歷史查詢 | `/reviewer/parts/:id/history` | BR-14 | 顯示該 Part No 所有提交、審查、退回、重新提交的完整時間軸 |
| P20 | 調解差異 (QC Alert) 頁面 | `/reviewer/qc-alerts` | BR-25, BR-42 | 顯示 Value+ 回傳資料與 CCH 參考資料不符的 Part No 清單；點入後可進行調查並記錄根因 |

**Reviewer 操作流程：**
```
審查佇列（依 SLA 倒數排序）
  ↓
選取 PENDING_REVIEW 的 Part No → 審查詳細頁
  ↓
查看：[客戶建議碼] vs [AI 建議碼 + 信心分數] vs [CBP 官方描述]
  ↓
選擇：
  [接受] → 紀錄審查者身分 + 時間戳 → 狀態 → ACTIVE
  [退回] → 填寫強制必填的說明 + 可選填的替代碼 → 狀態 → RETURNED
```

### 1.5 Data Admin / Superadmin 頁面

| # | 頁面名稱 | 路由 | 對應 BR | 說明 |
|---|---------|------|---------|------|
| P21 | 管理員 Dashboard | `/admin/dashboard` | BR-41 | 所有 7 種狀態的 Part No 計數；SLA 逾期數量；未解決調解差異數；Lookup Gaps（無 Active 碼的 Part No）|
| P22 | 帳號管理頁 | `/admin/users` | BR-33, BR-34 | 列出所有使用者；Superadmin 可啟用新 B2C 客戶帳號；設定角色（5 種）；新客戶帳號等待審核列表 |
| P23 | 帳號詳細 / 角色設定 | `/admin/users/:id` | BR-34, BR-35 | 設定使用者角色；設定 Customer 帳號的 Supplier Scope |
| P24 | 調解差異報告 | `/admin/reports/reconciliation` | BR-42 | 依日期範圍查詢 CCH 參考碼 vs. Value+ 實際申報碼的不符清單；可匯出 Excel |
| P25 | SLA 趨勢報告 | `/admin/reports/sla-trend` | BR-43 | 確認請求數量趨勢圖（SVG 純製）、SLA 合規率 |
| P26 | 稽核日誌匯出 | `/admin/audit-log` | BR-39 | 依日期範圍篩選所有稽核事件；匯出 Excel / CSV；匯出動作本身亦記錄 |
| P27 | 季度審查警示頁 | `/admin/quarterly-review` | BR-38 | 顯示超過 90 天未重新確認的 ACTIVE 碼清單 |

---

## 2. 後端 API 清單

**基礎規則（來自 BIT Coding Standards）：**
- 所有業務邏輯在 Service 層；Controller 僅負責 HTTP
- 所有分頁 API 必須回傳 `{ data: [], total: N }`
- 所有 ID 使用 MS SQL SEQUENCE 物件
- 所有端點均需 JWT 驗證 + RBAC server-side 驗證

### 2.1 認證 / 身分驗證 API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| A01 | POST | `/api/auth/oidc-callback` | 全部 | 接收 OIDC callback，驗證 token，建立/更新內部 user session；自動對應 CCH 角色 |
| A02 | POST | `/api/auth/b2c-callback` | Customer | 接收 Azure AD B2C callback；若帳號未啟用（pending）則回傳 403 並標記狀態；記錄登入事件至稽核日誌 |
| A03 | POST | `/api/auth/logout` | 全部 | 撤銷 server-side session；記錄登出事件 |
| A04 | GET | `/api/auth/me` | 全部 | 回傳當前使用者資訊、角色、Supplier Scope（Customer 帳號） |
| A05 | POST | `/api/auth/refresh` | 全部 | Token 刷新；配合前端 Axios interceptor request queue |

### 2.2 Part No / HTS Code 核心 CRUD API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| B01 | GET | `/api/parts` | All internal + Customer | 分頁搜尋（Supplier/PartNo/USHTS/HS Code，case-insensitive）；Customer 帳號自動套用 Supplier Scope 過濾；支援多值逗號分隔查詢 |
| B02 | GET | `/api/parts/:id` | All | 取單筆 Part No 完整資料（含當前 HTS Code、狀態、最新分類歷史） |
| B03 | GET | `/api/parts/:id/history` | All | 取完整分類歷史時間軸（每一次提交、審查、退回、狀態變更） |
| B04 | POST | `/api/parts` | Customer, DataAdmin | 建立新 Part No；驗證 USHTS 格式（NNNN.NN.NNNN）；初始狀態為 PENDING_REVIEW；觸發 Reviewer 通知；呼叫 AI API（Track B） |
| B05 | PATCH | `/api/parts/:id/status` | System（內部使用） | 狀態機轉換接口，由各商業邏輯觸發；記錄到 ClassificationHistory |
| B06 | DELETE | `/api/parts/:id` | DataAdmin, Superadmin | 軟刪除：設定 status = SUPERSEDED，保留完整歷史；不可真實刪除 |
| B07 | GET | `/api/parts/export` | All internal | 搜尋結果匯出 Excel；需紀錄匯出事件至稽核日誌 |

### 2.3 分類工作流 API (Classification Workflow)

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| C01 | POST | `/api/parts/:id/submit` | Customer | 客戶提交或重新提交 HTS Code；驗證格式；狀態 → PENDING_REVIEW；呼叫 AI 分類 API 並儲存建議結果；發送通知給 Reviewer 佇列 |
| C02 | POST | `/api/parts/:id/review/accept` | Reviewer | Reviewer 接受：**強制記錄**審查者 UserId、Role、時間戳；狀態 → ACTIVE；發送通知給客戶 |
| C03 | POST | `/api/parts/:id/review/return` | Reviewer | Reviewer 退回：**reasoning 欄位強制必填**（不可為空）；可附替代碼；狀態 → RETURNED；發送通知給客戶（含理由與替代碼） |
| C04 | POST | `/api/parts/:id/customer/accept-alternative` | Customer | 客戶接受 Dimerco 的替代碼：以替代碼更新記錄；狀態 → PENDING_REVIEW（重新觸發 Reviewer 確認）|
| C05 | POST | `/api/parts/:id/flag` | DCB, Reviewer, DataAdmin | 手動將 UNKNOWN/ACTIVE 的 Part No 標記為 PENDING_CUSTOMER；觸發客戶通知 |
| C06 | GET | `/api/reviewer/queue` | Reviewer | 取得 Reviewer 佇列（PENDING_REVIEW 清單）；包含 SLA 倒數（業務日計算）；標示 Express/Standard/Enhanced（Track B）|

### 2.4 批量上傳 API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| D01 | POST | `/api/upload/validate` | Customer, DataAdmin | 接收 Excel 檔；驗證檔案格式/大小/類型；解析欄位；回傳預覽資料與欄位對應（Track C 彈性欄位對應）|
| D02 | POST | `/api/upload/process` | Customer, DataAdmin | 執行 Upsert 邏輯：新 Part No → 插入 PENDING_REVIEW；現有 Part No 且 HTS 碼有變 → 更新 + 退回 PENDING_REVIEW；現有且無變化 → 維持現有狀態；重複偵測（精確 + 模糊）；回傳逐行匯入報告 |
| D03 | GET | `/api/upload/:batchId/report` | Customer, DataAdmin | 取得特定批次的匯入報告 |

### 2.5 AI 輔助 API（Track B）

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| E01 | POST | `/api/ai/classify` | Internal | 接收產品描述 + Part No；呼叫外部 AI 分類 API（Avalara / Zonos / Descartes CustomsInfo）；儲存建議 HTS Code、信心分數、引用理由、API 版本與時間戳（永久保留）；回傳給前端 |
| E02 | POST | `/api/ai/description-score` | Customer, Internal | 評估產品描述品質；回傳 Weak/Adequate/Strong 及改善建議提示；純本地邏輯，不呼叫外部 API |
| E03 | GET | `/api/ai/suggestion/:partId` | Reviewer | 取得特定 Part No 最新的 AI 建議結果（含信心分數、理由）；僅限 Reviewer 角色可見，不開放給 Customer |

### 2.6 Value+ 整合 API（內部使用）

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| F01 | POST | `/api/internal/valplus-sync` | System（排程觸發）| 接收/拉取 Value+ eCBS 夜間同步資料；偵測新 Part No（建立 UNKNOWN 記錄）；儲存已申報 HTS Code 用於調解比對 |
| F02 | POST | `/api/internal/reconcile` | System（排程觸發）| 執行 CCH 參考碼 vs. Value+ 申報碼比對；符合 → 記錄調解確認；不符 → 建立 QC Alert，通知 DCB Ops Manager 和 Reviewer |

### 2.7 通知 API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| G01 | GET | `/api/notifications` | All | 取得當前使用者的通知清單（分頁） |
| G02 | PATCH | `/api/notifications/:id/read` | All | 標記通知已讀 |
| G03 | POST | `/api/internal/notify` | System（內部使用）| 統一通知發送入口；依事件類型選擇收件者與模板；發送 Email + 系統內通知 |

### 2.8 報告 / Dashboard API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| H01 | GET | `/api/reports/dashboard` | DataAdmin, Superadmin | 取得 Dashboard 統計：7 種狀態計數、SLA 逾期數、QC Alert 未解決數、Lookup Gaps 數 |
| H02 | GET | `/api/reports/reconciliation` | DCB, Reviewer, DataAdmin | 依日期範圍取得調解差異報告（CCH 碼 vs. 實際申報碼）；可匯出 |
| H03 | GET | `/api/reports/sla-trend` | DataAdmin, Superadmin | SLA 合規率趨勢資料（按週/月）|
| H04 | GET | `/api/reports/quarterly-overdue` | DataAdmin | 超過 90 天未重新確認的 ACTIVE 碼清單 |
| H05 | POST | `/api/reports/audit-log/export` | DataAdmin, Superadmin | 依日期範圍匯出稽核日誌 Excel/CSV；此匯出本身亦寫入稽核日誌 |

### 2.9 使用者管理 API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| I01 | GET | `/api/users` | Superadmin | 列出所有使用者（含 B2C 待審核帳號）|
| I02 | GET | `/api/users/:id` | Superadmin | 取單一使用者詳細資料 |
| I03 | PATCH | `/api/users/:id/role` | Superadmin | 設定角色（5 種）；立即生效；記錄稽核日誌 |
| I04 | PATCH | `/api/users/:id/approve` | Superadmin | 啟用 B2C 客戶帳號；發送啟用通知 Email |
| I05 | PATCH | `/api/users/:id/supplier-scope` | Superadmin | 設定 Customer 帳號的 Supplier 白名單；server-side 強制過濾 |
| I06 | DELETE | `/api/users/:id` | Superadmin | 停用帳號（軟刪除）|

### 2.10 稽核日誌 API

| # | Method | 路徑 | 角色 | 商業邏輯 |
|---|--------|------|------|---------|
| J01 | GET | `/api/audit-log` | DataAdmin, Superadmin | 搜尋稽核日誌（依使用者/事件類型/日期範圍）|

---

## 3. Database Schema

**資料庫：** MS SQL Server  
**ORM：** Entity Framework Core（Code-First）  
**ID 生成：** SEQUENCE 物件（禁用 MAX+1 模式）

### Table 清單總覽

| # | Table 名稱 | 主要用途 |
|---|-----------|---------|
| T01 | `Users` | 所有使用者（內部 OIDC + 外部 B2C） |
| T02 | `UserSupplierScopes` | Customer 帳號的 Supplier 白名單（Junction Table） |
| T03 | `Suppliers` | 供應商主檔 |
| T04 | `PartNumbers` | Part No 主檔（當前狀態與 HTS Code） |
| T05 | `ClassificationHistory` | 分類決策完整歷史（每次動作一筆） |
| T06 | `AiSuggestions` | AI 分類建議結果（永久保留） |
| T07 | `UploadBatches` | 批量上傳批次記錄 |
| T08 | `UploadBatchRows` | 批量上傳逐行結果 |
| T09 | `ValuePlusSyncRecords` | Value+ 夜間同步的已申報 HTS Code |
| T10 | `QcAlerts` | 調解差異 QC 警示 |
| T11 | `TariffChanges` | 關稅變動紀錄（USITC/Federal Register） |
| T12 | `Notifications` | 系統通知 |
| T13 | `AuditLogs` | 所有操作稽核日誌（5 年保留） |
| T14 | `SlaConfigurations` | SLA 設定（業務日計算規則） |

---

### T01 - `Users`

```sql
Users (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_users),
    ExternalId      NVARCHAR(256) NOT NULL UNIQUE,  -- OIDC subject 或 B2C objectId
    AuthProvider    NVARCHAR(50) NOT NULL,           -- 'OIDC' | 'B2C'
    Email           NVARCHAR(256) NOT NULL,
    DisplayName     NVARCHAR(200) NOT NULL,
    Role            NVARCHAR(50) NOT NULL,           -- 'Customer'|'DCBFilingStaff'|'TradeComplianceReviewer'|'DataAdmin'|'Superadmin'
    Status          NVARCHAR(50) NOT NULL DEFAULT 'Active',  -- 'PendingApproval'|'Active'|'Disabled'
    CreatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ApprovedAt      DATETIME2 NULL,
    ApprovedByUserId BIGINT NULL REFERENCES Users(Id),
    LastLoginAt     DATETIME2 NULL,
    IsDeleted       BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT PK_Users PRIMARY KEY (Id)
)
```

---

### T02 - `UserSupplierScopes`

```sql
UserSupplierScopes (
    Id          BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_user_supplier_scopes),
    UserId      BIGINT NOT NULL REFERENCES Users(Id),
    SupplierName NVARCHAR(200) NOT NULL,   -- 對應 PartNumbers.SupplierName
    CreatedAt   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_UserSupplierScopes PRIMARY KEY (Id),
    CONSTRAINT UQ_UserSupplierScopes UNIQUE (UserId, SupplierName)
)
```

---

### T03 - `Suppliers`

```sql
Suppliers (
    Id           BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_suppliers),
    Name         NVARCHAR(200) NOT NULL UNIQUE,
    CreatedAt    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive     BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Suppliers PRIMARY KEY (Id)
)
```

---

### T04 - `PartNumbers`（核心主檔）

```sql
PartNumbers (
    Id                  BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_part_numbers),
    SupplierName        NVARCHAR(200) NOT NULL,
    PartNo              NVARCHAR(200) NOT NULL,
    ProductDescription  NVARCHAR(2000) NULL,
    CurrentUshtsCode    NVARCHAR(15) NULL,           -- 格式: NNNN.NN.NNNN（驗證後才存）
    CurrentChinaHsCode  NVARCHAR(15) NULL,           -- 中國 HS Code（AI 自動對應或人工填入）
    Status              NVARCHAR(50) NOT NULL DEFAULT 'UNKNOWN',
    -- 'UNKNOWN'|'PENDING_CUSTOMER'|'PENDING_REVIEW'|'RETURNED'|'ACTIVE'|'FLAGGED'|'SUPERSEDED'
    
    -- 當前版本追蹤
    CurrentReviewerUserId    BIGINT NULL REFERENCES Users(Id),  -- 最後審查者
    CurrentReviewerRole      NVARCHAR(50) NULL,
    LastReviewedAt           DATETIME2 NULL,
    LastReviewReasoning      NVARCHAR(4000) NULL,
    LastCbpRulingReference   NVARCHAR(100) NULL,
    
    -- 提交者追蹤
    SubmittedByUserId        BIGINT NULL REFERENCES Users(Id),
    SubmittedAt              DATETIME2 NULL,
    UploadBatchId            BIGINT NULL REFERENCES UploadBatches(Id),
    
    -- SLA 追蹤
    ReviewDeadline           DATETIME2 NULL,          -- 審查 SLA 截止（提交後 +2 業務日）
    CustomerResponseDeadline DATETIME2 NULL,          -- 客戶回應 SLA 截止（退回後 +3 業務日）
    LastQuarterlyReviewAt    DATETIME2 NULL,
    NextReviewDueAt          DATETIME2 NULL,          -- 季度審查到期日
    
    -- 元資料
    CreatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsDeleted       BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT PK_PartNumbers PRIMARY KEY (Id),
    CONSTRAINT UQ_PartNumbers UNIQUE (SupplierName, PartNo),  -- 同 Supplier 的 PartNo 唯一
    INDEX IX_PartNumbers_Status (Status),
    INDEX IX_PartNumbers_SupplierName (SupplierName),
    INDEX IX_PartNumbers_CurrentUshtsCode (CurrentUshtsCode)
)
```

---

### T05 - `ClassificationHistory`（稽核核心）

每一次狀態變更、提交、審查、退回均寫一筆，永不刪除。

```sql
ClassificationHistory (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_classification_history),
    PartNumberId    BIGINT NOT NULL REFERENCES PartNumbers(Id),
    
    -- 動作記錄
    ActionType      NVARCHAR(50) NOT NULL,
    -- 'SUBMITTED'|'REVIEWED_ACCEPTED'|'REVIEWED_RETURNED'|'CUSTOMER_ACCEPTED_ALTERNATIVE'
    -- |'CUSTOMER_RESUBMITTED'|'FLAGGED'|'UNFLAGGED'|'SUPERSEDED'|'ADMIN_OVERRIDE'
    
    -- 動作執行者（強制記錄）
    ActorUserId     BIGINT NOT NULL REFERENCES Users(Id),
    ActorRole       NVARCHAR(50) NOT NULL,   -- 記錄當時角色（Role 可能之後變動）
    ActorName       NVARCHAR(200) NOT NULL,  -- 快照姓名
    
    -- HTS 碼快照
    PreviousUshtsCode   NVARCHAR(15) NULL,
    NewUshtsCode        NVARCHAR(15) NULL,
    
    -- 審查/退回說明
    Reasoning           NVARCHAR(4000) NULL,
    CbpRulingReference  NVARCHAR(100) NULL,
    AlternativeCode     NVARCHAR(15) NULL,  -- Reviewer 提出的替代碼
    
    -- 狀態快照
    PreviousStatus  NVARCHAR(50) NULL,
    NewStatus       NVARCHAR(50) NOT NULL,
    
    -- AI 關聯
    AiSuggestionId  BIGINT NULL REFERENCES AiSuggestions(Id),
    
    CreatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_ClassificationHistory PRIMARY KEY (Id),
    INDEX IX_ClassificationHistory_PartNumberId (PartNumberId),
    INDEX IX_ClassificationHistory_ActorUserId (ActorUserId)
)
```

---

### T06 - `AiSuggestions`（Track B）

```sql
AiSuggestions (
    Id                  BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_ai_suggestions),
    PartNumberId        BIGINT NOT NULL REFERENCES PartNumbers(Id),
    
    -- AI 建議結果
    SuggestedUshtsCode  NVARCHAR(15) NULL,
    SuggestedChinaHs    NVARCHAR(15) NULL,
    ConfidenceScore     DECIMAL(5,2) NULL,   -- 0.00 ~ 100.00
    Reasoning           NVARCHAR(4000) NULL, -- AI 引用的理由（GRI Rule / CBP Ruling 等）
    
    -- 路由分類（Track B）
    ReviewRoute         NVARCHAR(20) NULL,   -- 'Express'|'Standard'|'Enhanced'
    
    -- 外部 API 追蹤（法規要求永久保留）
    ApiVendor           NVARCHAR(100) NOT NULL,   -- 'Avalara'|'ZonosClassify'|'DescartesCustomsInfo'
    ApiVersion          NVARCHAR(50) NOT NULL,
    ApiRequestId        NVARCHAR(200) NULL,
    
    ProductDescriptionUsed NVARCHAR(2000) NULL,  -- 快照當時送出的描述
    DescriptionQualityScore NVARCHAR(20) NULL,   -- 'Weak'|'Adequate'|'Strong'
    
    CreatedAt           DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_AiSuggestions PRIMARY KEY (Id),
    INDEX IX_AiSuggestions_PartNumberId (PartNumberId)
)
```

---

### T07 - `UploadBatches`

```sql
UploadBatches (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_upload_batches),
    UploadedByUserId BIGINT NOT NULL REFERENCES Users(Id),
    OnBehalfOfSupplierName NVARCHAR(200) NULL,  -- 代客戶上傳時填入
    FileName        NVARCHAR(500) NOT NULL,
    Status          NVARCHAR(50) NOT NULL,  -- 'Processing'|'Completed'|'Failed'
    TotalRows       INT NULL,
    InsertedCount   INT NULL,
    UpdatedCount    INT NULL,
    UnchangedCount  INT NULL,
    RejectedCount   INT NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CompletedAt     DATETIME2 NULL,
    
    CONSTRAINT PK_UploadBatches PRIMARY KEY (Id)
)
```

---

### T08 - `UploadBatchRows`

```sql
UploadBatchRows (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_upload_batch_rows),
    BatchId         BIGINT NOT NULL REFERENCES UploadBatches(Id),
    RowNumber       INT NOT NULL,
    SupplierName    NVARCHAR(200) NULL,
    PartNo          NVARCHAR(200) NULL,
    UshtsCode       NVARCHAR(15) NULL,
    ProductDescription NVARCHAR(2000) NULL,
    
    -- 處理結果
    Result          NVARCHAR(50) NOT NULL,  -- 'Inserted'|'Updated'|'Unchanged'|'Rejected'|'DuplicateFlagged'
    PreviousUshtsCode NVARCHAR(15) NULL,    -- 更新時記錄舊碼
    RejectionReason NVARCHAR(500) NULL,
    
    CONSTRAINT PK_UploadBatchRows PRIMARY KEY (Id),
    INDEX IX_UploadBatchRows_BatchId (BatchId)
)
```

---

### T09 - `ValuePlusSyncRecords`

```sql
ValuePlusSyncRecords (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_valplus_sync),
    SyncRunId       NVARCHAR(100) NOT NULL,  -- 每次夜間同步的批次識別
    SupplierName    NVARCHAR(200) NOT NULL,
    PartNo          NVARCHAR(200) NOT NULL,
    FiledUshtsCode  NVARCHAR(15) NOT NULL,   -- Value+ 回傳的實際申報碼
    EntryReference  NVARCHAR(200) NULL,      -- 報關單號
    FiledDate       DATE NOT NULL,
    SyncedAt        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ReconciliationStatus NVARCHAR(50) NULL,  -- 'Matched'|'Discrepancy'|'NotFound'
    QcAlertId       BIGINT NULL REFERENCES QcAlerts(Id),
    
    CONSTRAINT PK_ValuePlusSyncRecords PRIMARY KEY (Id),
    INDEX IX_ValuePlusSyncRecords_PartNo (SupplierName, PartNo)
)
```

---

### T10 - `QcAlerts`

```sql
QcAlerts (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_qc_alerts),
    PartNumberId    BIGINT NOT NULL REFERENCES PartNumbers(Id),
    AlertType       NVARCHAR(50) NOT NULL,  -- 'ReconciliationDiscrepancy'|'TariffChange'
    
    CchUshtsCode    NVARCHAR(15) NULL,  -- CCH 參考碼
    FiledUshtsCode  NVARCHAR(15) NULL,  -- Value+ 實際申報碼
    
    Status          NVARCHAR(50) NOT NULL DEFAULT 'Open',  -- 'Open'|'Investigating'|'Resolved'
    AssignedReviewerUserId BIGINT NULL REFERENCES Users(Id),
    ResolutionNotes NVARCHAR(2000) NULL,
    ResolvedAt      DATETIME2 NULL,
    ResolvedByUserId BIGINT NULL REFERENCES Users(Id),
    
    CreatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_QcAlerts PRIMARY KEY (Id)
)
```

---

### T11 - `TariffChanges`（Track B）

```sql
TariffChanges (
    Id                  BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_tariff_changes),
    Source              NVARCHAR(100) NOT NULL,  -- 'USITC'|'FederalRegister_Section301'
    AffectedUshtsCode   NVARCHAR(15) NOT NULL,
    ChangeType          NVARCHAR(100) NOT NULL,  -- 'RateChange'|'Section301Addition'|'Section301Exclusion'
    OldRate             NVARCHAR(100) NULL,
    NewRate             NVARCHAR(100) NULL,
    EffectiveDate       DATE NULL,
    FederalRegisterCitation NVARCHAR(500) NULL,
    RawData             NVARCHAR(MAX) NULL,      -- JSON 原始資料
    DetectedAt          DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_TariffChanges PRIMARY KEY (Id),
    INDEX IX_TariffChanges_AffectedCode (AffectedUshtsCode)
)
```

---

### T12 - `Notifications`

```sql
Notifications (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_notifications),
    RecipientUserId BIGINT NOT NULL REFERENCES Users(Id),
    EventType       NVARCHAR(100) NOT NULL,
    -- 'NewSubmissionForReview'|'ReviewCompleted'|'CodeReturned'|'SlaOverdue'
    -- |'ReconciliationDiscrepancy'|'NewPartNoUnknown'|'TariffChangeDetected'
    -- |'DutyOptimisationOpportunity'|'NewCustomerRegistration'
    Title           NVARCHAR(500) NOT NULL,
    Body            NVARCHAR(2000) NOT NULL,
    RelatedPartNumberId BIGINT NULL REFERENCES PartNumbers(Id),
    RelatedQcAlertId    BIGINT NULL REFERENCES QcAlerts(Id),
    IsRead          BIT NOT NULL DEFAULT 0,
    EmailSentAt     DATETIME2 NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    
    CONSTRAINT PK_Notifications PRIMARY KEY (Id),
    INDEX IX_Notifications_RecipientUserId_IsRead (RecipientUserId, IsRead)
)
```

---

### T13 - `AuditLogs`（5 年保留，只寫不修改）

```sql
AuditLogs (
    Id              BIGINT NOT NULL DEFAULT (NEXT VALUE FOR seq_audit_logs),
    Timestamp       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ActorUserId     BIGINT NULL,           -- NULL 表示 System 觸發
    ActorEmail      NVARCHAR(256) NULL,    -- 快照（用戶可能被刪除）
    ActorRole       NVARCHAR(50) NULL,
    EventType       NVARCHAR(100) NOT NULL,
    -- 'Login'|'Logout'|'PartCreate'|'PartUpdate'|'PartDelete'|'BulkUpload'
    -- |'ReviewAccept'|'ReviewReturn'|'CustomerSubmit'|'Export'|'UserRoleChange'
    -- |'UserApproved'|'AuditLogExport' ...
    EntityType      NVARCHAR(100) NULL,    -- 'PartNumber'|'User'|'UploadBatch' 等
    EntityId        BIGINT NULL,
    Description     NVARCHAR(2000) NULL,
    IpAddress       NVARCHAR(50) NULL,
    UserAgent       NVARCHAR(500) NULL,
    
    CONSTRAINT PK_AuditLogs PRIMARY KEY (Id),
    INDEX IX_AuditLogs_Timestamp (Timestamp),
    INDEX IX_AuditLogs_ActorUserId (ActorUserId),
    INDEX IX_AuditLogs_EventType (EventType)
)
-- 注意：此 Table 不得有 UPDATE/DELETE 權限，僅允許 INSERT + SELECT
```

---

### T14 - `SlaConfigurations`

```sql
SlaConfigurations (
    Id              INT NOT NULL IDENTITY(1,1),
    ConfigKey       NVARCHAR(100) NOT NULL UNIQUE,
    -- 'ReviewerSlaBusinessDays'|'CustomerResponseSlaBusinessDays'
    -- |'QuarterlyReviewThresholdDays'
    Value           INT NOT NULL,
    Description     NVARCHAR(500) NULL,
    UpdatedAt       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedByUserId BIGINT NULL REFERENCES Users(Id),
    
    CONSTRAINT PK_SlaConfigurations PRIMARY KEY (Id)
)
-- 預設資料：ReviewerSla=2, CustomerResponseSla=3, QuarterlyThreshold=90
```

---

## 4. 背景排程工作

| # | Job 名稱 | 觸發時間 | 說明 |
|---|---------|---------|------|
| JB01 | SLA 倒數引擎 | 每日 08:00 HKT | 計算所有 PENDING_REVIEW 和 RETURNED/PENDING_CUSTOMER 的 SLA；發送 T-1 日提醒、到期當日通知、逾期升級通知給 DCB Ops Manager |
| JB02 | 季度審查掃描 | 每季第 1 日 09:00 HKT | 查詢所有 ACTIVE 且超過 90 天未觸碰的 Part No；發送摘要通知給 Data Steward 和相關客戶 |
| JB03 | Value+ 夜間同步 | 每日 01:00 HKT | 拉取 Value+ eCBS 當日完成的報關資料；更新 ValuePlusSyncRecords；觸發調解比對（JB04）|
| JB04 | 調解比對引擎 | 接續 JB03 | 對比 CCH Active 碼 vs. Value+ 申報碼；建立 QC Alerts；發送通知給 Reviewer 和 DCB Ops Manager |
| JB05 | USITC HTS 變動監控（Track B）| 每日 02:00 HKT | 下載 USITC 最新 HTS Revision Diff；比對 CCH ACTIVE 碼；有變動的設為 FLAGGED；記錄 TariffChanges |
| JB06 | Federal Register 解析（Track B）| 每日 02:30 HKT | 呼叫 api.federalregister.gov；解析 USTR Section 301 通知；交叉比對 CCH ACTIVE 碼；觸發 Duty Optimisation Alert |
| JB07 | MS SQL 自動備份 | 每日 00:00 HKT | 資料庫完整備份；保留週期確保最大 24 小時 RPO；備份完成紀錄至 AuditLogs |

---

## 5. 外部整合

| # | 整合對象 | 方向 | 用途 | 認證方式 |
|---|---------|------|------|---------|
| EXT01 | OIDC SSO（企業 IdP）| CCH ← IdP | 內部員工登入 | OIDC / OpenID Connect |
| EXT02 | Azure AD B2C | CCH ← B2C | 外部客戶登入/註冊 | Azure AD B2C OAuth 2.0 |
| EXT03 | AI 分類 API（Avalara / Zonos / Descartes） | CCH → API | AI HTS Code 建議 | API Key（appsettings） |
| EXT04 | Value+ eCBS | CCH ← Value+ | 夜間同步 Part No + 申報碼 | ❓ 待確認（API or DB View）|
| EXT05 | USITC HTS Schedule | CCH ← USITC | 關稅變動監控 | 公開資源，無需認證 |
| EXT06 | Federal Register API | CCH ← api.federalregister.gov | Section 301 通知解析 | 公開 API，無需認證 |
| EXT07 | SMTP / Email 服務 | CCH → Email | 各類通知 Email | SMTP 憑證（appsettings）|

---

## 6. AI 輔助建置分段計畫

以下按照 **Track A → Track B → Track C** 的順序，將系統拆分為可獨立交付給 AI 的建置單元。每個 Sprint 均有清楚的輸入規格與輸出驗收標準。

---

### Sprint 0：基礎建設（1 週）

**目標：** 建立可運行的專案骨架  
**AI 建置指令要點：**
- 建立 .NET 8 ASP.NET Core Web API 專案（Solution + 4 Projects：API / Domain / Infrastructure / Tests）
- 建立 Vue 3 + TypeScript + Vite 專案
- 設定 EF Core Code-First，連接 MS SQL Server
- 建立所有 SEQUENCE 物件
- 建立 T13 `AuditLogs` + T14 `SlaConfigurations` + T01 `Users` + T02 `UserSupplierScopes` + T03 `Suppliers`
- 建立 Axios interceptor（含 token refresh queue）
- 設定 IIS URL Rewrite（Vue SPA fallback）

---

### Sprint 1：身分驗證與 RBAC（Track A，1-2 週）

**目標：** BR-01, BR-02, BR-03, BR-04, BR-05  
**AI 建置指令要點：**
- 實作 OIDC SSO callback（A01）
- 實作 Azure AD B2C callback + 新帳號 Pending 流程（A02）
- 實作 JWT 中間件 + server-side RBAC（每個 Controller/Action 標記所需角色）
- 實作 `AuditLogMiddleware`（所有寫入操作自動記錄）
- 建置 P01 / P02 / P03 / P04 登入頁面
- **驗收：** 5 種角色可各自登入，跨角色操作被攔截並回傳 403

---

### Sprint 2：Part No CRUD + 搜尋（Track A，1 週）

**目標：** BR-15, BR-21, BR-22, BR-20  
**AI 建置指令要點：**
- 建立 T04 `PartNumbers` + T05 `ClassificationHistory`
- 實作 B01-B07 API（含 Supplier Scope 過濾）
- 實作 USHTS Code 格式驗證（NNNN.NN.NNNN，10 位數）
- 實作軟刪除（SUPERSEDED）
- 建置 P13 搜尋頁 + P14 詳細頁（唯讀）
- **驗收：** 多值逗號分隔批量搜尋正確返回；Customer 帳號無法看到非自己 Supplier 的資料

---

### Sprint 3：批量上傳（Track A，1 週）

**目標：** BR-16, BR-17, BR-18, BR-19, BR-06  
**AI 建置指令要點：**
- 建立 T07 `UploadBatches` + T08 `UploadBatchRows`
- 實作 D01-D03 API（檔案驗證 → Upsert 邏輯 → 逐行報告）
- Upsert 邏輯：新 Part No → PENDING_REVIEW；現有+HTS 有變 → PENDING_REVIEW；無變化 → 維持
- 精確重複偵測（同 Supplier + Part No）
- 建置 P10 批量上傳頁 + P16 代客戶上傳頁
- **驗收：** 1000 行 Excel 上傳時間 < 2 分鐘；逐行報告正確區分 4 種結果

---

### Sprint 4：7 狀態分類工作流（Track C，2 週）

**目標：** BR-07~BR-14, BR-36  
**AI 建置指令要點：**
- 實作狀態機（State Machine）：定義所有合法的狀態轉換
- 實作 C01~C05 API
- C02/C03：強制記錄審查者身分（UserId + Role + Name 快照）；C03 reasoning 欄位不可為空
- 實作 SLA 計算服務（業務日計算，參考 T14 設定）
- 建置 P17 Reviewer 佇列 + P18 審查詳細頁
- 建置 P07 客戶 Part No 清單 + P08 詳細頁 + P09 新增表單 + P11 重新提交頁
- **驗收：** 完整走過 Flow A（客戶上傳 → 審查 → ACTIVE）；RETURNED 後客戶可接受或重新提交

---

### Sprint 5：通知系統 + SLA 排程（Track C，1 週）

**目標：** BR-12, BR-13, BR-27  
**AI 建置指令要點：**
- 建立 T12 `Notifications`
- 實作 G01-G03 API
- 實作 Email 發送服務（模板化，7 種事件類型）
- 實作 JB01 SLA 倒數排程（Hangfire 或 .NET BackgroundService）
- 建置 P12 通知中心
- **驗收：** 提交後 Reviewer 收到通知；審查後客戶收到通知；SLA 逾期時 DCB Ops Manager 收到升級通知

---

### Sprint 6：使用者管理（Track C，1 週）

**目標：** BR-33, BR-34, BR-35  
**AI 建置指令要點：**
- 實作 I01-I06 API
- B2C 新帳號 → Pending → Superadmin 通知 → 啟用流程
- Supplier Scope 設定 + server-side 過濾
- 建置 P22 帳號管理頁 + P23 帳號詳細頁
- **驗收：** B2C 新帳號在啟用前無法存取任何資料；Supplier Scope 設定後立即生效

---

### Sprint 7：AI HTS 建議（Track B，2 週）

**目標：** BR-48, 10.1, 10.2, 10.3  
**AI 建置指令要點：**
- 建立 T06 `AiSuggestions`
- 實作 E01-E03 API（整合外部 AI 分類 API）
- 實作信心分數路由邏輯（≥90% Express；70-89% Standard；<70% Enhanced）
- 實作描述品質評分（E02，本地規則引擎）
- 更新 P18 審查詳細頁（三欄佈局：客戶碼 vs AI 建議 vs CBP 官方）
- 更新 P09 新增表單（顯示描述品質評分）
- **注意：** AI 信心分數僅 Reviewer 可見，不可向客戶顯示
- **驗收：** 輸入產品描述後 3 秒內回傳 AI 建議；AI 建議正確記錄 API 版本與時間戳

---

### Sprint 8：關稅變動監控（Track B，2 週）

**目標：** BR-48, 10.6  
**AI 建置指令要點：**
- 建立 T11 `TariffChanges`
- 實作 JB05 USITC HTS 監控排程
- 實作 JB06 Federal Register Section 301 解析排程
- 實作 Duty Optimisation Alert 生成邏輯（含估算節省金額）
- 建置 P20 QC Alert 頁面（用於展示）
- **驗收：** 手動觸發排程後，模擬資料中有 HTS Code 變動的 Part No 自動被標記為 FLAGGED；Duty Alert Email 生成正確格式

---

### Sprint 9：Value+ 整合 + 調解（Track C，2 週）

**目標：** BR-24, BR-25, BR-26  
**AI 建置指令要點：**
- 建立 T09 `ValuePlusSyncRecords` + T10 `QcAlerts`
- 實作 F01-F02 API
- 實作 JB03 Value+ 夜間同步排程（❓ 依 Value+ API 規格確認後再建置）
- 實作 JB04 調解比對引擎
- **驗收：** 夜間同步後，CCH 碼與 Value+ 申報碼不符的 Part No 自動建立 QC Alert 並通知

---

### Sprint 10：Dashboard + 報告（Track C，1 週）

**目標：** BR-38~BR-43  
**AI 建置指令要點：**
- 實作 H01-H05 API
- 實作 JB02 季度審查掃描排程
- 建置 P21 管理員 Dashboard（含 SVG 純製圖表，不使用圖表 Library）
- 建置 P24 調解差異報告 + P25 SLA 趨勢報告 + P26 稽核日誌匯出 + P27 季度審查警示
- **驗收：** Dashboard 數字與資料庫實際狀態一致；匯出 Excel 正確且匯出事件本身被記錄

---

### Sprint 11：收尾與安全強化（Track A，1 週）

**目標：** BR-04, BR-05, BR-06, BR-44~BR-47  
**AI 建置指令要點：**
- 確認所有 secrets 均透過 `appsettings.Production.json` 或環境變數管理（無硬編碼）
- 確認 TLS 1.2+ 已在 IIS 層設定
- 確認所有上傳端點有完整的檔案類型/大小/內容驗證
- 確認 CORS 白名單設定
- 實作 JB07 自動備份排程
- 設定 Staging 環境（與 Production 完全隔離）
- 建置可用性監控（15 分鐘內通知 DCB Ops Manager）

---

## 補充說明

### 關鍵商業規則速查

| 規則 | 說明 |
|------|------|
| **永不真刪除** | Part No 只能設為 SUPERSEDED，ClassificationHistory 永不刪除 |
| **Reasoning 強制必填** | 審查者退回時必須填寫說明，系統層面不可繞過 |
| **AI 不可自動接受** | 無論信心分數多高，都必須有名字可查的 Reviewer 手動 accept |
| **AI 結果不給客戶看** | 信心分數僅限 Reviewer 內部使用 |
| **Supplier Scope 強制** | Customer 帳號的 Supplier 過濾必須在 server-side 執行，前端不可信任 |
| **稽核日誌只能寫** | AuditLogs Table 不得有 UPDATE/DELETE 權限 |
| **ID 使用 SEQUENCE** | 禁用 MAX+1 模式以避免並發問題 |
| **Junction Table** | 所有多對多關係使用 Junction Table，禁用逗號分隔字串 |

### 5 種使用者角色能力矩陣

| 功能 | Customer | DCB Filing Staff | Reviewer | Data Admin | Superadmin |
|------|:---:|:---:|:---:|:---:|:---:|
| 搜尋 Part No | ✓（自己 Supplier）| ✓（全部）| ✓（全部）| ✓（全部）| ✓（全部）|
| 提交 HTS Code | ✓ | — | — | ✓ | ✓ |
| 批量上傳 | ✓ | ✓（代客戶）| — | ✓ | ✓ |
| 審查 / 接受 / 退回 | — | — | ✓ | — | — |
| 標記 PENDING_CUSTOMER | — | ✓ | ✓ | ✓ | ✓ |
| 管理員 Dashboard | — | — | — | ✓ | ✓ |
| 匯出稽核日誌 | — | — | — | ✓ | ✓ |
| 管理使用者 / 角色 | — | — | — | — | ✓ |

---

*文件版本：基於 CCH BRS v1.6（March 2026）*  
*分析日期：2026-04-08*
