# Implementation Tasks - RBAC Approval Limits

Generated from SPEC.md using spec-driven approach

## Task 1: Domain Model Extensions

**Agent**: implementation-agent  
**Estimated**: 30 min  
**Depends on**: SPEC.md

### Subtasks
1. Create `ApproverRole` enum with Manager/Director/Admin
2. Add first/second approver fields to `Invoice.cs`
3. Verify compilation

### Files
- NEW: `backend/src/InvoiceApp.Domain/ApproverRole.cs`
- MODIFY: `backend/src/InvoiceApp.Domain/Invoice.cs`

### Acceptance Criteria
- Code compiles without errors
- Enum has XML documentation
- Invoice fields are nullable

---

## Task 2: Single-Approver Limits Implementation

**Agent**: implementation-agent  
**Estimated**: 1 hour  
**Depends on**: Task 1

### Subtasks
1. Update `ApprovalService.Approve` signature to accept `ApproverRole`
2. Add `ValidateApprovalAuthority` method
3. Implement Manager $50k limit check
4. Implement Director $500k limit check
5. Update `ApprovalsController` to pass role from session

### Files
- MODIFY: `backend/src/InvoiceApp.Domain/ApprovalService.cs`
- MODIFY: `backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs`

### Acceptance Criteria
- Manager can approve ≤ $50k
- Manager cannot approve > $50k (throws InvalidOperationException)
- Director can approve ≤ $500k
- Director cannot approve > $500k alone (throws InvalidOperationException)
- Error messages include actual amounts

---

## Task 3: Unit Tests for Single-Approver

**Agent**: test-agent  
**Estimated**: 45 min  
**Depends on**: Task 2

### Subtasks
1. Test Manager approves $30k (positive)
2. Test Manager attempts $60k (negative)
3. Test Director approves $250k (positive)
4. Test Director attempts $600k (negative)
5. Verify error messages
6. Verify invoice state unchanged on rejection

### Files
- MODIFY: `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`

### Acceptance Criteria
- All tests pass
- Negative scenarios tested
- Error messages validated
- Code coverage > 85%

---

## Task 4: COMMIT 1 - Single-Approver Limits

**Commit Message**:
```
Implement single-approver role-based limits

- Add ApproverRole enum (Manager/Director/Admin)
- Extend Invoice model with approval tracking
- Implement Manager $50k and Director $500k limits
- Update controller to pass role from session
- Add 6 unit tests (positive + negative scenarios)
- Validate error messages include actual amounts
```

**Verify**:
- `dotnet build` succeeds
- `dotnet test` all pass
- Git log shows clear commit message

---

## Task 5: Two-Approver Logic Implementation

**Agent**: implementation-agent  
**Estimated**: 1.5 hours  
**Depends on**: Task 4 (committed)

### Subtasks
1. Add `HandleTwoApproverApproval` method to ApprovalService
2. Detect when amount > $500k
3. Store first approver without changing status
4. Validate second approver is distinct user
5. Validate second approver is Director/Admin
6. Mark approved only after valid second approval

### Files
- MODIFY: `backend/src/InvoiceApp.Domain/ApprovalService.cs`

### Acceptance Criteria
- Amounts > $500k enter two-approver flow
- First approval stores info, keeps PendingApproval status
- Second approval validates distinct user + Director role
- Invoice moves to Approved only after valid second approval

---

## Task 6: Unit Tests for Two-Approver

**Agent**: test-agent  
**Estimated**: 1 hour  
**Depends on**: Task 5

### Subtasks
1. Test Manager+Director approve $750k (positive)
2. Test Director+Director approve $750k (positive)
3. Test same user attempts twice (negative - CRITICAL)
4. Test Manager+Manager attempt (negative - CRITICAL)
5. Verify first approval doesn't change status
6. Verify second approval changes to Approved
7. Verify error messages

### Files
- MODIFY: `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`

### Acceptance Criteria
- All positive scenarios pass
- Both critical negative scenarios tested and pass
- Error messages validated
- Code coverage > 90%

---

## Task 7: COMMIT 2 - Two-Approver Logic

**Commit Message**:
```
Add two-approver requirement for high-value invoices

- Implement two-approver flow for amounts > $500k
- First approval stores approver, keeps status pending
- Second approval validates distinct user and Director role
- Add 6 unit tests for two-approver scenarios
- Include critical negative tests (same user, wrong role)
- All 12 tests passing
```

**Verify**:
- `dotnet build` succeeds
- `dotnet test` shows 12+ tests passing
- Mutation test score > 80%

---

## Task 8: E2E Test Setup

**Agent**: test-agent  
**Estimated**: 30 min  
**Depends on**: Task 7 (committed)

### Subtasks
1. Create LoginPage page object
2. Create InvoicesPage page object
3. Use role-based locators (getByRole, getByLabel)

### Files
- NEW: `e2e/pages/LoginPage.ts`
- NEW: `e2e/pages/InvoicesPage.ts`

### Acceptance Criteria
- Page objects use role-based locators
- No CSS or XPath selectors
- Helper methods for common actions

---

## Task 9: E2E Test Implementation

**Agent**: test-agent  
**Estimated**: 45 min  
**Depends on**: Task 8

### Subtasks
1. Write test: Manager attempts to approve $75k
2. Verify error message displayed
3. Verify invoice status remains Pending
4. Capture screenshot of error message

### Files
- NEW: `e2e/tests/approval-limits.spec.ts`

### Acceptance Criteria
- Test uses page objects
- Error message validated
- Invoice state verified
- Screenshot captured as proof

---

## Task 10: COMMIT 3 - E2E Test

**Commit Message**:
```
Add E2E test for approval limit validation

- Create LoginPage and InvoicesPage page objects
- Implement test for Manager over-limit rejection
- Use role-based locators per AGENTS.md conventions
- Verify error message and invoice state
- Include screenshot proof in test output
```

**Verify**:
- `cd e2e && npm test` passes
- Screenshot exists in test-results/

---

## Task 11: Mutation Testing

**Agent**: main  
**Estimated**: 20 min  
**Depends on**: Task 10

### Commands
```bash
cd backend/tests/InvoiceApp.Tests
dotnet tool install -g dotnet-stryker --version 4.13.0
dotnet stryker
```

### Acceptance Criteria
- Mutation score > 80%
- Report generated in StrykerOutput/
- Scores documented in logs

---

## Task 12: PR Creation & AI Review

**Agent**: main  
**Estimated**: 30 min  
**Depends on**: Task 11

### Subtasks
1. Push all commits to GitHub
2. Create Pull Request
3. Run AI review on the diff
4. Document findings with verdicts
5. Update PR description

### Acceptance Criteria
- PR has clear description
- All findings listed with verdicts
- AI review logged

---

## Summary

**Total Tasks**: 12  
**Estimated Time**: ~8 hours  
**Commits**: 3 (single-approver, two-approver, e2e)  
**Agents Used**: implementation-agent, test-agent, main  
**Sub-agents**: context-gatherer (already used)
