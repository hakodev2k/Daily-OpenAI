const { test, expect } = require('@playwright/test');

test('publish product shows final status', async ({ page }) => {
  await page.goto('/');
  await page.getByRole('button', { name: 'Publish' }).click();

  // Investigation note: test đang quyết định lúc nào UI "đã sẵn sàng" bằng cách nào?
  await page.waitForTimeout(300);

  const status = await page.locator('#status').textContent();
  expect(status).toBe('Published');
});
