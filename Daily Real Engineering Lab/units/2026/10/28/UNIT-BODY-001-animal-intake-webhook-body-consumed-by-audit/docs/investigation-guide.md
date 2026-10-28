# Investigation guide
1. Chạy `./reproduce.ps1`, ghi status, auditBytes, endpointBytes và expectedBytes.
2. So sánh thứ tự `RequestAuditMiddleware.InvokeAsync` và `RequestDelegate endpoint`.
3. Kiểm tra `CanSeek`, lifecycle và trạng thái của request body giữa các bước.
4. Đưa ra hai giả thuyết cạnh tranh, tìm evidence loại trừ từng giả thuyết.
5. Chỉ sửa file middleware trong starter; giữ nguyên JSON fixture và endpoint.
6. Chạy `./verify.ps1`; sau đó cân nhắc body lớn, cancellation và thông tin nhạy cảm.
