# CCH Project: Architectural Specification (ARCHITECTURE.md)

This document defines the architectural standards and patterns for the CCH project to ensure consistency between human developers and AI assistants.

## 🏗️ 1. Modular Layered Architecture (SOLID Implementation)

### 1.1 Single Responsibility Principle (SRP)
- **Service Splitting**: 
  - Split large services into granular files when implementation exceeds 250 lines or covers distinct domains (e.g., `PartQueryService`, `PartLifecycleService`, `PartExcelService`).
  - **Cross-cutting Support**: Use `CCH.Services/Infrastructure/` for infrastructure-level services (e.g., `UserContext`) that provide environment/session support to multiple feature modules. (繁體中文) 基礎設施級別的服務（如 `UserContext`）應放置於 `Infrastructure/` 資料夾，為多個功能模組提供環境或會話支援。
- **DTO Specificity**: DTOs must be purpose-built (e.g., `PartSaveRequest`, `PartDetailResponse`). Avoid generic "PartDto" for all operations.

### 1.2 Open-Closed Principle (OCP)
- **Extension via Interface**: Add new behaviors (e.g., a new export format) by creating new service implementations rather than modifying existing ones.

### 1.3 Liskov Substitution Principle (LSP)
- Derived classes or interface implementations must remain compatible with the base contract without throwing `NotImplementedException`.

### 1.4 Interface Segregation Principle (ISP)
- Controllers must only inject the specific interface they need.
- `IPartQueryService` vs `IPartLifecycleService`.

### 1.5 Dependency Inversion Principle (DIP)
- High-level modules (Services) must depend on abstractions (Interfaces in `CCH.Core`), not on low-level modules (Repositories).

## 🔐 2. Role-Based Access Control (RBAC)

The system supports three primary roles, each with specific logic boundaries:

| Role | Responsibility | Data Access Boundary |
| :--- | :--- | :--- |
| **Customer** | Draft and submit parts. | Can only see/edit their own parts in status S01/S03. |
| **Dimerco** | Legacy/ReadOnly Support. | Access is maintained for audit; active review migrated to DCB. (繁體中文) 僅供稽核使用；審核功能已移至 DCB。 |
| **DCB** | Primary review and acceptance. | Can view/accept/return parts in S02. |

## 📊 3. Data Flow Pattern
1. **API Controller**: Receives request -> Validates `ModelState` -> Calls Service Interface.
2. **Service Layer**: 
   - Orchestrates logic.
   - **Handles Mapping**: Converts Entities to DTOs.
   - **Calculates Business logic**: SLA status, rates, etc.
3. **Repository Layer**: 
   - **Pure CRUD**: No business logic or DTO mapping.
   - Returns Entities (`CCH.Core.Entities`).
   - Interacts with JSON Mock files.

## 🧪 4. Testing Standard
- **Unit Tests**: Every public method in the Service layer must have a corresponding test in `CCH.Tests`.
- **Mocking**: Use `Moq` for service dependencies.
- **Verification**: Tests must verify both "Happy Path" and "Edge Cases" (e.g., invalid status transitions).

## 📋 5. Documentation Maintenance (SOP)

To keep the project AI-friendly and human-readable, the following documents must be synchronized:

1. **llms.txt (The Map)**: 
   - **Update Trigger**: Whenever a new module is added, a service is split, or a file is moved.
   - **Responsibility**: The developer (human or AI) performing the structural change.
2. **ARCHITECTURE.md (The Standard)**:
   - **Update Trigger**: When a new architectural pattern is introduced or RBAC logic changes.
3. **AI_RULES.md (The Constitution)**:
   - **Update Trigger**: When coding standards, audit procedures, or bilingual requirements are modified.
