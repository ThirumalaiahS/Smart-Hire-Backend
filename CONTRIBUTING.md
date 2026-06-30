# Contributing — UI Development Guide for SmartHire

## Overview
This document provides a complete project overview and practical guidance for building the UI for the SmartHire backend API. It covers architecture, API contract expectations, authentication, recommended stacks, security, testing, and developer workflows.

## Project architecture (backend)
- Backend: .NET 9 Web API (SmartHire.API)
- Authentication: ASP.NET Identity with JWT tokens produced by `ITokenService`
- Primary concepts:
  - `AppUser` (Identity user) and `SystemUser` (application user record)
  - APIs are namespaced under `/api/[controller]`
  - Common responses are wrapped in `ApiResponse<T>` with consistent `Success`/`Error` shapes

If you need additional backend details, see the API project files and controllers.

## API contract — key endpoints (observed)
- Auth:
  - `POST /api/auth/register` — registers a user
  - `POST /api/auth/login` — authenticates and returns a token
  - `POST /api/auth/forgot-password` — generates a reset token
  - `POST /api/auth/reset-password` — resets password using provided token
  - `PATCH /api/auth/deactivate/{id}` — (Admin) deactivate account
  - `PATCH /api/auth/activate/{id}` — (Admin) activate account
  - `DELETE /api/auth/delete/{id}` — (Admin) delete account

Notes:
- Endpoints return `ApiResponse<T>`; unwrap `data`, `message`, and `errors` accordingly.
- Authorization uses role-based policies (example: `Authorize(Roles = "Admin")`). Ensure the UI sends the Bearer token in `Authorization` header.
- No refresh token endpoints were found. Tokens may be short-lived — plan for token expiry handling on the client.

## Authentication & token handling (recommended)
- Store access tokens using one of these strategies depending on your threat model:
  - HttpOnly secure cookie (recommended for web apps to mitigate XSS). Requires CORS and same-site configuration on the API.
  - In-memory storage (best for SPA with refresh token flow).
  - `localStorage` only if you accept XSS risk (less recommended).
- Always send `Authorization: Bearer {token}` on protected requests.
- Implement automatic logout when the API returns 401/403 and attempt token refresh only if backend supports it.

## Suggested UI stacks
Pick one based on team skills and product requirements.
- React + TypeScript (recommended)
  - Data: React Query or Redux Toolkit Query
  - Forms: react-hook-form + Yup
  - HTTP: Axios (centralized instance for interceptors)
  - UI: MUI, Ant Design, or Tailwind CSS + component library
- Blazor WebAssembly
  - If you prefer .NET full-stack and typed models end-to-end
- Angular / Vue + TypeScript
  - Equally valid; follow analogous patterns for state, forms and HTTP

## Client architecture patterns
- Folder structure suggestion (React):
  - `src/components` — reusable components
  - `src/pages` — route-level pages (Login, Dashboard, JobApplications, Admin)
  - `src/features` — domain features + hooks/queries (e.g., `auth`, `jobs`, `users`)
  - `src/api` — API clients and typed contracts
  - `src/lib` — utilities, validation schemas, auth helpers
- Centralized API client: create an Axios (or fetch wrapper) with
  - Base URL from env (e.g., `REACT_APP_API_URL`)
  - Request interceptor to attach auth token
  - Response interceptor to handle global errors (401 -> logout)
- Use cancellation (AbortController) for long-running requests; backend controllers accept CancellationToken.

## Role-based UI & routing
- Implement protected routes that check authentication and roles before rendering admin-only pages.
- Do not rely solely on client checks — every sensitive action must be validated by the API.

## Error handling & API responses
- Unwrap `ApiResponse<T>` and surface `errors` to the user with contextual messaging.
- Show friendly messages for common cases:
  - 401: "Session expired, please log in again"
  - Validation errors: show per-field feedback
  - 500: "An unexpected error occurred. Please try again later."

## Forms & validation
- Use client-side validation with the same rules as backend where feasible.
- For registration and password reset, expect backend to return identity errors (strings). Map them to UI fields when possible.

## Security best practices
- Use HTTPS for all environments.
- Prefer HttpOnly cookies for tokens where possible and configure CORS and SameSite properly on the API.
- Sanitize and validate all user input displayed in the UI.
- Enforce CSP headers and other browser hardening options.

## Accessibility & internationalization
- Follow WCAG guidelines when designing components.
- Build with i18n in mind (react-i18next or equivalent) if you plan to support multiple locales.

## Testing
- Unit tests for components and utility functions (Jest + React Testing Library for React)
- Integration tests for critical flows (login, register, job application flow)
- E2E tests (Playwright or Cypress) for user journeys

## CI / Linting / Formatting
- Enforce linting and formatting in CI (ESLint, Prettier for JS/TS; or dotnet-format for Blazor)
- Add pre-commit hooks (husky) to run linters and tests locally

## Local development & environment
- API base URL: configure in environment variables (e.g., `.env.development`)
- Example env vars:
  - `VITE_API_URL` or `REACT_APP_API_URL` — URL to SmartHire.API
  - `NODE_ENV=development`
- When running the backend locally, ensure the database and identity configuration are available and that CORS allows your UI origin.

## Components / Pages to implement (minimum viable set)
- Auth: `Login`, `Register`, `ForgotPassword`, `ResetPassword`
- Dashboard: `Overview` (role-aware)
- Job Applications: list, detail, create (if applicable)
- Admin: `ManageUsers` (activate/deactivate/delete), `UserDetails`
- Shared: `Header`, `Footer`, `PrivateRoute`/`AuthProvider`, `Toast/Notification` service

## API integration tips
- Keep DTO shapes in sync with backend DTOs (`LoginDto`, `RegisterDto`, `UserDto`, etc.)
- Handle paginated endpoints, filters and search consistently
- Respect rate limits and debounce search inputs

## Contribution workflow
- Branch from `develop` for new features: `feature/<short-description>`
- Open PRs targeting `develop` branch with clear description and testing steps
- Include unit tests for new components and hooks

## Where to find more backend details
- Inspect controllers under `SmartHire.API/Controllers` for route names, expected payloads and security attributes.
- Check `SmartHire.Core/DTOs` and `SmartHire.Core/Entities` for exact property names and types.

---

Thank you for contributing to the UI for SmartHire. If you want, I can also generate a starter React + TypeScript project scaffold configured to work with this backend, or produce an OpenAPI/Swagger client if the API exposes Swagger documentation.