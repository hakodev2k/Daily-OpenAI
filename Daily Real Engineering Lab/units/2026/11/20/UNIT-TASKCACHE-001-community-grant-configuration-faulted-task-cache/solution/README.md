# Reference Solution — UNIT-TASKCACHE-001

## Root cause
`_cached ??= LoadAsync()` lưu một `Task<string>`, không chỉ giá trị thành công. Task đã fault vẫn khác null, nên các lượt sau nhận lại đúng exception cũ mà không gọi backend.

## Fix
Chỉ tái sử dụng task đang chạy hoặc đã hoàn thành thành công; thay task faulted/canceled dưới một lock để tránh nhiều caller cùng tạo request mới. Code tại [Program.cs](Program.cs). Đây là chính sách retry-on-next-call, không phải retry vô hạn trong cùng request.

## Verify
Chạy `./scripts/verify.ps1` để kiểm chứng **starter do bạn sửa**. Đối chiếu `dotnet run --project solution/GrantPolicyLab.csproj` khi cần.

## Wrong fixes
- `catch { return "grant-policy:v2"; }`: che lỗi và bịa chính sách khi endpoint không phản hồi.
- `return LoadAsync();` mọi lần: bỏ caching, tăng tải upstream và tạo thundering herd.
- `_cached = null` không đồng bộ từ nhiều thread: có thể gây nhiều lần fetch đồng thời và race.

## Trade-offs
Nếu policy thay đổi theo thời gian, cần TTL/version/invalidation độc lập. Một lock chỉ bảo vệ việc thay thế task trong một process; distributed cache đòi hỏi hợp đồng riêng. Đo số request upstream, fault rate, thời gian hồi phục và số request cùng lúc trước khi thay đổi kiến trúc.
