# Incident packet — dữ liệu giả lập để phân tích

Dịch vụ .NET 8 có 6 replicas trên Azure App Service. Credential HMAC ký request gửi đến gateway biển báo. Key Vault lưu secret `signing-key`; mỗi replica đọc secret khi startup và cache trong singleton. Gateway chấp nhận đúng một key active theo cấu hình, deploy độc lập.

| Thời điểm (+07) | Dấu hiệu |
|---|---|
| 01:55 | 6/6 replicas trả 2xx, p95 140 ms |
| 02:00 | Key Vault có version mới `v18`; `v17` chưa bị disable |
| 02:02 | Gateway bắt đầu kiểm tra chữ ký bằng `v18` |
| 02:03 | 4/6 replicas gửi request 401; 2/6 vẫn 2xx |
| 02:08 | Restart 2 replicas lỗi; 2 replicas đó chuyển sang 2xx |
| 02:12 | 2 replicas lỗi còn lại vẫn 401; 2 replicas chưa restart vẫn 2xx |
| 02:25 | Tỷ lệ 401 ~31% theo log tổng hợp, không có tăng 429 từ Key Vault |

## Telemetry (giả lập)
```text
instance=A startup=01:10 keyVersion=v17 outbound=401
instance=B startup=01:12 keyVersion=v17 outbound=401
instance=C startup=02:05 keyVersion=v18 outbound=200
instance=D startup=02:06 keyVersion=v18 outbound=200
instance=E startup=01:09 keyVersion=v17 outbound=401
instance=F startup=01:11 keyVersion=v17 outbound=401
gateway activeKey=v18 acceptedVersions=[v18]
keyvault secret signing-key enabledVersions=[v17,v18]
```

**Lưu ý:** Timeline và bảng log là dữ liệu mô phỏng có chủ đích, không phải telemetry thực tế. Dòng tóm tắt 02:12 có thể mâu thuẫn với snapshot bên dưới; ghi rõ mức độ tin cậy, không tự sửa bằng suy đoán.

## Constraints
- Không được lưu secret plaintext vào log hoặc repository.
- Mục tiêu: availability ≥99.95%; lỗi xác thực trong rotation dưới 0.1% requests.
- Gateway và client deploy bởi hai nhóm, không có distributed transaction.
- Key Vault truy cập qua Managed Identity; không được cấp quyền rộng hơn Get cho client.
- Security yêu cầu rotation mỗi 30 ngày, emergency revoke trong 10 phút.
- Budget cho giai đoạn overlap tối đa 20 phút; không thể restart đồng loạt.
- 6 replicas, ~250 requests/giây, mỗi request phải được ký đúng phiên bản.

## Nhiệm vụ phân tích
1. Nêu 3 giả thuyết, bằng chứng ủng hộ/phản bác và cách thu thêm evidence.
2. Chỉ ra những điểm mâu thuẫn hoặc thiếu dữ liệu.
3. Đề xuất protocol rotation có trạng thái, guardrail, rollback và revoke khẩn cấp.
4. Viết test matrix cho mixed-version replicas, gateway failover, Key Vault unavailable, clock skew và rollback.
