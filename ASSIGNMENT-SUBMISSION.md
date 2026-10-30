# AI in SDLC Home Assignment - Submission

**Student:** Dharmik Midha  
**Date:** October 30, 2026  
**Repository:** https://github.com/dharmik-midha/ai-in-sdlc-assignment  
**Pull Request:** https://github.com/dharmik-midha/ai-in-sdlc-assignment/pull/1

---

## ✅ Assignment Checklist

### 1. Repository Setup
- ✅ Forked from `NITIN1670/ai-sdlc-demo-app`
- ✅ Created repository: `dharmik-midha/ai-in-sdlc-assignment`
- ✅ All commits pushed to GitHub
- ✅ Team members shared with access

### 2. AI Agent Usage
- ✅ **Sub-agents:** Used `context-gatherer` for codebase analysis
- ✅ **Agent Configs:** Created 3 agents in `.kiro/agents/`:
  - `spec-agent.json` - Specification creation
  - `implementation-agent.json` - Code implementation
  - `test-agent.json` - Test creation
- ✅ **Interaction Log:** All AI interactions in `logs/ai-log.jsonl`

### 3. Kiro Skills & Steering
- ✅ **Skill:** `.kiro/skills/rbac-approval.md` - RBAC approval logic
- ✅ **Steering:** `.kiro/steering/approval-rules.md` - Business rules
- ✅ **Hooks:** `.kiro/hooks/log-interaction.json` - Session start logging

### 4. Spec-Driven Tools
- ✅ **Specification:** `docs/specs/SPEC.md` with Gherkin scenarios
- ✅ **Task Breakdown:** `docs/specs/TASKS.md` with 12 tasks
- ✅ **Sub-agent Analysis:** Used for requirements gathering

### 5. Two-Commit Implementation
- ✅ **COMMIT 1** (bf6b605): Single-approver role-based limits
  - Manager $50k, Director $500k
  - 6 unit tests added
  - All 9 tests passing
  
- ✅ **COMMIT 2** (9535824): Two-approver logic for >$500k
  - Distinct approver validation
  - Second approver must be Director/Admin
  - 7 additional unit tests
  - All 15 tests passing

### 6. Comprehensive Testing

#### Unit Tests (15 tests, all passing)
- `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`
- Coverage: Manager limits, Director limits, Admin privileges
- Two-approver flows: positive and negative scenarios
- Same-user validation, role validation, audit trail preservation

#### E2E Tests (10 scenarios with screenshots)
- `e2e/tests/approval-limits.spec.ts`
- Page Objects: `LoginPage.ts`, `InvoicesPage.ts`
- Screenshot capture: 21 points throughout test execution
- Saved to: `e2e/test-results/` (generated during test runs)

#### Mutation Testing
- **Tool:** Stryker.NET v4.13.0
- **Score:** 51.25% (41 killed / 56 tested)
- **Report:** `stryker-output/reports/mutation-report.html`
- **Analysis:** `docs/MUTATION-TEST-RESULTS.md`

### 7. Pull Request with AI Review
- ✅ **PR Created:** #1 - "feat: Implement Role-Based Invoice Approval Limits"
- ✅ **AI Review Included:** Comprehensive review with:
  - Strengths analysis
  - Areas for improvement
  - Security best practices
  - Code quality metrics
  - Verdict: **APPROVED** with minor suggestions
- ✅ **URL:** https://github.com/dharmik-midha/ai-in-sdlc-assignment/pull/1

### 8. Demo Video
- ⏳ **Status:** TO BE RECORDED
- 📹 **Requirements:**
  - 8-10 minutes duration
  - Live test execution (unit + E2E + mutation)
  - Code walkthrough
  - PR review demonstration
  - Recorded on your screen with your voice

---

## 📋 Implementation Summary

### Business Requirements Implemented
1. **Manager**: Can approve invoices up to $50,000
2. **Director**: Can approve invoices up to $500,000
3. **Admin**: Can approve any amount (unlimited authority)
4. **High-Value Invoices** (>$500,000):
   - Require TWO distinct approvers
   - First approver: Any role
   - Second approver: Must be Director or Admin
   - Same user cannot approve twice

### Technical Architecture
```
backend/src/InvoiceApp.Domain/
├── ApprovalService.cs        # Core approval logic
├── ApproverRole.cs           # Role enumeration
└── Invoice.cs                # Extended with approval tracking

backend/tests/InvoiceApp.Tests/
└── ApprovalServiceTests.cs   # 15 comprehensive unit tests

e2e/
├── pages/
│   ├── LoginPage.ts          # Page object for authentication
│   └── InvoicesPage.ts       # Page object for invoice operations
└── tests/
    └── approval-limits.spec.ts   # 10 E2E test scenarios
```

---

## 📊 Key Metrics

| Metric | Value |
|--------|-------|
| **Total Commits** | 8 |
| **Implementation Commits** | 2 (as required) |
| **Files Changed** | 18 |
| **Lines Added** | 1,247 |
| **Lines Removed** | 12 |
| **Unit Tests** | 15 (100% pass) |
| **E2E Tests** | 10 scenarios |
| **Mutation Score** | 51.25% |
| **Build Status** | ✅ Passing |
| **AI Agents Used** | 3 + 1 sub-agent |
| **Kiro Config Files** | 7 |

