# SmartHire Backend Instructions

This project is a .NET 9.0 backend for the SmartHire application, following a Clean Architecture pattern.

## Architecture
- **[SmartHire.Core](./SmartHire.Core/GEMINI.md)**: Contains domain entities, enums, and core logic. Should have no dependencies on other projects.
- **[SmartHire.Infrastructure](./SmartHire.Infrastructure/GEMINI.md)**: Implements data access using Entity Framework Core (for complex relations/Identity) and Dapper (for performance-critical queries).
- **[SmartHire.API](./SmartHire.API/GEMINI.md)**: The entry point, containing controllers/minimal APIs and OpenAPI configuration.
- **[SmartHire.Worker](./SmartHire.Worker/GEMINI.md)**: Handles background processing (e.g., AI resume feedback, notifications).
- **[SmartHire.Tests](./SmartHire.Tests/GEMINI.md)**: Contains xUnit tests.

## Coding Standards
- **Language**: C# 12/13 (.NET 9.0).
- **Naming**: Use PascalCase for classes, methods, and public properties. Use camelCase for private fields (prefixed with `_`).
- **Nullability**: Nullable reference types are enabled. Use `?` for optional properties and `!` for properties that will be initialized by EF Core.
- **Async**: Prefer `async/await` for all I/O bound operations.
- **DI**: Use constructor injection for all dependencies.

## Data Access
- Use EF Core for most CRUD operations and Identity management.
- Use Dapper for high-performance read-only queries if needed.
- All database-related logic should reside in `SmartHire.Infrastructure`.

## Testing
- Use **xUnit** for unit and integration tests.
- Aim for high coverage of domain logic in `SmartHire.Core`.
- Use mocking (e.g., Moq or NSubstitute) for external dependencies in unit tests.

## Workflows
- When adding a new feature, start with the domain model in `SmartHire.Core`.
- Update migrations in `SmartHire.Infrastructure` if entities change.
- Always add/update tests in `SmartHire.Tests` for any logic changes.
