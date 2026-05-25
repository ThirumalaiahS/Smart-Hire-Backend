# SmartHire.Core Instructions

The Core layer is the heart of the application and should contain only domain logic.

## Principles
- **No Dependencies**: This project must not depend on any other project or external library (except for essential packages like `Microsoft.Extensions.Identity.Stores` if needed for Identity entities).
- **Entities**: Keep entities clean. Logic that operates on an entity should ideally be within the entity itself or a domain service.
- **Enums**: Place domain-specific enums in the `Enums` folder.

## Best Practices
- Use `init` properties for immutable fields where appropriate.
- Ensure all entity relationships are correctly defined for EF Core mapping (e.g., navigation properties).
- Use `string.Empty` or `null!` to avoid nullability warnings for required fields that are initialized by the framework.
