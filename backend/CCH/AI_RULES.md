# CCH Project: AI Rules & Backend Engineering Standards (AI 規則與後端工程規範)

## ⚖️ 1. Core Mandates & Authority (核心指令與權威文件)
- **Bilingual Mandate (雙語指令)**: All technical explanations, plan summaries, and code **remarks** MUST be provided in both **Traditional Chinese (繁體中文)** and **English**.
- **Documentation Authority (文件權威)**: 
  - **llms.txt**: The primary navigation map. AI MUST read this before any task and MUST update it immediately after any structural change (adding modules, splitting services, moving files). (繁體中文) AI 在執行任務前**必須**讀取此檔，且在任何結構變更後（新增模組、拆分服務、移動檔案）**必須立即同步更新**此檔。
  - **ARCHITECTURE.md**: The technical standard. AI MUST adhere to the SOLID patterns defined herein. (繁體中文) AI **必須**遵守其中定義的 SOLID 模式。
- **Code Cleanliness (代碼整潔)**: **Trust Git for history.** Do not preserve large blocks of commented-out code permanently. You MAY comment out code during active refactoring for clarity, but it MUST be deleted once the new implementation is verified to keep files under the 250-line limit. (繁體中文) **相信 Git 的版本管理。** 不要永久保留大塊的註解代碼。重構期間可暫時註解以利比對，但驗證成功後**必須刪除**以維持 250 行限額。
- **Audit SOP (稽核標準程序)**: Before any modification, present the mandatory audit block (User, Date, Ticket, Intent, Impact) and obtain explicit user approval.

## 🏗️ 2. Backend Architecture (後端架構 - Modular 4-Layer)
- **Framework**: .NET 10.0 Web API (Controller-based).
- **Quality Guardrails (品質護欄)**:
  - **No Shortcuts**: AI is STRICTLY FORBIDDEN from using "hacks" (e.g., `dynamic`, `object` for data models, empty `catch` blocks) to bypass errors or design responsibilities. (繁體中文) 嚴禁使用模糊型別或空擷取區塊來規避錯誤或設計責任。
  - **File Limit**: Implementation files SHOULD NOT exceed 250 lines. If they do, they MUST be reviewed for SRP splitting. (繁體中文) 檔案不宜超過 250 行，否則必須重新檢視是否符合 SRP 並進行拆分。
  - **Bilingual XML**: Every public member MUST have XML documentation in both English and Traditional Chinese.
- **Layered Structure (分層架構)**:
  1. **CCH.API**: Controllers, JWT Auth Configuration, Middlewares.
  2. **CCH.Core**: 
     - **Features/**: Feature-based modules containing DTOs, Enums, and Service Interfaces. (繁體中文) 以功能為單位的模組，包含 DTO、列舉與服務介面。
     - **Shared/**: Global DTOs and Response wrappers.
  3. **CCH.Services**: 
     - **Features/**: Service implementations grouped by feature.
     - **Repositories/**: Data access implementations (JSON persistence).
     - **Infrastructure/**: Cross-cutting support services (e.g., `UserContext.cs`) used across multiple features. (繁體中文) 跨切面支援服務（如 `UserContext.cs`），供多個功能模組共同使用。
- **SOLID Mandate (SOLID 強制規範)**: 
  - **Single Responsibility (SRP)**: 
    - Split large services into granular files (Query, Lifecycle, Excel).
    - **Mapping Responsibility**: Services handle DTO mapping and business logic (SLA). Repositories MUST only handle Entity retrieval/storage. (繁體中文) Mapping 與業務邏輯 (SLA) 由 Service 處理；Repository 僅限於 Entity 的存取。
  - **Interface Segregation (ISP)**: Clients should only depend on relevant interfaces.
  - **Dependency Inversion (DIP)**: Services depend on abstractions, never on concrete repository implementations directly.
- **Repository Mandate**: 
  - Services MUST NOT access `DbContext` or raw storage directly.
  - **Mock Phase Persistence**: Repositories MUST use persistent JSON files in `Repositories/Data/`.
- **SOLID Principles**: Implementation must strictly adhere to SOLID principles.
- **Auth (認證)**: Use **JWT Token-based authentication**. Secure controllers with `[Authorize]`.
- **RESTful API**: 
  - Use standard HTTP methods (`GET`, `POST`, `PUT`, `DELETE`).
  - Meaningful resource-based URLs (e.g., `/api/parts/{id}`).
  - Standard HTTP status codes.
- **Naming Conventions (命名規範)**:
  - **PascalCase**: Classes, Methods, Interfaces, Properties.
  - **camelCase**: Local variables, parameters.
  - **Interface Prefix**: Must start with `I` (e.g., `IService`).

## 🧪 3. Testing & Validation (測試與驗證)
- **Test-Driven Delivery (TDD)**: No code change is complete without corresponding unit/integration tests.
- **Mandatory Testing**: EVERY function and API endpoint must have associated tests (xUnit/FluentAssertions).
- **Environment Exception**: If the shell environment SDK version is incompatible with the project (e.g., .NET 7 vs .NET 10), AI MUST perform **exhaustive static code analysis** and request user manual verification. "Done" is achieved upon manual confirmation in such cases. (繁體中文) 若環境 SDK 不相容，AI 必須進行**詳盡的靜態分析**並請求使用者手動驗證。

## 🛡️ 4. API Security & Standards (API 安全與標準)
- **Response Format**: Standardize responses using a generic wrapper (e.g., `ApiResponse<T>`).