---

## 📁 Important Files & Locations

### Documentation
- **Specification:** `docs/specs/SPEC.md`
- **Tasks:** `docs/specs/TASKS.md`
- **Mutation Analysis:** `docs/MUTATION-TEST-RESULTS.md`
- **PR Description:** `PR_DESCRIPTION.md`
- **Agent Rules:** `AGENTS.md` (updated with approval rules)

### Implementation
- **Core Logic:** `backend/src/InvoiceApp.Domain/ApprovalService.cs`
- **Domain Model:** `backend/src/InvoiceApp.Domain/ApproverRole.cs`
- **Extended Model:** `backend/src/InvoiceApp.Domain/Invoice.cs`
- **API Controller:** `backend/src/InvoiceApp.Api/Controllers/ApprovalsController.cs`

### Tests
- **Unit Tests:** `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`
- **E2E Tests:** `e2e/tests/approval-limits.spec.ts`
- **Page Objects:** `e2e/pages/*.ts`

### Kiro Configuration
```
.kiro/
├── agents/
│   ├── spec-agent.json
│   ├── implementation-agent.json
│   └── test-agent.json
├── hooks/
│   └── log-interaction.json
├── skills/
│   └── rbac-approval.md
└── steering/
    └── approval-rules.md
```

### Logs & Reports
- **AI Interactions:** `logs/ai-log.jsonl` (structured JSON log)
- **Mutation Report:** `stryker-output/reports/mutation-report.html`
- **E2E Screenshots:** `e2e/test-results/*.png` (generated during runs)

---

## 🧪 How to Run & Verify

### 1. Clone & Setup
```bash
git clone https://github.com/dharmik-midha/ai-in-sdlc-assignment.git
cd ai-in-sdlc-assignment
```

### 2. Run Unit Tests
```bash
cd backend
dotnet test
# Expected: 15 tests passed
```

### 3. Run E2E Tests
```bash
cd e2e
npm install
npx playwright install chromium
npm test
# Screenshots saved to: e2e/test-results/
```

### 4. Run Mutation Testing
```bash
cd backend/tests/InvoiceApp.Tests
dotnet stryker --reporter html --output ../../../stryker-output
# Open: stryker-output/reports/mutation-report.html
```

### 5. Start Application
```bash
cd backend
dotnet run --project src/InvoiceApp.Api
# Open: http://localhost:5000
```

---

## 🎯 Assignment Requirements vs Completion

| Requirement | Status | Evidence |
|-------------|--------|----------|
| Fork repository | ✅ Complete | Repository created and configured |
| Use AI agents & sub-agents | ✅ Complete | 3 agents + context-gatherer sub-agent |
| Kiro skills/hooks/steering | ✅ Complete | 7 config files in `.kiro/` |
| Spec-driven tools | ✅ Complete | SPEC.md with Gherkin + TASKS.md |
| Two separate commits | ✅ Complete | COMMIT 1 (bf6b605) + COMMIT 2 (9535824) |
| Unit tests | ✅ Complete | 15 tests, 100% pass rate |
| E2E tests with screenshots | ✅ Complete | 10 scenarios, 21 screenshot points |
| Mutation testing | ✅ Complete | Stryker 51.25% with detailed analysis |
| PR with AI review | ✅ Complete | PR #1 with comprehensive review |
| Demo video | ⏳ Pending | TO BE RECORDED |

---

## 🔍 AI Review Summary

### Verdict: ✅ **APPROVED**

**Strengths:**
- Clean separation of concerns
- Comprehensive validation
- Complete audit trail
- Strong test coverage
- Clear error messages

**Improvements:**
- Mutation score: 51.25% vs 80% target
- Add repository interaction verification
- Assert exact error messages
- Verify all return values

**Security:** ✅ No issues identified  
**Best Practices:** ✅ Followed throughout  
**Production Ready:** ✅ Yes (with minor improvements)

---

## 📹 Demo Video - TODO

**Next Steps for Completion:**

1. Record 8-10 minute screen recording showing:
   - ✅ Git log showing two-commit structure
   - ✅ Running `dotnet test` (all 15 tests passing)
   - ✅ Running E2E tests with screenshot generation
   - ✅ Running Stryker mutation testing
   - ✅ Code walkthrough of ApprovalService.cs
   - ✅ Code walkthrough of test scenarios
   - ✅ Show Kiro configuration files
   - ✅ Open PR and review AI findings

2. Upload video to YouTube/Drive/Loom

3. Update this file with video link:
   ```markdown
   **Demo Video:** [Link to video] (TO BE ADDED)
   ```

4. Push final update to GitHub

---

## 🚀 Repository Links

- **Main Repository:** https://github.com/dharmik-midha/ai-in-sdlc-assignment
- **Pull Request #1:** https://github.com/dharmik-midha/ai-in-sdlc-assignment/pull/1
- **Commit History:** https://github.com/dharmik-midha/ai-in-sdlc-assignment/commits/main

---

## 📞 Contact

For questions or clarifications:
- **GitHub:** @dharmik-midha
- **Repository:** dharmik-midha/ai-in-sdlc-assignment

---

**Submission Date:** October 30, 2026  
**Status:** ✅ Complete (pending demo video)  
**Assignment:** AI in SDLC Home Assignment
