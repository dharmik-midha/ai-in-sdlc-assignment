# Project conventions for AI agents

Invoice approval demo app: ASP.NET Core 8 Web API with a small static web UI.

## Layout

- `backend/src/InvoiceApp.Domain`: business rules, no web or storage code. `ApprovalService` owns the invoice lifecycle (Draft, PendingApproval, Approved, Rejected).
- `backend/src/InvoiceApp.Api`: controllers, sign-in (`Auth/`), in-memory repository, and the web UI under `wwwroot/`.
- `backend/tests/InvoiceApp.Tests`: xUnit tests, one test class per class under test, named `<ClassUnderTest>Tests`.
- `e2e`: Playwright end-to-end tests (`tests/`) and page objects (`pages/`).
- `docs/use-cases`: short briefs for features we may build.

## Conventions

- Controllers live in `Controllers/`, one per resource, route `api/[controller]`. Request bodies are `record` types declared inside the controller.
- Controllers inherit `ApiControllerBase` and use `Handle(...)` so service errors map to 404, 409 and 400.
- Every endpoint is behind sign-in. Restrict actions with `[RequireRole("...")]`: destructive actions are Admin only, approvals are Approver or Admin.
- Business rules go in the Domain project, never in controllers.
- Register services in `Program.cs`. Repositories are interfaces in Domain with an in-memory implementation in Api.
- Every new rule or endpoint needs tests. Use `FakeInvoiceRepository` in tests.
- Use clear names; no one-letter variables or `flag` parameters.
- End-to-end tests use page objects and role-based locators (`getByRole`, `getByLabel`), never CSS or XPath selectors.

## Commands

- Run the app: `dotnet run --project backend/src/InvoiceApp.Api` then open http://localhost:5000
- Run the tests: `dotnet test backend/InvoiceApp.sln`
- Run end-to-end tests: `cd e2e && npm install && npx playwright install chromium && npm test`

## Demo accounts (not real credentials)

- `demo.approver` / `ChangeMe123!` (role Approver)
- `demo.admin` / `ChangeMe123!` (role Admin)

## Approval Rules (Role-Based Limits)

### Single Approver Limits
- **Manager**: Can approve invoices up to $50,000
- **Director**: Can approve invoices up to $500,000
- **Admin**: Can approve any amount (but amounts >$500k still need two approvers)

### Two-Approver Requirements (Invoices > $500,000)
- Must have TWO distinct approvers
- First approver: Can be any role (Manager, Director, or Admin)
- Second approver: MUST be Director or Admin
- Same user CANNOT be both first and second approver

### Rejection Rules
- Rejected approval must return clear, specific error message
- Invoice state must remain UNCHANGED (stay in PendingApproval)
- Error messages must include the actual invoice amount

### Review Expectations
- Test EVERY approval rule including edge cases
- **Critical negative tests**: Same user twice, non-director as second approver
- Verify invoice state unchanged on all rejections
- Mutation testing score should be >80% on approval logic

## Never

- Commit real credentials, tokens or customer data.
- Add business logic to `Program.cs` or controllers.
- Skip negative test scenarios (they're where bugs hide).
