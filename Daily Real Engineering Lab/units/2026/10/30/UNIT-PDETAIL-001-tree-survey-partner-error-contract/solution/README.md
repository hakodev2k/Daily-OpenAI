# Reference Solution — Chỉ xem sau khi tự điều tra

## 1. Symptoms

Các batch bị từ chối với 422, 503 và 500 đều bị quy về HttpRequestException. 200 vẫn thành công. Bộ phận hỗ trợ thiếu mã lỗi đối tác và trace ID.

## 2. Evidence

Fake partner trả 422 và 503 dưới dạng application/problem+json, có code và traceId. Response 500 dùng text/html nhưng có X-Request-ID. Những trường này hiện diện trên wire, nhưng không có trong contract exception hiện tại.

## 3. Root cause

Trong starter, EnsureSuccessStatusCode() ném exception ngay sau khi nhận response không thành công. Không có bước diễn giải body/headers thành contract lỗi ở tầng ứng dụng. HttpRequestException có thể giữ status, nhưng không tự mang code, traceId và quyết định retry nghiệp vụ.

## 4. Why the fix works

[Reference code](PartnerClient.cs) kiểm tra response.IsSuccessStatusCode trước. Khi không thành công, code đọc có giới hạn payload lỗi nếu Content-Type là application/problem+json, trích code/traceId, dùng X-Request-ID làm fallback và ném PartnerSyncException có kiểu. Không ghi nguyên detail/body vào message. Lỗi 5xx được đánh dấu retryable, 422 không retryable theo policy của lab.

## 5. How to verify

Tự sửa starter/PartnerClient.cs rồi chạy ./verify.ps1. Bốn case phải PASS: 422 + TREE_PLOT_INVALID + trace-422 + no retry; 503 + PARTNER_MAINTENANCE + trace-503 + retry; 500 HTML + UPSTREAM_HTTP_ERROR + trace-500 + retry; 200 vẫn trả SyncReceipt. verify.ps1 không dùng solution. Sau khi tự thử mới so sánh code tham khảo.

## 6. Alternative fixes

Có thể trả discriminated result thay vì exception, hoặc xây middleware/client adapter chuẩn hóa ProblemDetails. Với nhiều đối tác, adapter riêng từng contract giúp tránh giả định rằng mọi API trả cùng schema.

## 7. Wrong / tempting fixes

- Retry mọi non-2xx: lặp lại 422 không thể tự sửa dữ liệu và có thể tăng tải.
- Chỉ log ex.Message: không khôi phục mã lỗi và trace bị mất.
- Log nguyên response body: có thể rò dữ liệu nhạy cảm hoặc log injection.
- Catch HttpRequestException rồi gán code mặc định cho tất cả: che khuất khác biệt nghiệp vụ.
- Tăng timeout: không giải quyết việc mất thông tin lỗi.

## 8. Production implications

Giới hạn kích thước body, sanitize mã trace/code không tin cậy, áp dụng retry budget và idempotency key cho POST nếu triển khai retry tự động. Cancellation phải được truyền vào quá trình đọc body. Đo tỷ lệ 422 riêng với 5xx, tránh ghi payload không cần thiết.

## 9. Trade-offs

Typed exception làm contract rõ nhưng gắn caller với mô hình lỗi. Result object giúp control flow tường minh hơn nhưng yêu cầu xử lý mọi nhánh. Bounded parsing có thể bỏ qua body quá lớn; fallback vẫn phải an toàn và quan sát được. 5xx retryable không đồng nghĩa retry ngay lập tức hoặc vô hạn.

## 10. Senior engineer should notice

Status, headers, body và lỗi domain là bốn lớp thông tin khác nhau. Chỉ có status không đủ cho điều tra. Trước khi tự động retry một POST, phải xác định tính idempotent và outcome ambiguity. Hợp đồng lỗi của upstream là untrusted input, cần validation và logging discipline.
