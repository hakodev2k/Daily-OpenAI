# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
API URL không tồn tại trả 200 HTML dù client mong 404 JSON. SPA route hợp lệ vẫn hiển thị bình thường; vì vậy HTTP 200 trên dashboard che mất lỗi hợp đồng API.

## 2. Evidence
Contract in ra ba API path sai có `200 text/html`, trong khi `/api/permits/42` là `200 application/json`. Static asset có dấu chấm vẫn 404. Như vậy vấn đề gắn với endpoint matching, không phải JSON serializer của API hợp lệ.

## 3. Root cause
`MapFallback` mặc định dùng pattern `{*path:nonfile}` với mức ưu tiên fallback. Nó phục vụ cả đường dẫn SPA không có dấu chấm **lẫn** API path không được handler khác khớp. Fallback HTML trả 200 nên API client không phân biệt được route không tồn tại.

## 4. Why the fix works
[PermitRoutes.cs](PermitRoutes.cs) khai báo route catch-all dành riêng cho `/api/{**path}` trả 404 JSON. Endpoint API cụ thể được ưu tiên hơn catch-all, còn fallback SPA chỉ xử lý các URL non-file bên ngoài namespace API. Tách hợp đồng API và SPA tại routing boundary, không sửa status trong client.

## 5. How to verify
Chạy `./scripts/verify.ps1` từ unit root sau khi sửa **starter**. Tất cả sáu kiểm tra phải PASS. Có thể chạy `dotnet run --project ./contract/Contract.csproj -- --mode reference` để kiểm mã tham khảo sau khi đã tự làm.

## 6. Alternative fixes
- Route group `/api` với endpoint catch-all/404 theo namespace: phù hợp khi nhiều API modules cùng host.
- Tách SPA và API sang host hoặc subdomain khác: giảm xung đột routing nhưng tăng vận hành và cấu hình CORS/proxy.
- Middleware nhận diện API prefix trước SPA fallback: có thể hợp lệ nhưng phải cẩn thận về thứ tự middleware, endpoint routing và status/body contract.

## 7. Wrong or misleading fixes
- Bắt lỗi JSON parser ở frontend rồi coi như 404: che HTTP status sai, gây sai lệch telemetry và cache.
- Trả 404 cho **mọi** URL không được map: làm hỏng client-side routing của SPA.
- Thêm từng route sai vào danh sách deny: không bao phủ endpoint mới, method khác và typo khác.
- Chỉ đổi Content-Type thành JSON nhưng vẫn 200: không sửa semantic HTTP.

## 8. Production implications
Theo dõi riêng `http.server.request.duration` và tỷ lệ 4xx của namespace API; kiểm tra gateway/CDN không cache HTML fallback dưới API path. Kiểm thử cả GET và các method khác nếu ứng dụng có contract tương ứng; cân nhắc Problem Details thống nhất trong production.

## 9. Trade-offs
Một catch-all API 404 nhỏ, dễ bảo trì và không cần thêm service. Tuy nhiên cần quản lý các prefix versioned API (`/api/v2`) và tránh che mất 405 Method Not Allowed của endpoint hợp lệ khi định nghĩa catch-all đa phương thức. Khi production cần phân biệt 404/405, kiểm thử thêm method mismatch và thiết kế endpoint fallback theo method/route constraints phù hợp.

## 10. What a Senior engineer should notice
HTTP 200 không đồng nghĩa business success; SPA và API chia sẻ origin phải có ranh giới URL rõ ràng. Test không chỉ kiểm tra happy path mà còn kiểm tra negative route, media type và observability. Hãy kiểm tra 404, 405, cache và reverse proxy khi nâng cấp routing.

Tài liệu chính thức: [ASP.NET Core routing](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-8.0), [Minimal API route handlers](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/route-handlers?view=aspnetcore-8.0).
