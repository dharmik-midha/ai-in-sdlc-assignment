---
inclusion: auto
name: approval-rules
description: Role-based approval limits and validation rules
---

# Approval Rules Context

When implementing approval logic, follow these rules:

## Role Limits
- Manager: $50,000
- Director: $500,000
- Admin: Unlimited (but >$500k needs two approvers)

## Two-Approver Requirements
- Invoices > $500,000 require TWO approvers
- First approver: Any role
- Second approver: MUST be Director or Admin
- Same user CANNOT approve twice

## Validation Rules
1. Check amount limits BEFORE state changes
2. For >$500k, check if this is first or second approval
3. On second approval, validate distinct user and Director role
4. Throw InvalidOperationException with clear message
5. Include actual invoice amount in error messages

## Error Messages
- Manager over limit: "Manager role can only approve invoices up to $50,000. This invoice amount is $XX,XXX.XX."
- Director over limit: "Invoices over $500,000 require two approvers. This invoice amount is $XX,XXX.XX."
- Same user twice: "The same user cannot approve an invoice twice. This invoice was already approved by {username}."
- Wrong role second: "Second approver for amounts over $500,000 must be a Director or Admin. Current role: {role}."

## Testing Requirements
- Test EVERY rule with positive and negative cases
- Verify invoice status unchanged on rejection
- Test error message content
- Mutation testing score must be >80%
