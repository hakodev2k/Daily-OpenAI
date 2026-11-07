# Hint 3

Thử `await expect(page.locator('#status')).toHaveText('Published')` và bỏ fixed `waitForTimeout` cùng one-shot `textContent()` assertion.
