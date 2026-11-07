# Concepts after lab — UTF-8 streaming và contract của transport

- UTF-8 dùng từ 1 đến 4 byte cho mỗi Unicode scalar value; ranh giới chunk không phải ranh giới ký tự.
- `Encoding.UTF8.GetString` trên từng đoạn độc lập không giữ trạng thái của chuỗi byte chưa hoàn chỉnh.
- `Decoder` có thể giữ các byte chưa đủ qua nhiều lần gọi; chỉ flush khi thông điệp kết thúc.
- Replacement fallback giúp hiển thị dữ liệu lỗi nhưng không phù hợp với hợp đồng ingestion yêu cầu integrity; exception fallback cho phép fail closed.
- `ReadOnlyMemory<byte>` giúp chuyển fragment không cần copy; lifetime của buffer phải được bảo đảm cho tới khi decode xong.
- Đối với payload rất lớn, nên decode theo stream vào writer có giới hạn kích thước, thay vì gom toàn bộ message trong RAM.
