# SmartHire.API Instructions

The API layer is the entry point for all client requests.

## Development Standards
- **Minimal APIs vs. Controllers**: Prefer Minimal APIs for simple endpoints and Controllers for more complex resource management.
- **DTOs**: Always use Data Transfer Objects (DTOs) for requests and responses. Never expose domain entities directly.
- **Validation**: Use FluentValidation (if available) to validate incoming requests.

## Best Practices
- Keep endpoints thin. Delegate logic to services or handlers.
- Use proper HTTP status codes (e.g., 201 Created, 204 No Content, 400 Bad Request, 404 Not Found).
- Ensure OpenAPI (Swagger) documentation is accurate and up-to-date.
