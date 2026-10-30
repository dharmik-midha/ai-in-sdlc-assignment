import { Page, Locator } from '@playwright/test';

export class LoginPage {
  readonly page: Page;
  readonly usernameInput: Locator;
  readonly roleSelect: Locator;
  readonly loginButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.usernameInput = page.locator('input[placeholder="Enter username"]');
    this.roleSelect = page.locator('select#role');
    this.loginButton = page.locator('button:has-text("Login")');
  }

  async goto() {
    await this.page.goto('/');
  }

  async login(username: string, role: 'Manager' | 'Director' | 'Admin') {
    await this.usernameInput.fill(username);
    await this.roleSelect.selectOption(role);
    await this.loginButton.click();
    
    // Wait for navigation to invoices page
    await this.page.waitForURL('**/invoices');
  }
  
  async getCurrentUser(): Promise<string> {
    return await this.page.locator('.current-user').innerText();
  }
}
