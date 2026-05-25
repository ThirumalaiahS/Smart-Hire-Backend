# SmartHire.Infrastructure Instructions

The Infrastructure layer handles all interactions with external systems, primarily the database.

## Data Access
- **EF Core**: Use for most database operations. Keep `DbContext` configurations (Fluent API) organized in a `Data` or `Configuration` folder.
- **Dapper**: Use for specific read-heavy or performance-critical queries. Define Dapper-based repository implementations where necessary.
- **Migrations**: Always review generated migrations to ensure they match the intended schema changes.

## Best Practices
- Keep repositories thin. Business logic belongs in `Core`.
- Use `IEntityTypeConfiguration<T>` for complex entity configurations to keep the `DbContext` class clean.
- Ensure connection strings are handled via `IConfiguration` and not hardcoded.
