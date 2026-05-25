# SmartHire.Worker Instructions

The Worker project handles long-running background tasks and message processing.

## Principles
- **Idempotency**: Ensure that background tasks are idempotent, as they may be retried upon failure.
- **Logging**: Provide detailed logging for background processes to facilitate troubleshooting.
- **Error Handling**: Implement robust error handling and retry logic (e.g., using Polly or built-in message queue features).

## Best Practices
- Use `BackgroundService` or `IHostedService` for scheduled or continuous tasks.
- Keep the worker logic focused on orchestration; delegate heavy lifting to domain services in `Core`.
