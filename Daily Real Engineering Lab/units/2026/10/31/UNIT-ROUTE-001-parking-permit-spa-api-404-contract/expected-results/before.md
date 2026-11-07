# Trước khi sửa
- `/api/permits/42`: 200, `application/json`, có `permitId=42`.
- `/renewals`: 200, `text/html`, chứa SPA shell.
- `/api/permits/not-a-number`, `/api/unknown`, `/api/permits/42/details`: **đều 200 HTML**, không phải JSON 404.
- `/assets/site.css`: 404 vì URL static asset không khớp fallback không-file.
- `reproduce.ps1` trả 0 khi xác nhận đủ ba API contract failures và ba control cases đúng.
- `verify.ps1` trả mã khác 0 trên starter chưa sửa.

Nếu không reproduce: kiểm tra SDK 8, lệnh setup và xác nhận bạn chưa chỉnh starter.
