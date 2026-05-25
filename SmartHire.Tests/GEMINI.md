# SmartHire.Tests Instructions

This project contains all tests for the solution.

## Testing Standards
- **Framework**: xUnit.
- **Naming**: Use the `UnitOfWork_StateUnderTest_ExpectedBehavior` naming convention (e.g., `AddApplication_ValidInput_ReturnsSuccess`).
- **Pattern**: Follow the **Arrange-Act-Assert** (AAA) pattern.

## Guidelines
- **Unit Tests**: Focus on testing logic in `SmartHire.Core`. Mock all external dependencies.
- **Integration Tests**: Test the interaction between layers, typically involving a test database (e.g., SQLite in-memory or Docker-based SQL Server).
- **Coverage**: Prioritize coverage for complex business rules and edge cases.
