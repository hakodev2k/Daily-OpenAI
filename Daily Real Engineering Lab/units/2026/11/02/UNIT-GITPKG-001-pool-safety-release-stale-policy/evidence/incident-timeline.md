# Incident evidence (synthetic)
- 08:00 — Release revision phê duyệt `active.json`, xóa policy cũ.
- 08:05 — Runner đã chạy một job khác và có file JSON tồn dư trong working tree.
- 08:08 — Packaging job hoàn tất, không báo lỗi.
- 08:12 — Smoke test chỉ xác nhận `active.json` có trong artifact: PASS.
- 08:30 — QA phát hiện policy cũ vẫn xuất hiện trong package.

## Investigation tasks
Thu thập `git log --oneline -2`, `git status --short`, `git ls-files -- release/*.json`, `artifact/manifest.json` và `artifact/package/`. Xác định bước nào làm artifact khác commit. Timeline là giả lập, không phải log production thật.
