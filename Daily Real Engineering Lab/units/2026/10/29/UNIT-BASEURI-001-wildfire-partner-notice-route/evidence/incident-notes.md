# Incident evidence (synthetic)
- 09:18: Smoke test tới https://dispatch.partner.invalid/v3/notices?district=NW-4 trả 200.
- 09:21: Adapter BaseUrl https://dispatch.partner.invalid/v3/ trả 404 route-not-found.
- 09:23: Backup BaseUrl https://backup.partner.invalid/integrations/v3/ cũng 404.
- 09:25: Legacy BaseUrl https://legacy.partner.invalid/ trả 200.
- 09:26: DNS/TLS telemetry không có lỗi handshake hoặc timeout; CPU bình thường.
- 09:27: SDK version và district query không thay đổi.
Đây là dữ liệu giả lập. Domain .invalid không được truy cập qua mạng; chạy run.ps1 để thấy URI từ starter.
