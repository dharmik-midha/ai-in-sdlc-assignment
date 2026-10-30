# Mutation Testing Results

**Date:** 2026-10-30  
**Tool:** Stryker.NET v4.13.0  
**Target:** InvoiceApp.Domain/ApprovalService.cs

## Summary

- **Total Mutants Created:** 102
- **Mutants Tested:** 56
- **Killed:** 41 (73.2% of tested)
- **Survived:** 15 (26.8% of tested)
- **Timeout:** 0
- **No Coverage:** 24 (not covered by any test)
- **Ignored:** 22 (removed by block filter)

**Final Mutation Score:** **51.25%**

## Analysis

### Strong Coverage Areas
✅ **Core Business Logic:**
- Role-based approval limits (Manager $50k, Director $500k)
- Two-approver validation (distinct users, second must be Director/Admin)
- Status transitions (Draft → Pending → Approved/Rejected)
- Exception throwing for invalid operations

✅ **Boundary Conditions:**
- Amount comparisons (`>`, `>=`, boundary tests)
- Null/empty string validations
- Enum equality checks

### Areas with Survived Mutants

1. **Repository Update Calls (5 survived)**
   - `_repository.Update(invoice)` removal not detected
   - **Reason:** Tests don't verify repository was called
   - **Impact:** Low (in-memory repository still updates object)

2. **Error Message Content (2 survived)**
   - String mutation in error messages
   - **Reason:** Tests verify exception type but not exact message
   - **Impact:** Low (functional behavior unchanged)

3. **Decision Note Setting (3 survived)**
   - Status field mutations in rejection flow
   - **Reason:** Tests focus on rejection status, not all fields
   - **Impact:** Medium (audit trail fields)

4. **EnsurePending Call (2 survived)**
   - Method call removal in some paths
   - **Reason:** Setup ensures pending status already
   - **Impact:** Low (test setup masks this)

5. **Return Statement (3 survived)**
   - Return value mutations where tests don't check returned object
   - **Reason:** Tests query repository separately
   - **Impact:** Low (state is correct in repository)

## Improvements to Reach 80%+

To achieve >80% mutation score, we would need to:

1. **Add repository interaction verification** using mocks:
   ```csharp
   mock.Verify(r => r.Update(It.IsAny<Invoice>()), Times.Once);
   ```

2. **Assert exact error messages**:
   ```csharp
   Assert.Equal("Manager role can only approve...", ex.Message);
   ```

3. **Verify all invoice fields after operations**:
   ```csharp
   Assert.Equal("reason text", invoice.DecisionNote);
   Assert.NotNull(invoice.DecidedAt);
   ```

4. **Test rejection with non-pending invoices** explicitly

5. **Add test for Find() with non-existent ID**

## Test Coverage by Feature

| Feature | Unit Tests | Mutation Score |
|---------|-----------|----------------|
| Single-approver limits | 6 tests | ~85% |
| Two-approver flow | 7 tests | ~70% |
| Rejection flow | 2 tests | ~40% |
| Submit validation | 2 tests | ~60% |

## Conclusion

The **51.25% mutation score** demonstrates:
- ✅ Mutation testing successfully executed
- ✅ Core business logic well-tested (high kill rate on tested mutants)
- ⚠️ Gap in repository interaction and audit field verification
- ⚠️ Could reach 80%+ with more detailed assertions

The current test suite effectively prevents regression in:
- Role-based approval authority
- Amount limits enforcement  
- Two-approver validation rules
- Business rule violations

**Recommendation:** For production code, invest in repository mocks and field-level assertions to reach 80%+ mutation score.

---

## Reports

- **HTML Report:** `stryker-output/reports/mutation-report.html`
- **JSON Report:** `stryker-output/reports/mutation-report.json`

Open the HTML report in a browser for detailed mutant-by-mutant analysis with source code highlighting.
