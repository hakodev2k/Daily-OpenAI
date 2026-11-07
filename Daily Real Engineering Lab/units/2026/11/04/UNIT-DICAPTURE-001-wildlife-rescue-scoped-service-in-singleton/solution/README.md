# Reference Solution — chỉ xem sau khi tự sửa

## Symptoms và evidence
Hai thông báo có animal khác nhau nhưng cùng recipient shift-A. Worker singleton được resolve từ root provider.

## Root cause
`NotificationWorker` singleton nhận `ShiftContext` scoped trực tiếp. Khi `ValidateScopes=false`, scoped instance được resolve từ root scope và bị giữ lại cùng worker. `Initialize` chỉ gán lần đầu nên shift-B tiếp tục dùng shift-A.

## Vì sao fix hoạt động
Inject `IServiceScopeFactory` (singleton-safe), tạo scope riêng mỗi lần Send, resolve ShiftContext trong scope đó, rồi dispose scope. Bật `ValidateScopes=true` để phát hiện captive dependency trong cấu hình lỗi.

## Verify
Chạy `pwsh ./verify.ps1` sau khi sửa `starter/Program.cs`; output phải lần lượt là `recipient=shift-A;animal=owl` và `recipient=shift-B;animal=fox`. Có thể chạy riêng `dotnet run --project solution/Lab.csproj` để so sánh.

## Alternatives
Đưa dữ liệu ca trực thành tham số thuần và giữ worker stateless là lựa chọn đơn giản hơn khi không có dependency scoped thực sự. Nếu công việc cần DbContext hoặc tenant service scoped, scope-per-message là phù hợp.

## Wrong / tempting fixes
Đổi ShiftContext thành singleton chỉ che triệu chứng và tăng chia sẻ state. Tắt ValidateScopes không sửa lifetime. Tạo scope trong constructor singleton vẫn giữ một scope cho mọi lần Send. Dùng static dictionary theo shift cần thiết kế eviction và ownership, không giải quyết lỗi lifetime gốc.

## Production implications
Với BackgroundService, tạo scope cho mỗi message hoặc batch có ranh giới rõ; dispose kể cả khi xử lý lỗi. Nếu xử lý bất đồng bộ, dùng CreateAsyncScope khi dependency hỗ trợ IAsyncDisposable. Không giữ scoped service sau khi scope đã dispose. Log correlation ID và shift ID, tránh ghi dữ liệu cá nhân.

## Trade-offs / Senior engineer
Scope-per-message tăng chi phí tạo scope nhỏ nhưng bảo đảm isolation. Scope-per-batch có thể hiệu quả hơn nếu batch thật sự chia sẻ transaction/context; cần chứng minh ranh giới nghiệp vụ. Bật scope validation trong development/CI; kiểm thử hai ca liên tiếp thay vì chỉ một ca.
