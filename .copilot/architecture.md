# zento-api Architecture Overview

## Project Structure
- `src/Zento.Api/` — Main API project
  - `Common/Models/` — Shared models (e.g., PagedResult, Result)
  - `Domain/Entities/` — Domain entities (Category, Product, Supplier)
  - `Endpoints/` — Minimal API endpoints for each entity
  - `Features/` — CQRS handlers, queries, and commands for each entity
  - `Infrastructure/Data/` — DbContext and repository implementations
  - `Properties/` — launchSettings.json and other project properties
- `.github/workflows/` — CI/CD workflows for build and deployment
- `.copilot/` — Copilot and AI agent instructions and documentation

## Patterns & Technologies
- **CQRS with MediatR**: All business logic is handled via MediatR request/response handlers.
- **Minimal APIs**: Endpoints are defined using minimal API style for simplicity and performance.
- **Entity Framework Core**: Used for data access, with SQLite as the default provider (can be swapped for Azure SQL, PostgreSQL, etc.).
- **Repository Pattern**: Data access logic is abstracted via repositories.
- **PagedResult & Result Wrapping**: All list responses are wrapped in PagedResult<T> for consistent pagination metadata.
- **Validation**: FluentValidation is used for request validation.
- **CI/CD**: GitHub Actions workflow builds, publishes, and deploys the app to Azure Web App.

## Key Architectural Decisions
- **Separation of Concerns**: Handlers, repositories, and models are separated for maintainability.
- **Cross-cutting Concerns**: Filters and middleware are used for response shaping and error handling.
- **Environment Configuration**: appsettings.json and appsettings.Development.json are used for environment-specific settings.
- **Extensibility**: The architecture supports easy addition of new entities, features, and endpoints.

## Deployment
- The published output (`./publish`) is deployed to Azure Web App via GitHub Actions.
- Environment variables and secrets are managed in GitHub for Azure credentials.

## How Copilot Should Work
- Reference this file and instructions.md for context on architecture and workflow.
- When generating code, follow the patterns described here (CQRS, minimal APIs, repository, etc.).
- When updating workflows, ensure published output and workflow variables are used.
- When adding new features, follow the structure and naming conventions.

---

For more details, see CONTRIBUTING.md and instructions.md in the .copilot folder.
