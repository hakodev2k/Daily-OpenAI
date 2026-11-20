# Reference Solution — spoiler
Chỉ xem sau khi reproduce và tự sửa.

## Symptoms & evidence
Với lịch interleaving cố định, tenant A được gửi cùng credential của tenant B. X-Tenant vẫn đúng, vì vậy lỗi không nằm ở fake partner mapping.

## Root cause
Một HttpClient được dùng lại cho nhiều logical tenant operations. Mỗi operation gán `DefaultRequestHeaders.Authorization` vào state chung. Tenant A chờ tenant B ghi header; cả hai gửi sau khi header mặc định đã trở thành token B. DefaultRequestHeaders không phải nơi lưu credential biến thiên theo request; thay đổi nó khi có request đồng thời cũng không phải mô hình thread-safe được bảo đảm.

## Why fix works
Gán `request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token)` lên từng `HttpRequestMessage`, giữ nguyên shared HttpClient. Mỗi request có state riêng; interleaving không còn làm thay đổi credential request khác. Xem `solution/Program.cs` để đối chiếu, nhưng hãy sửa `starter/Program.cs` để `scripts/verify.ps1` kiểm tra chính code của bạn.

## Verification
`./scripts/reproduce.ps1` phải exit 0 trên starter nguyên bản; sau khi sửa, `./scripts/verify.ps1` exit 0 và in `ISOLATION_OK`. Thử chạy verify nhiều lần; thay token/tenant trong test và kiểm tra isolation.

## Alternatives and trade-offs
Typed client/HttpClientFactory vẫn hữu ích cho quản lý handler lifetime, nhưng không tự động bảo vệ per-tenant credential nếu tiếp tục ghi shared DefaultRequestHeaders. Client riêng theo tenant chỉ phù hợp nếu có isolation và lifecycle quản lý rõ ràng; tốn tài nguyên và phức tạp hơn.

## Wrong fixes
- Thêm `Task.Delay`: chỉ che race.
- Tạo `new HttpClient()` cho từng request: giảm shared state nhưng làm mất lợi ích connection pooling và có nguy cơ socket exhaustion.
- Lock quanh riêng lệnh gán header: không khóa toàn bộ vòng đời request và không thiết lập request ownership.
- Log Authorization để debug: làm lộ secret; chỉ log tenant pseudonym và correlation ID.

## Production implications
Thu hồi/rotate token có nguy cơ lộ, audit outbound requests theo tenant và correlation ID, đánh giá cross-tenant data exposure. Thêm regression test với interleaving điều khiển được; không dựa vào timing ngẫu nhiên.

## References
- https://learn.microsoft.com/dotnet/api/system.net.http.httpclient.defaultrequestheaders
- https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines
- https://learn.microsoft.com/dotnet/api/system.net.http.httprequestmessage.headers
