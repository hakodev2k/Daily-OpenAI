# Reference solution — Spoiler
## Root cause
`LabelService` giữ `PrinterConfig` được đọc từ `PrinterRegistry.Current` ngay trong constructor. `Update` thay thế snapshot trong registry, nhưng service tiếp tục dùng snapshot cũ.

## Fix
Giữ tham chiếu registry và lấy `Current` tại đầu mỗi lần `Render`. Dùng một local immutable snapshot để hai field trên cùng nhãn đến từ một version cấu hình.

## Kiểm chứng
Chép nội dung `solution/Program.cs` vào `starter/Program.cs`, chạy `pwsh ./scripts/verify.ps1`, sau đó khôi phục starter để tự thử. Không sửa scripts để ép pass.

## Wrong fixes
- Khởi tạo lại toàn bộ application sau mỗi thay đổi: che lỗi vòng đời và gây downtime.
- Chỉ cập nhật prefix trong service: printerId có thể lệch version.
- Đọc từng field cấu hình ở các thời điểm khác nhau: dễ tạo nhãn hỗn hợp nếu cập nhật đồng thời.

## Trade-offs
Bản demo là một process và cập nhật tuần tự. Production cần cơ chế publication thread-safe (ví dụ immutable snapshot + `Volatile.Read/Write`), nguồn config reload đáng tin cậy, logging version và policy xử lý lệnh in đang chạy. Với ASP.NET Core Options, so sánh `IOptions`, `IOptionsSnapshot`, `IOptionsMonitor` theo lifetime và semantics.

