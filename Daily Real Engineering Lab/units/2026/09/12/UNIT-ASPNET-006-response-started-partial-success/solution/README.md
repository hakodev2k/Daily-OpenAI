# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms

Failure request có thể trả `200 OK` và một phần CSV, trong khi server ghi exception. Client nhìn thấy success contract nhưng payload không hoàn chỉnh.

## 2. Evidence

Starter ghi status `200`, bắt đầu ghi body và `FlushAsync` trước khi thao tác có thể fail hoàn tất. Sau đó exception xảy ra khi response đã bắt đầu.

## 3. Root cause

HTTP response đã được committed trước khi exception xảy ra. Khi `HttpResponse.HasStarted == true`, middleware phía ngoài không còn khả năng đáng tin cậy để thay status code và headers thành một error response mới. Exception handler không thể “quay ngược” bytes đã gửi cho client.

## 4. Why the fix works

Reference solution hoàn thành phần tạo report có thể fail trước, rồi mới commit `200 OK` và body. Nếu tạo report fail, exception middleware vẫn chạy trước khi response bắt đầu nên có thể trả `500` nhất quán.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Script kiểm tra learner-editable `starter/`:

- failure request không còn `200` với partial CSV
- failure request không chứa data rows CSV
- success request vẫn trả đủ 5 rows

## 6. Alternative fixes

- Buffer vào file tạm hoặc stream có giới hạn thay vì giữ toàn bộ report trong memory.
- Với report rất lớn, chuyển sang asynchronous export job: tạo artifact hoàn chỉnh trước rồi cho client tải về.
- Nếu bắt buộc true streaming, định nghĩa protocol/failure contract mà consumer hiểu được, ví dụ framed stream có terminal status, thay vì giả vờ HTTP status có thể thay đổi sau khi body đã bắt đầu.
- Preflight các bước có khả năng fail trước khi ghi body, nếu chi phí và semantics cho phép.

## 7. Wrong / tempting fixes

- Chỉ thêm `try/catch` rồi tiếp tục trả partial CSV: che symptom nhưng contract vẫn sai.
- Cố gán `StatusCode = 500` sau khi response đã started: có thể ném lỗi hoặc không sửa được response mà client đã nhận.
- Chỉ tăng retry phía client: client vẫn không có tín hiệu đáng tin cậy để biết response `200` là incomplete.
- Buffer mọi export không giới hạn: sửa correctness nhưng có thể tạo memory pressure lớn. Phải xét kích thước report.

## 8. Production implications

Status code là một phần của wire contract. Với streaming, khi bytes đầu tiên đã được gửi, khả năng rollback ở HTTP layer gần như không còn. Thiết kế endpoint phải quyết định rõ atomicity, memory/cost, latency-to-first-byte và cách biểu diễn partial failure.

## 9. Trade-offs

Reference solution dùng in-memory buffering vì lab giả định report nhỏ và ưu tiên atomic response. Với dữ liệu lớn, file-backed buffering hoặc asynchronous export thường tốt hơn để tránh LOH/GC pressure. True streaming giảm memory và latency-to-first-byte nhưng làm failure semantics phức tạp hơn.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi “exception handler ở đâu” mà phải xác định response commit boundary. Cần phân biệt application exception handling với khả năng thay đổi protocol state sau khi dữ liệu đã được gửi, đồng thời chọn strategy dựa trên report size, SLA và consumer contract.