---
name: rbac-approval
description: Skill for implementing role-based approval limits
---

# RBAC Approval Implementation Skill

## Context
This skill guides implementation of role-based access control for invoice approvals.

## Key Patterns

### 1. Validation Before State Change
```csharp
// ALWAYS validate BEFORE changing invoice state
ValidateApprovalAuthority(invoice, approver, role);
invoice.Status = InvoiceStatus.Approved; // Only after validation passes
```

### 2. Two-Approver Detection
```csharp
// Check if two approvers needed
if (invoice.Amount > 500_000m)
{
    return HandleTwoApproverApproval(invoice, approver, role);
}
```

### 3. Error Messages with Context
```csharp
throw new InvalidOperationException(
    $"Manager role can only approve invoices up to ${ManagerLimit:N0}. " +
    $"This invoice amount is ${invoice.Amount:N2}.");
```

### 4. Distinct User Validation
```csharp
if (invoice.FirstApprovedBy == approver)
{
    throw new InvalidOperationException(
        "The same user cannot approve an invoice twice. " +
        $"This invoice was already approved by {approver}.");
}
```

## Implementation Order
1. Add ApproverRole enum
2. Extend Invoice model with first/second approver fields
3. Update Approve method signature to include role
4. Implement single-approver validation
5. Implement two-approver logic
6. Update controller to pass role
7. Write unit tests for all scenarios
8. Write E2E test for rejection case

## Testing Checklist
- [ ] Manager approves $30k (pass)
- [ ] Manager attempts $60k (fail)
- [ ] Director approves $250k (pass)  
- [ ] Director attempts $600k alone (fail)
- [ ] Manager+Director approve $750k (pass)
- [ ] Same user twice on $750k (fail)
- [ ] Manager+Manager on $750k (fail)
- [ ] Invoice state unchanged on all failures

## Common Pitfalls
- ❌ Checking single limits before two-approver detection
- ❌ Not including actual amounts in error messages
- ❌ Forgetting to validate second approver role
- ❌ Changing invoice state before validation
- ❌ Not testing negative scenarios
