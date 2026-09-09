# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Hai request đầu nhận headers `200` gần như ngay lập tức. Request thứ ba không nhận được headers trong budget dù local downstream server vẫn chạy bình thường.

## 2. Evidence

- `SocketsHttpHandler.MaxConnectionsPerServer = 2`.
- Request 1 và 2 dùng `HttpCompletionOption.ResponseHeadersRead`.
- Server tiếp tục stream response body sau khi headers đã được gửi.
- Starter giữ hai `HttpResponseMessage` sống cho tới cuối chương trình.
- Request thứ ba chỉ tiến triển sau khi có connection slot khả dụng.

## 3. Root cause

Với `ResponseHeadersRead`, `SendAsync` hoàn thành khi headers đã sẵn sàng, không phải khi response body đã được consume hoàn toàn. Hai response đầu tiên vẫn sở hữu lifecycle của streaming body; starter giữ chúng sống nên hai connection slot bị chiếm. Với `MaxConnectionsPerServer = 2`, request thứ ba phải chờ slot và chạm budget.

## 4. Why the fix works

Giới hạn lifetime của từng `HttpResponseMessage` bằng `using`/`Dispose` sau khi đã lấy metadata cần thiết. Khi response không còn cần dùng, dispose giúp giải phóng hoặc đóng resource phía dưới để handler có thể phục vụ request tiếp theo.

Reference code trong `solution/Program.cs` xử lý từng response trong scope riêng và không giữ chúng trong collection.

## 5. How to verify

Sửa `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Verification chỉ pass khi learner-editable `starter/`:

- exit code bằng `0`
- không còn dòng `SYMPTOM:`
- có `Request 3 completed: 200`

## 6. Alternative fixes

Nếu business flow thực sự cần đọc body, consume stream trong một scope có lifetime rõ ràng rồi dispose response sau khi hoàn tất. Nếu endpoint hỗ trợ metadata bằng `HEAD`, dùng `HEAD` có thể phù hợp hơn, nhưng đó là API-contract decision chứ không phải mẹo để né lifecycle management.

## 7. Wrong / tempting fixes

- Tăng `MaxConnectionsPerServer`: có thể trì hoãn symptom nhưng resource leak/lifetime sai vẫn còn.
- Retry request 3: làm tăng pressure và không giải phóng slot đang bị giữ.
- Tăng timeout: chỉ cho request chờ lâu hơn.
- Tạo `HttpClient` mới cho mỗi request: phá vỡ pooling strategy và tạo vấn đề khác; không sửa ownership của response hiện tại.

## 8. Production implications

Streaming HTTP code thường có memory profile tốt hơn buffering toàn bộ response, nhưng đổi lại developer phải quản lý response/stream lifetime chặt chẽ hơn. Connection-pool exhaustion có thể trông giống downstream latency hoặc network failure dù nguyên nhân nằm trong caller.

## 9. Trade-offs

`ResponseHeadersRead` hữu ích cho payload lớn và streaming, nhưng complexity cao hơn `ResponseContentRead`. Nếu payload nhỏ và throughput không yêu cầu streaming, cách mặc định có thể đơn giản và an toàn hơn. Chọn completion mode dựa trên evidence thay vì mặc định tối ưu hóa allocation.

## 10. What a Senior engineer should notice

Timeout là symptom, không phải root cause. Khi capacity hữu hạn bị cạn, cần truy lifecycle của resource đang giữ capacity: connection, stream, response, semaphore, DB connection hoặc lease. Senior engineer phải phân biệt downstream service unhealthy với caller-side resource ownership failure trước khi scale hoặc retry.
