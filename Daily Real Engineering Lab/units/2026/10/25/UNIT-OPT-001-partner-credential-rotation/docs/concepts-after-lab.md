# Khái niệm sau lab

- `IOptions<T>`: một giá trị options cho consumer, không phải hợp đồng live-refresh.
- `IOptionsSnapshot<T>`: scoped, phù hợp request-scoped consumer, không đưa vào singleton.
- `IOptionsMonitor<T>`: singleton-compatible, có `CurrentValue` và `OnChange`; phụ thuộc configuration provider phát reload token.
- `IConfigurationRoot.Reload()` trong harness dùng để mô phỏng change notification có kiểm soát. Đây không phải bằng chứng mọi secret provider ngoài production sẽ tự cập nhật.
- Với outbound request, cố định một version cho **mỗi operation**, không đọc nhiều version giữa chừng trong cùng một request.

Tham khảo chính thức: Microsoft Learn — Options pattern in .NET:
https://learn.microsoft.com/en-us/dotnet/core/extensions/options

Câu hỏi mở rộng: nếu credential mới chưa được tất cả partner edge node chấp nhận, bạn sẽ thiết kế overlap window, health check và rollback như thế nào mà không log secret hoặc retry non-idempotent request vô hạn?
