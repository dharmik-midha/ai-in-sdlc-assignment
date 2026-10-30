import { test, expect } from '@playwright/test';
import { LoginPage } from '../pages/LoginPage';
import { InvoicesPage } from '../pages/InvoicesPage';

/**
 * E2E tests for role-based approval limits
 * 
 * Assignment Requirements:
 * - Manager: $50,000 limit
 * - Director: $500,000 limit  
 * - >$500,000: Requires two distinct approvers, second must be Director or Admin
 * - Same user cannot approve twice
 */

test.describe('Role-Based Approval Limits - E2E', () => {
  let loginPage: LoginPage;
  let invoicesPage: InvoicesPage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    invoicesPage = new InvoicesPage(page);
  });

  test('Manager can approve invoice up to $50,000', async ({ page }) => {
    // Login as Manager
    await loginPage.goto();
    await loginPage.login('alice.manager', 'Manager');
    
    // Take screenshot after login
    await page.screenshot({ 
      path: 'e2e/test-results/01-manager-login.png', 
      fullPage: true 
    });

    // Create invoice for $45,000
    const invoiceId = await invoicesPage.createInvoice('ACME Corp', 45000);
    
    // Take screenshot of created invoice
    await page.screenshot({ 
      path: 'e2e/test-results/02-manager-invoice-45k.png', 
      fullPage: true 
    });

    // Approve invoice
    await invoicesPage.approveInvoice(invoiceId);
    
    // Verify status
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Approved');
    
    // Take screenshot of approval
    await page.screenshot({ 
      path: 'e2e/test-results/03-manager-approved-45k.png', 
      fullPage: true 
    });
  });

  test('Manager cannot approve invoice over $50,000', async ({ page }) => {
    // Login as Manager
    await loginPage.goto();
    await loginPage.login('bob.manager', 'Manager');

    // Create invoice for $60,000
    const invoiceId = await invoicesPage.createInvoice('XYZ Ltd', 60000);
    
    // Take screenshot before attempting approval
    await page.screenshot({ 
      path: 'e2e/test-results/04-manager-invoice-60k.png', 
      fullPage: true 
    });

    // Try to approve - should fail
    await invoicesPage.approveInvoice(invoiceId);
    
    // Expect error message
    await invoicesPage.expectErrorMessage('Manager role can only approve invoices up to $50,000');
    
    // Verify status remains Pending
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Pending');
    
    // Take screenshot of error
    await page.screenshot({ 
      path: 'e2e/test-results/05-manager-rejected-60k-error.png', 
      fullPage: true 
    });
  });

  test('Director can approve invoice up to $500,000', async ({ page }) => {
    // Login as Director
    await loginPage.goto();
    await loginPage.login('carol.director', 'Director');

    // Create invoice for $250,000
    const invoiceId = await invoicesPage.createInvoice('BigCo Inc', 250000);
    
    await page.screenshot({ 
      path: 'e2e/test-results/06-director-invoice-250k.png', 
      fullPage: true 
    });

    // Approve invoice
    await invoicesPage.approveInvoice(invoiceId);
    
    // Verify approved
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Approved');
    
    await page.screenshot({ 
      path: 'e2e/test-results/07-director-approved-250k.png', 
      fullPage: true 
    });
  });

  test('Director can approve exactly $500,000', async ({ page }) => {
    await loginPage.goto();
    await loginPage.login('dave.director', 'Director');

    const invoiceId = await invoicesPage.createInvoice('MaxCorp', 500000);
    await invoicesPage.approveInvoice(invoiceId);
    
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Approved');
    
    await page.screenshot({ 
      path: 'e2e/test-results/08-director-approved-500k.png', 
      fullPage: true 
    });
  });

  test('Two-approver flow: Manager then Director for $750k invoice', async ({ page }) => {
    // Step 1: Manager creates and gives first approval
    await loginPage.goto();
    await loginPage.login('alice.manager', 'Manager');
    
    const invoiceId = await invoicesPage.createInvoice('MegaCorp', 750000);
    
    await page.screenshot({ 
      path: 'e2e/test-results/09-two-approver-invoice-750k.png', 
      fullPage: true 
    });
    
    // First approval by Manager
    await invoicesPage.approveInvoice(invoiceId);
    
    // Verify still pending (needs second approval)
    let status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Pending');
    
    await page.screenshot({ 
      path: 'e2e/test-results/10-first-approval-manager.png', 
      fullPage: true 
    });
    
    // Step 2: Logout and login as Director
    await invoicesPage.logout();
    await loginPage.login('carol.director', 'Director');
    
    // Second approval by Director
    await invoicesPage.approveInvoice(invoiceId);
    
    // Verify fully approved
    status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Approved');
    
    await page.screenshot({ 
      path: 'e2e/test-results/11-second-approval-director-approved.png', 
      fullPage: true 
    });
  });

  test('Two-approver flow: Director then Director for $600k invoice', async ({ page }) => {
    // First Director approval
    await loginPage.goto();
    await loginPage.login('dave.director', 'Director');
    
    const invoiceId = await invoicesPage.createInvoice('SuperCorp', 600000);
    await invoicesPage.approveInvoice(invoiceId);
    
    let status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Pending');
    
    await page.screenshot({ 
      path: 'e2e/test-results/12-first-director-approval-600k.png', 
      fullPage: true 
    });
    
    // Second Director approval
    await invoicesPage.logout();
    await loginPage.login('eve.director', 'Director');
    
    await invoicesPage.approveInvoice(invoiceId);
    
    status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Approved');
    
    await page.screenshot({ 
      path: 'e2e/test-results/13-second-director-approval-approved.png', 
      fullPage: true 
    });
  });

  test('Same user cannot approve invoice twice', async ({ page }) => {
    // First approval
    await loginPage.goto();
    await loginPage.login('dave.director', 'Director');
    
    const invoiceId = await invoicesPage.createInvoice('DuplicateCorp', 800000);
    await invoicesPage.approveInvoice(invoiceId);
    
    await page.screenshot({ 
      path: 'e2e/test-results/14-same-user-first-approval.png', 
      fullPage: true 
    });
    
    // Try second approval with same user
    await invoicesPage.approveInvoice(invoiceId);
    
    // Expect error
    await invoicesPage.expectErrorMessage('Same user cannot approve an invoice twice');
    
    // Status should remain Pending
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Pending');
    
    await page.screenshot({ 
      path: 'e2e/test-results/15-same-user-error.png', 
      fullPage: true 
    });
  });

  test('Second approver must be Director or Admin, not Manager', async ({ page }) => {
    // First approval by Manager
    await loginPage.goto();
    await loginPage.login('alice.manager', 'Manager');
    
    const invoiceId = await invoicesPage.createInvoice('RuleCorp', 650000);
    await invoicesPage.approveInvoice(invoiceId);
    
    await page.screenshot({ 
      path: 'e2e/test-results/16-manager-first-approval-650k.png', 
      fullPage: true 
    });
    
    // Try second approval with another Manager
    await invoicesPage.logout();
    await loginPage.login('bob.manager', 'Manager');
    
    await invoicesPage.approveInvoice(invoiceId);
    
    // Expect error
    await invoicesPage.expectErrorMessage('Second approver must be Director or Admin');
    
    // Status should remain Pending
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Pending');
    
    await page.screenshot({ 
      path: 'e2e/test-results/17-second-manager-error.png', 
      fullPage: true 
    });
  });

  test('Admin can approve any amount with single approval', async ({ page }) => {
    await loginPage.goto();
    await loginPage.login('frank.admin', 'Admin');
    
    // Create high-value invoice
    const invoiceId = await invoicesPage.createInvoice('UnlimitedCorp', 1500000);
    
    await page.screenshot({ 
      path: 'e2e/test-results/18-admin-invoice-1.5m.png', 
      fullPage: true 
    });
    
    // Single approval
    await invoicesPage.approveInvoice(invoiceId);
    
    // Verify approved immediately
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Approved');
    
    await page.screenshot({ 
      path: 'e2e/test-results/19-admin-approved-1.5m-single.png', 
      fullPage: true 
    });
  });

  test('Rejection workflow preserves first approver audit trail', async ({ page }) => {
    // First approval
    await loginPage.goto();
    await loginPage.login('alice.manager', 'Manager');
    
    const invoiceId = await invoicesPage.createInvoice('RejectCorp', 700000);
    await invoicesPage.approveInvoice(invoiceId);
    
    await page.screenshot({ 
      path: 'e2e/test-results/20-reject-first-approval.png', 
      fullPage: true 
    });
    
    // Reject by Director
    await invoicesPage.logout();
    await loginPage.login('carol.director', 'Director');
    
    await invoicesPage.rejectInvoice(invoiceId, 'Budget constraints for Q4');
    
    // Verify rejected
    const status = await invoicesPage.getInvoiceStatus(invoiceId);
    expect(status).toContain('Rejected');
    
    // Check that first approver info is still visible (audit trail)
    const details = await invoicesPage.getInvoiceDetails(invoiceId);
    expect(details.decidedBy).toContain('carol.director');
    
    await page.screenshot({ 
      path: 'e2e/test-results/21-rejected-with-audit-trail.png', 
      fullPage: true 
    });
  });
});
