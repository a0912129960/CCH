# CCH Project: Framework & Technical Specification (專案框架與技術規範)

---

## 1. Project Overview (專案概述)

The Customs Compliance Hub (CCH) is a comprehensive platform designed to automate and manage customs classification, tariff monitoring, and compliance reconciliation for Dimerco. It leverages AI-driven classification and real-time monitoring of US and China tariff schedules.

Customs Compliance Hub (CCH) 是一個全方位的平台，旨在為中菲行 (Dimerco) 自動化管理海關歸類、關稅監控及合規核查。它利用 AI 驅動的歸類技術，以及對美中關稅表的即時監控。

---

## 2. Technical Stack (技術架構)

The project utilizes the latest stable enterprise technologies to ensure performance, security, and maintainability.
本專案採用最新穩定的企業級技術，以確保效能、安全與可維護性。

| Layer (分層) | Technology (技術) | Description (說明) |
| :--- | :--- | :--- |
| **Backend (後端)** | .NET 10 (C# 14) | ASP.NET Core Web API (LTS version) |
| **Frontend (前端)** | Vue 3 + TypeScript | Single Page Application (SPA) with Vite |
| **Database (資料庫)** | MS SQL Server | Relational data storage with EF Core 10 |
| **Authentication (認證)** | OIDC / Azure AD B2C | OpenID Connect for internal, B2C for customers |
| **Hosting (託管)** | Windows Server / IIS | Hosted on BJS infrastructure |
| **State Management** | Pinia | Vue 3 reactive state management |
| **API Client** | Axios | With request interceptors for token refresh |

---

## 3. Architecture & Design (架構設計)

### 3.1 N-Tier Pattern (N 層架構模式)
- **Controller Layer**: Handles HTTP requests and routing. (處理 HTTP 請求與路由)
- **Service Layer**: Contains all business logic (Classification, Audit, etc.). (包含所有業務邏輯：歸類、稽核等)
- **Repository Layer**: Data access via Entity Framework Core. (透過 EF Core 進行資料存取)
- **Models Layer**: DTOs and Database Entities. (資料傳輸物件與資料庫實體)

### 3.2 Key Design Principles (核心設計原則)
- **Soft Deletes**: All entities implement `is_deleted` and `deleted_at`. (所有實體實作軟刪除)
- **Global Filters**: EF Core automatically filters out deleted records. (EF Core 自動過濾已刪除紀錄)
- **Audit Logging**: Every sensitive action is recorded in `AuditEvents`. (所有敏感操作皆記錄於稽核日誌)
- **RBAC**: Role-Based Access Control enforced at the Middleware level. (於中介層強制執行基於角色的存取控制)

---

## 4. Security & Compliance (安全與合規)

- **Credential Management**: Secrets stored in environment variables, never in code. (金鑰儲存於環境變數，絕不寫入程式碼)
- **PII Protection**: Masking personally identifiable information in logs. (在日誌中遮罩個人識別資訊)
- **SLA Enforcement**: Automated escalation for delayed reviews or customer responses. (針對延遲的審核或客戶回應自動進行升級處理)

---

