# Scenario & independent acceptance contract

Factory maintenance API triển khai audit policy mới, chỉ áp dụng với đường dẫn bắt đầu bằng `/api`. Contract độc lập với implementation:

| Request | Status | Body | X-Audit-Policy |
|---|---:|---|---|
| /health | 200 | healthy | absent |
| /api/v1/maintenance/active | 200 | active:3 | audit-v1 |
| /api/v1/maintenance/history | 200 | history:2 | audit-v1 |
| /missing | 404 | empty | absent |

Không được xóa audit policy để API pass; không gắn audit header cho health/missing; không sửa test harness để hard-code pass. Reproduction chạy pipeline thật trong bộ nhớ, không cần HTTP server.
