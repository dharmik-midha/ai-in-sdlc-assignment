import { Page, Locator, expect } from '@playwright/test';

export class InvoicesPage {
  readonly page: Page;
  readonly createButton: Locator;
  readonly customerNameInput: Locator;
  readonly amountInput: Locator;
  readonly submitButton: Locator;
  readonly invoicesTable: Locator;

  constructor(page: Page) {
    this.page = page;
    this.createButton = page.locator('button:has-text("Create Invoice")');
    this.customerNameInput = page.locator('input[name="customerName"]');
    this.amountInput = page.locator('input[name="amount"]');
    this.submitButton = page.locator('button:has-text("Submit for Approval")');
    this.invoicesTable = page.locator('table.invoices');
  }

  async goto() {
    await this.page.goto('/invoices');
  }

  async createInvoice(customerName: string, amount: number): Promise<string> {
    await this.createButton.click();
    await this.customerNameInput.fill(customerName);
    await this.amountInput.fill(amount.toString());
    await this.submitButton.click();
    
    // Wait for success message or new invoice to appear
    await this.page.waitForTimeout(500);
    
    // Get the invoice ID from the table (assumes most recent is at top)
    const invoiceId = await this.page.locator('table.invoices tbody tr:first-child td:first-child').innerText();
    return invoiceId;
  }

  async getInvoiceRow(invoiceId: string): Promise<Locator> {
    return this.page.locator(`tr:has-text("${invoiceId}")`);
  }

  async approveInvoice(invoiceId: string, screenshot?: string) {
    const row = await this.getInvoiceRow(invoiceId);
    
    if (screenshot) {
      await this.page.screenshot({ path: screenshot, fullPage: true });
    }
    
    await row.locator('button:has-text("Approve")').click();
    await this.page.waitForTimeout(500);
  }

  async rejectInvoice(invoiceId: string, reason: string) {
    const row = await this.getInvoiceRow(invoiceId);
    await row.locator('button:has-text("Reject")').click();
    
    // Fill rejection reason in modal/input
    await this.page.locator('textarea[name="reason"], input[name="reason"]').fill(reason);
    await this.page.locator('button:has-text("Confirm Reject")').click();
    await this.page.waitForTimeout(500);
  }

  async getInvoiceStatus(invoiceId: string): Promise<string> {
    const row = await this.getInvoiceRow(invoiceId);
    return await row.locator('td.status, .badge').innerText();
  }
  
  async getInvoiceDetails(invoiceId: string): Promise<{
    customer: string;
    amount: string;
    status: string;
    decidedBy?: string;
  }> {
    const row = await this.getInvoiceRow(invoiceId);
    const cells = await row.locator('td').allInnerTexts();
    
    return {
      customer: cells[1],
      amount: cells[2],
      status: cells[3],
      decidedBy: cells[4] || undefined,
    };
  }

  async expectErrorMessage(message: string) {
    await expect(this.page.locator('.error, .alert-danger, [role="alert"]')).toContainText(message);
  }
  
  async logout() {
    await this.page.locator('button:has-text("Logout"), a:has-text("Logout")').click();
    await this.page.waitForURL('**/');
  }
}
