# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Client nhận đúng 200, 429, 503, 404; dashboard metric lại ghi 200 cho cả bốn request. Đây là sai lệch về **measurement**, không phải lỗi status của endpoint.

## 2. Evidence
Chạy `./reproduce.ps1` trên starter gốc: response contract PASS, metric matches client FAIL; ba dòng 429/503/404 được ghi metric=200. Log CPU và latency không giải thích được sai lệch này.

## 3. Root cause
Trong `BulletinStatusMetricMiddleware.InvokeAsync`, recorder đọc `HttpResponse.StatusCode` **trước** khi gọi/await downstream `RequestDelegate`. `DefaultHttpContext` khởi tạo status mặc định 200; endpoint chỉ cập nhật status ở giai đoạn tiếp theo. Instrumentation đang lấy trạng thái chưa hoàn tất.

## 4. Why the fix works
Chờ downstream hoàn thành rồi mới ghi metric. Thay thứ tự trong middleware:
```csharp
await _next(context);
_recorder.Record(context.Request.Path.Value ?? "", context.Response.StatusCode);
```
Mã đầy đủ nằm ở [Program.cs](Program.cs). Với các endpoint bình thường không ném exception, recorder đọc được status cuối do endpoint đặt.

## 5. How to verify
Chỉnh `starter/Program.cs` (không chỉnh harness/cases) rồi chạy `./verify.ps1`. Cả `Response contract` và `Metric matches client` phải PASS; status metrics lần lượt 200, 429, 503, 404. `reproduce.ps1` khi đó không còn xác nhận được triệu chứng cũ, đúng với mục đích của script.

## 6. Alternative fixes
Có thể dùng một middleware bao ngoài toàn pipeline và ghi metric sau khi downstream/exception handler đã xử lý, hoặc callback ở lifecycle phù hợp nếu cần status khi response thực sự bắt đầu/kết thúc. Chọn cơ chế dựa trên định nghĩa metric và streaming semantics.

## 7. Wrong / tempting fixes
- Thay status endpoint thành 200 để metric khớp: che giấu lỗi thật, phá contract HTTP.
- Bỏ 429/503 khỏi dashboard: làm sai mẫu số và SLO.
- Thêm retry trong endpoint: không sửa timing đo lường và có thể gây side effects.
- Chỉ đổi màu dashboard: không sửa dữ liệu gốc.

## 8. Production implications
Kiểm tra vị trí exception handler, endpoint routing, response streaming và middleware metrics. Nếu downstream ném exception và handler nằm **bên ngoài** middleware này, lệnh sau `await _next` có thể không chạy; `try/finally` đảm bảo callback nhưng status lúc đó có thể chưa là 500 cuối cùng. Không tuyên bố giải pháp mẫu bao phủ mọi exception. Cần thiết kế vị trí đo hoặc lifecycle callback và test thực tế.

## 9. Trade-offs
Đo sau downstream phản ánh status hoàn tất trong case đơn giản, nhưng việc ghi metric đồng bộ cần nhanh và tránh làm tăng latency. Dùng recorder thread-safe khi đưa vào app có concurrency; mẫu dùng List vì harness chạy tuần tự. Không đưa request ID/user ID làm metric label gây cardinality cao.

## 10. Senior engineer should notice
Sai lệch monitoring có thể gây missed incident ngay cả khi business logic đúng. Phải so sánh nguồn dữ liệu độc lập (client, access log, metrics), kiểm tra metric contract và đo tại đúng lifecycle boundary. Sau fix, thêm regression cho 200/4xx/5xx, exception path và streaming path trước khi triển khai production.
