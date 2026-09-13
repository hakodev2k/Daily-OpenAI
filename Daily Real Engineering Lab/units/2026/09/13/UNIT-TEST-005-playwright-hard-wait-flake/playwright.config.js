const { defineConfig } = require('@playwright/test');
module.exports = defineConfig({
  testDir: './starter',
  timeout: 10000,
  use: { baseURL: 'http://127.0.0.1:4173', headless: true },
  webServer: { command: 'node starter/server.js', port: 4173, reuseExistingServer: false }
});
