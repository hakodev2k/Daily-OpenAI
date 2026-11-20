# Evidence — giả lập sự cố (không phải telemetry thực tế)

Topology dự kiến: Client → CDN → ingress → ASP.NET Core (2 replicas). Sau migration, một môi trường cho phép đi trực tiếp tới ingress trong mạng nội bộ. CDN có thể thêm thông tin client address; ingress có thể append header. Các địa chỉ thuộc dải RFC 5737 dùng làm ví dụ.

| Trace | Peer tới ingress | Peer tới app | X-Forwarded-For nhận ở app | IP trong audit | Ghi chú |
|---|---|---|---|---|---|
| A | 198.51.100.10 (CDN) | 10.20.0.12 | 203.0.113.17, 198.51.100.10 | 203.0.113.17 | Đường CDN chuẩn |
| B | 10.30.0.44 (internal direct) | 10.20.0.12 | 192.0.2.88 | 192.0.2.88 | Caller nội bộ tự gửi header |
| C | 198.51.100.10 (CDN) | 10.20.0.13 | 192.0.2.99, 203.0.113.20, 198.51.100.10 | 192.0.2.99 | Caller gửi giá trị tiền tố |

Thông tin bổ sung: audit log không lưu trusted-hop count, peer socket address hay config version. Rate limiter dùng cùng giá trị IP với audit. Deployment gần đây thay ingress replica, nhưng không có bằng chứng tất cả replica dùng cấu hình giống nhau.

## Các ràng buộc
- IP dùng để audit và rate limiting; không dùng làm bằng chứng danh tính hoặc authorization duy nhất.
- Hệ thống phải hỗ trợ đường CDN chuẩn; đường trực tiếp không được tự khai IP ngoài trust boundary.
- Không thể tắt hệ thống quá 5 phút; cần rollout theo từng replica và rollback.
- IP client thật có thể qua NAT; không được giả định mỗi người dùng có IP riêng.

## Nhiệm vụ tái hiện trên giấy
Với A/B/C: vẽ chain, phân loại hop nào được xác thực bởi network boundary, chỉ rõ trường hợp thiếu dữ liệu để kết luận. Đừng kết luận rằng giá trị đầu tiên của header luôn đúng.