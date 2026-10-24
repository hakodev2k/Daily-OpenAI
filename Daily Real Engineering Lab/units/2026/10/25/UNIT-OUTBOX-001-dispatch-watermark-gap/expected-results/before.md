# Expected Results — Starter

`setup.ps1` build thành công; `reproduce.ps1` in `INCIDENT_REPRODUCED` và trả exit 0. Trong dữ liệu deterministic: publish attempt có 101, 102, 103; event 102 bị từ chối một lần và không được gửi lại; downstream nhận 101 và 103; checkpoint đạt 103. `verify.ps1` phải trả exit 1 và có ít nhất một `FAIL`.

Nếu chưa reproduce: kiểm tra .NET 8 SDK, chạy từ unit root, đảm bảo chưa chỉnh `starter/OutboxPipeline.cs`, và xác nhận `--reproduce` đã dùng one-shot failure injection.
