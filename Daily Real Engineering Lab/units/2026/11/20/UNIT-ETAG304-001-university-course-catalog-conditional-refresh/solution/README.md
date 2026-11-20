# Reference Solution — chỉ xem sau khi tự thử
## Symptoms
Lần đầu GET 200; lần sau GET có If-None-Match nhận 304 nhưng job lỗi.

## Evidence
Trace xác nhận request có validator "v1"; server trả 304 không body; health upstream vẫn ổn.

## Root cause
`EnsureSuccessStatusCode()` chỉ chấp nhận 2xx, trong khi 304 là phản hồi HTTP cache validation hợp lệ nhưng thuộc 3xx. Không thể parse body 304 như body 200.

## Why the fix works
[Program.cs](Program.cs) phân biệt 304 trước khi kiểm tra 2xx; giữ nguyên snapshot đã cache khi representation chưa thay đổi; 200 thay snapshot và ETag sau khi đọc body; 503 vẫn ném exception mà không phá dữ liệu cũ.

## How to verify
Sửa `starter/Program.cs` theo cách của bạn, chạy `./verify.ps1`. Có thể đối chiếu bằng cách copy reference vào starter **sau khi** hoàn tất tự điều tra.

## Alternative fixes
Không dùng conditional GET và luôn tải 200 là đơn giản hơn nhưng tốn bandwidth; chấp nhận được khi catalog rất nhỏ hoặc upstream không hỗ trợ validators.

## Wrong or misleading fixes
Nuốt mọi `HttpRequestException` che 503 thực sự; coi 304 như danh sách rỗng gây mất dữ liệu; bỏ `EnsureSuccessStatusCode` nhưng vẫn parse body 304 gây sai snapshot.

## Production implications
Persist validator cùng representation một cách nguyên tử; 304 không có cache hợp lệ cần fail an toàn; kiểm tra ETag theo resource và cache scope; lưu metrics cho 200/304/5xx.

## Trade-offs
Conditional GET tiết kiệm bandwidth nhưng thêm trạng thái và edge cases. Cần chính sách stale-data rõ ràng khi upstream 503.

## What a Senior engineer should notice
Phân biệt transport success, HTTP semantics và business refresh success; kiểm tra invariant snapshot trước và sau failure, không chỉ assertion về status.
