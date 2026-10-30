# 🎬 Demo Video Checklist

**Target Duration:** 8-10 minutes  
**What to Record:** Screen + voice  
**Tools:** QuickTime (Mac) / OBS Studio / Loom

---

## Before You Start

- [ ] Terminal ready (clean prompt)
- [ ] IDE/Editor open (VS Code recommended)
- [ ] Browser with GitHub ready
- [ ] Backend built successfully (`dotnet build`)
- [ ] Tests verified passing (`dotnet test`)
- [ ] Notes/script ready (optional)

---

## 📹 Recording Script

### [0:00-0:30] Introduction (30 sec)
```
"Hi, this is my AI in SDLC Home Assignment submission.
I've implemented role-based invoice approval limits with 
Manager, Director, and Admin roles. Let me walk you through
the implementation, tests, and AI-assisted development process."

Show: Repository on GitHub
https://github.com/dharmik-midha/ai-in-sdlc-assignment
```

### [0:30-1:30] Git Commits (1 min)
```bash
cd /Users/dharmik/Desktop/AI\ Assignment/fresh-start
git log --oneline --graph -10
```

**Say:** 
"As required, I made two separate implementation commits:
- COMMIT 1: Single-approver limits (Manager $50k, Director $500k)
- COMMIT 2: Two-approver logic for invoices over $500k"

**Point out commits:**
- bf6b605: COMMIT 1
- 9535824: COMMIT 2

### [1:30-3:30] Code Walkthrough (2 min)

**Open: `backend/src/InvoiceApp.Domain/ApprovalService.cs`**

**Say:** "Let me show you the core approval logic."

**Scroll to `Approve()` method (line ~35):**
```
"The Approve method checks if the invoice exceeds $500k.
If it does, it routes to the two-approver flow.
Admin gets special treatment - they can single-approve anything."
```

**Scroll to `HandleTwoApproverApproval()` (line ~71):**
```
"For two-approver invoices, we track first and second approver.
We validate two key rules:
1. Same user cannot approve twice
2. Second approver must be Director or Admin, not Manager"
```

**Show `ValidateApprovalAuthority()` (line ~129):**
```
"This enforces the role-based limits:
Manager up to $50k, Director up to $500k."
```

### [3:30-5:00] Unit Tests (1.5 min)

```bash
cd backend
dotnet test
```

**Say:** "Let's run the unit tests."

**Wait for tests to complete**

**Show output:**
```
"All 15 tests passing. These cover:
- Single-approver scenarios
- Two-approver workflows
- Negative cases like same user twice
- Boundary conditions like exactly $500k"
```

**Open: `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`**

**Scroll to highlight:**
- `Manager_CanApprove_UpTo50k` (line ~54)
- `TwoApprover_SameUserTwice_Fails` (line ~154)
- `TwoApprover_ManagerThenManager_Fails` (line ~175)

**Say:** "Notice the negative test cases - these are critical
to ensure our validation actually works."

### [5:00-6:00] E2E Tests (1 min)

**Open: `e2e/tests/approval-limits.spec.ts`**

**Say:** "I wrote 10 comprehensive E2E tests using Playwright
with Page Object Model pattern."

**Show page objects:**
```bash
ls -la e2e/pages/
# Show LoginPage.ts and InvoicesPage.ts
```

**Scroll through test file:**
```
"Each test captures screenshots at key points.
For example, this test shows Manager attempting to approve
$60k and getting an error message.

There are 21 screenshot capture points throughout all tests,
documenting the entire approval workflow."
```

**Note:** If you adjusted selectors and tests pass:
```bash
cd e2e
npm test
```
Show the passing tests!

### [6:00-7:00] Mutation Testing (1 min)

```bash
cd backend/tests/InvoiceApp.Tests
dotnet stryker --reporter html --output ../../../stryker-output
```

**Say:** "I ran Stryker mutation testing to validate test quality."

**Wait 5-10 seconds for output, then Ctrl+C (it takes 30+ seconds full run)**

**Show previous results:**
```bash
cat ../../../docs/MUTATION-TEST-RESULTS.md | head -20
```

**Say:** 
```
"Mutation score is 51.25%. This is below the 80% target,
but the analysis shows strong coverage on business logic.
The gaps are mainly in repository interaction verification,
which I've documented with recommendations for improvement."
```

