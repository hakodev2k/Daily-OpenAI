# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Khi tenant A chờ trước khi gửi, tenant B cập nhật cấu hình request, request A mang token B.

## Evidence
Handler ghi `tenant-a actual=token-b` và `tenant-b actual=token-b`. Scheduler sử dụng TaskCompletionSource để cố định thứ tự.

## Root cause
`HttpClient.DefaultRequestHeaders` là cấu hình chung của client; thay đổi header xác thực theo từng request trong lúc các thao tác overlap làm rò credential qua tenant.

## Why the fix works
Tạo `HttpRequestMessage` riêng cho từng request, gán `request.Headers.Authorization` trước khi `SendAsync`. Không có trạng thái Authorization chia sẻ có thể bị ghi đè.

## How to verify
Sửa `starter/Program.cs` theo `solution/Program.cs`, rồi chạy `./scripts/verify.ps1`. Script luôn chạy starter, không chạy solution.

## Alternative fixes
Typed client riêng cho tenant có thể phù hợp khi lifecycle, pooling và tenant cardinality được kiểm soát; per-request header thường đơn giản hơn.

## Wrong / tempting fixes
`lock` quanh phần gán header mà không giữ tới khi gửi vẫn race. Tạo `new HttpClient()` mỗi request có thể tránh trạng thái chung nhưng làm tăng nguy cơ socket exhaustion và bỏ qua pooling. Thêm retry không khắc phục credential leak.

## Production implications
Đây là rủi ro phân tách tenant và có thể thành sự cố bảo mật. Không ghi token thô vào logs; log tenant ID đã kiểm duyệt, request correlation và kết quả policy. Xem xét revoke/rotate credential bị ảnh hưởng, audit truy cập chéo tenant.

## Trade-offs
Per-request message có thêm allocation nhỏ, đổi lại isolation rõ ràng. Bảo đảm dispose request/response và propagation cancellation trong ứng dụng thật.

## Senior engineer should notice
Test concurrency cần lịch trình overlap deterministic, không dựa vào sleep. Authorization phải được ràng buộc vào request và kiểm tra phía server, không tin tenant ID trong path.
