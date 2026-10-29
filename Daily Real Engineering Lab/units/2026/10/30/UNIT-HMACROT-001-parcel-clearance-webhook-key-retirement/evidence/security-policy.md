# Security policy — synthetic

- `K1` **không được dùng để verify** webhook kể từ **2026-10-30 09:30:00 +07**. Đây là hard cutoff, không phải cảnh báo.
- K2 chỉ được kích hoạt sau khi phân phối đến mọi replica; config drift là NO-GO.
- Timestamp mới gửi qua webhook: future skew tối đa 5 phút; event quá 24h phải bị từ chối và được điều tra qua kênh riêng, không tự động bypass.
- Không coi HTTP 200, eventId hay sự hiện diện của một bản ghi inbox là bằng chứng xác thực payload mới.
- Dùng constant-time signature comparison; ký trên raw bytes đúng hợp đồng. Không log raw signature, secret hay payload nhạy cảm.
- Một `partnerId + eventId` chỉ có một canonical body digest. Khác digest là security anomaly, không ghi đè.
- Feed reconciliation chỉ được tin sau khi xác thực nguồn qua TLS/OAuth, kiểm tra tenant scope và kiểm chứng tính đầy đủ. Webhook bị từ chối không tự động chuyển thành trusted feed event.
- Trong emergency compromise, Security có thể yêu cầu revoke ngay lập tức; điều này có thể ảnh hưởng availability và cần quy trình exception/incident riêng.
