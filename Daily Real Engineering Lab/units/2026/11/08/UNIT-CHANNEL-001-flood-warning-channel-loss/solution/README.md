# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms / Evidence
12 sự kiện được ghi, chỉ 3 sự kiện cuối được đọc; không có exception. Reproduction luôn xảy ra vì producer hoàn tất trước khi consumer bắt đầu.

## Root cause
`BoundedChannelFullMode.DropOldest` chấp nhận write khi buffer đầy nhưng loại bỏ phần tử cũ. `WriteAsync` thành công không đảm bảo event đã được xử lý. Chính sách này phù hợp telemetry có thể mất, không phù hợp cảnh báo an toàn.

## Why the fix works
Chuyển sang `BoundedChannelFullMode.Wait` và chạy consumer đồng thời với producer; producer đợi chỗ trống khi buffer đầy. Hoàn tất writer rồi đợi consumer drain, giữ thứ tự và số lượng.

## Verify
`./verify.ps1` chạy trên `starter/` sau khi bạn sửa. Có thể đối chiếu `dotnet run --project solution/ChannelIncident.csproj -- --verify`.

## Alternatives / Trade-offs
Channel unbounded tránh block nhưng có rủi ro tăng bộ nhớ vô hạn khi downstream chậm. Durable broker/outbox phù hợp khi cần chống mất dữ liệu sau process crash; in-memory Channel chỉ đảm bảo trong lifetime process. Chính sách drop có thể đúng với dashboard sampling, không đúng với cảnh báo bắt buộc.

## Wrong / Tempting Fixes
Tăng capacity từ 3 lên 100 chỉ trì hoãn mất dữ liệu; giảm tốc producer bằng `Task.Delay` che triệu chứng và phụ thuộc timing; thêm retry quanh `WriteAsync` không giúp khi write trả success mà dữ liệu đã bị loại.

## Production implications
Đo queue depth, thời gian chờ write, throughput, end-to-end acknowledgments và số lượng drop. Thiết kế shutdown drain, cancellation, bounded latency, durable persistence và alert priority nếu SLA yêu cầu.

## Senior engineer should notice
Phải định nghĩa rõ contract giữa accepted, queued, delivered và acknowledged; một API trả success không phải bằng chứng delivery. Kiểm tra failure khi consumer fault, cancellation và process restart.
