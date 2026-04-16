# CCH Project: AI Rules & Backend Engineering Standards (AI 規則與後端工程規範)

## ⚖️ 1. Core Mandates (核心指令)
- **Bilingual Mandate (雙語指令)**: All technical explanations, plan summaries, and code **remarks** MUST be provided in both **Traditional Chinese (繁體中文)** and **English**.
- **Legacy Code Preservation (舊程式碼保存)**: NEVER DELETE old code. You MUST comment it out using `/* ... */`.
- **Read-Only Remarks**: Existing `Update by...` comments are READ-ONLY. AI is forbidden from deleting or overwriting them.
- **Audit SOP (稽核標準程序)**: Before any modification, present the mandatory audit block (User, Date, Ticket, Intent, Impact) and obtain explicit user approval.

## 🏗️ 2. Backend Architecture (後端架構 - 4-Layer)
- **Framework**: .NET 10.0 Web API (Controller-based).
- **Layered Structure (分層架構)**:
  1. **CCH.API**: Controllers, JWT Auth Configuration, Middlewares.
  2. **CCH.Core**: 
     - **Entities**: Database models.
     - **DTOs**: Request/Response models for API.
     - **Interfaces**: Service and **Repository** interfaces.
  3. **CCH.Services**: 
     - **Services**: Business logic, orchestration, and DTO mapping.
     - **Repositories**: Data access implementations (SQL, EF Core, or Mock).
     - **Data**: `AppDbContext` implementation.
- **Repository Mandate (Repository 強制規範)**: 
  - All database CRUD operations MUST be handled by the Repository layer. 
  - Services MUST NOT access `DbContext` or raw data storage directly.
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
- **Validation**: A task is only "Done" after project-specific tests and linting pass.

## 🛡️ 4. API Security & Standards (API 安全與標準)
- **Response Format**: Standardize responses using a generic wrapper (e.g., `ApiResponse<T>`).
