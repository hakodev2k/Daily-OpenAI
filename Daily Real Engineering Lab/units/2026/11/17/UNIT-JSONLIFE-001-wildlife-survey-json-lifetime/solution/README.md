# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Ingest in `starter` báo thành công; worker đọc `JsonElement` sau đó ném `ObjectDisposedException`.

## Evidence
`INGEST_ACCEPTED` đã được in. Exception phát sinh khi gọi `GetProperty`, không phải ở `JsonDocument.Parse`. Payload là hằng số, nên không phải lỗi dữ liệu đầu vào.

## Root cause
`JsonElement` từ `JsonDocument.RootElement` tham chiếu storage thuộc sở hữu của `JsonDocument`. `using` dispose document trước khi worker dùng element.

## Why fix works
Trả về `root.Clone()` trong scope còn sống tạo một `JsonElement` độc lập với document nguồn. Xem `solution/Program.cs`; áp dụng tương đương vào `starter/Program.cs`.

## How to verify
`./verify.ps1` chạy starter, yêu cầu `INGEST_ACCEPTED` và `WORKER_OK CAM-42 pangolin`. Chạy `./reproduce.ps1` trước khi sửa.

## Alternative fixes
Chuyển thành immutable DTO ngay trong ingest, hoặc chuyển ownership của `JsonDocument` sang consumer với quy tắc dispose rõ ràng. DTO có lợi khi schema ổn định; giữ document đòi hỏi quản lý lifetime chặt chẽ.

## Wrong / tempting fixes
Nuốt `ObjectDisposedException` làm mất event. Bỏ `using` không có ownership policy có thể giữ tài nguyên quá lâu. Retry với cùng element không làm document sống lại. Thêm `Task.Delay` không giải quyết lifetime.

## Production implications
Hàng đợi nội bộ phải sở hữu payload đủ lâu; audit dữ liệu không được dựa vào đối tượng đã dispose. Đo allocation trước khi chọn clone hoặc DTO.

## Trade-offs
Clone cấp phát bộ nhớ tương ứng payload; DTO giảm coupling JSON nhưng tăng mapping; giữ document giảm copy nhưng tăng complexity và nguy cơ leak.

## Senior engineer should notice
Resource ownership phải được xác định ở boundary, không phụ thuộc vào việc worker chạy nhanh hay chậm. Regression test cần chạy consumer sau khi producer scope kết thúc.

## References
- https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom
- https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.clone