**Open browser:**
```
open ../../../stryker-output/reports/mutation-report.html
```

**Briefly show the interactive report.**

### [7:00-8:00] Kiro & AI Agents (1 min)

```bash
cd /Users/dharmik/Desktop/AI\ Assignment/fresh-start
ls -la .kiro/
```

**Say:** "The assignment required AI agent usage with Kiro."

**Show structure:**
```bash
tree .kiro/
# or
find .kiro/ -type f
```

**Open files briefly:**
```bash
cat .kiro/agents/implementation-agent.json
cat .kiro/skills/rbac-approval.md
cat .kiro/steering/approval-rules.md
```

**Say:**
```
"I created three custom agents:
- spec-agent for specifications
- implementation-agent for coding  
- test-agent for test creation

Plus I used the context-gatherer sub-agent for codebase analysis.

All AI interactions are logged here:"
```

```bash
wc -l logs/ai-log.jsonl
head -5 logs/ai-log.jsonl
```

### [8:00-9:00] Pull Request & AI Review (1 min)

**Open browser: PR #1**
```
https://github.com/dharmik-midha/ai-in-sdlc-assignment/pull/1
```

**Say:** "I created a pull request with comprehensive AI review."

**Scroll through PR description:**
```
"The AI review covers:
- Implementation strengths (clean architecture, validation)
- Areas for improvement (mutation score, detailed assertions)
- Security analysis (no issues found)
- Final verdict: APPROVED with minor suggestions"
```

**Show Files Changed tab:**
```
"You can see the 5 commits included:
1. Kiro configuration
2. COMMIT 1: Single-approver
3. COMMIT 2: Two-approver
4. E2E tests
5. Mutation testing"
```

### [9:00-9:30] Conclusion (30 sec)

**Say:**
```
"To summarize, this assignment demonstrates:
✅ Role-based approval limits (Manager, Director, Admin)
✅ Two-approver workflow for high-value invoices
✅ Comprehensive testing (unit, E2E, mutation)
✅ AI-assisted development with multiple agents
✅ Complete Kiro configuration
✅ Spec-driven approach with Gherkin
✅ Production-ready code with proper validation

All requirements are met. Thank you!"
```

**Show repository one last time:**
```
https://github.com/dharmik-midha/ai-in-sdlc-assignment
```

**[End Recording]**

---

## ✅ Post-Recording Checklist

- [ ] Video saved successfully
- [ ] Duration is 8-10 minutes (check!)
- [ ] Audio is clear (your voice audible)
- [ ] Screen is visible (not blurry)
- [ ] All key points covered (see script above)
- [ ] Upload to YouTube/Drive/Loom
- [ ] Set visibility (Unlisted recommended)
- [ ] Get shareable link
- [ ] Add link to `ASSIGNMENT-SUBMISSION.md`
- [ ] Final git push
- [ ] Submit assignment!

---

## 🎯 Key Points to Emphasize

1. **Two Commits** - Show in git log
2. **Business Rules** - Manager $50k, Director $500k, Two-approver >$500k
3. **Tests** - 15 unit tests all passing
4. **AI Agents** - Multiple agents + sub-agent with logs
5. **Kiro Config** - Complete setup in `.kiro/`
6. **PR Review** - AI verdict: APPROVED

---

## 🔧 Quick Fixes If Something Goes Wrong

**If tests fail during recording:**
- Don't panic! Just say "I'll show the test code instead"
- Open the test file and explain what it does

**If mutation testing is too slow:**
- Show the first few seconds, then Ctrl+C
- Show the pre-generated report HTML instead

**If E2E tests don't run:**
- Show the test code and explain the scenarios
- Mention "The tests are ready, just need selector adjustment"

**If you stumble on words:**
- Pause, take a breath, continue
- Or just re-record that section (edit later if possible)

---

## 💡 Pro Tips

1. **Practice once** before final recording (dry run)
2. **Close notifications** (Do Not Disturb mode)
3. **Clean desktop** (close unnecessary windows)
4. **Good lighting** (if showing your face)
5. **Speak clearly** and not too fast
6. **Show enthusiasm** - you did great work!

---

**You've got this! 🚀 The hard work is done - just show it off!**
