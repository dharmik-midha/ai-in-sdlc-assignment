# Specification: Role-Based Approval Limits

**ID**: RBAC-001  
**Created**: 2026-10-30  
**Status**: Implementation Ready

## Problem
Current system allows any approver to approve any invoice amount. Need role-based limits.

## Requirements

### FR-1: Manager Approval Limit ($50,000)
```gherkin
Given a user with Manager role
When they approve an invoice ≤ $50,000
Then approval succeeds
```

### FR-2: Manager Cannot Exceed Limit
```gherkin
Given a user with Manager role  
When they attempt to approve invoice > $50,000
Then system throws InvalidOperationException
And error message states "Manager role can only approve invoices up to $50,000"
And invoice remains in PendingApproval status
```

### FR-3: Director Approval Limit ($500,000)
```gherkin
Given a user with Director role
When they approve an invoice ≤ $500,000  
Then approval succeeds
```

### FR-4: Two-Approver Requirement (> $500,000)
```gherkin
Given an invoice amount > $500,000
When first approver (any role) approves
Then invoice remains PendingApproval
And stores first approver info
When second approver (Director/Admin) approves  
Then invoice moves to Approved
```

### FR-5: Same User Cannot Approve Twice
```gherkin
Given invoice has first approver
When same user attempts second approval
Then system throws InvalidOperationException
And error states "Same user cannot approve invoice twice"
```

### FR-6: Second Approver Must Be Director
```gherkin
Given invoice has first approver (Manager)
When second approver is also Manager
Then system throws InvalidOperationException
And error states "Second approver must be Director or Admin"
```

## Domain Model Changes

### New Enum: ApproverRole
```csharp
public enum ApproverRole {
    Manager,   // $50k limit
    Director,  // $500k limit
    Admin      // Unlimited
}
```

### Extended Invoice Fields
```csharp
public string? FirstApprovedBy { get; set; }
public DateTime? FirstApprovedAt { get; set; }
public string? SecondApprovedBy { get; set; }
public DateTime? SecondApprovedAt { get; set; }
```

## Implementation Tasks

### Task 1: Single-Approver Limits
**Files**: ApproverRole.cs (new), ApprovalService.cs, ApprovalsController.cs  
**Tests**: Manager $50k pass/fail, Director $500k pass/fail  
**Commit**: "Implement single-approver role-based limits"

### Task 2: Two-Approver Logic
**Files**: Invoice.cs (add tracking), ApprovalService.cs (HandleTwoApprover)  
**Tests**: Two-approver flow, same-user rejection, role validation  
**Commit**: "Add two-approver requirement for high-value invoices"

## Test Scenarios
- ✅ Manager approves $30k → Success
- ✅ Manager attempts $60k → Reject "Manager limit $50k"
- ✅ Director approves $250k → Success
- ✅ Director attempts $600k alone → Reject "Need two approvers"
- ✅ Manager then Director approve $750k → Success
- ❌ Same user twice on $750k → Reject "Same user"
- ❌ Manager then Manager on $750k → Reject "Second must be Director"

## Success Criteria
- All unit tests pass
- Mutation score > 80%
- Clear error messages with amounts
- Invoice state unchanged on rejection
