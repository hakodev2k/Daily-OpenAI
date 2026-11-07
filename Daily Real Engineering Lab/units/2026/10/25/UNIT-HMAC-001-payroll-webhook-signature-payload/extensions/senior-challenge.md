# Senior extension — production webhook trust boundary

Sau khi verify pass, đề xuất thiết kế ASP.NET Core endpoint thật (không cần triển khai cho bài lab chính) với các yêu cầu:

1. Giới hạn kích thước raw request body, xử lý cancellation và content type.
2. Provider gửi header gồm key ID, timestamp và signature; có giai đoạn xoay vòng hai khóa.
3. Ngăn replay trong cửa sổ 5 phút và xử lý duplicate event ID idempotently.
4. Không lưu hoặc log raw payroll payload trong telemetry.
5. Mô tả contract tests cho đổi key, signature sai, clock skew, duplicate, payload lớn và retry.

Viết một ADR ngắn: chọn nơi kiểm tra signature, nơi parse JSON, cơ chế idempotency và failure response; giải thích trade-off khi hệ thống chạy nhiều instance. Không sử dụng secret production trong lab.
