# Reference Solution — SPOILER

Chỉ xem sau khi reproduce, ghi hypothesis và tự sửa. Mã tham khảo: [TracePipeline.cs](TracePipeline.cs). **verify.ps1 luôn chạy starter**, không chạy solution.

## 1. Symptoms

Check-in thành công, local service log không chứa PII nhưng downstream telemetry có email và bốn số cuối giấy tờ giả lập.

## 2. Evidence

`OUTBOUND_BAGGAGE` chứa dữ liệu hành khách; `LOCAL_LOG_SAFE=True`, `BUSINESS_RESULT=ACCEPTED`. Đường truyền của trace context cần được phân biệt với application logs.

## 3. Root cause

Starter gắn `passenger_email` và `passport_last4` bằng `Activity.AddBaggage`. Baggage là context có thể được truyền qua service boundary bởi propagator; đây không phải nơi lưu dữ liệu cá nhân. Local harness cố tình mô phỏng export bằng cách tuần tự hóa `Activity.Baggage`.

## 4. Why the fix works

Chỉ đưa opaque `request_id` vào baggage để giữ correlation. Không đưa PII vào tag hoặc log thay thế. Local tags `gate` và `checkin.result` là thông tin vận hành không nhạy cảm trong fixture; với production phải phân loại và kiểm soát cardinality.

## 5. How to verify

Sửa `starter/TracePipeline.cs` tương đương mã tham khảo rồi chạy `pwsh -File ./scripts/verify.ps1`. Quan sát `SENSITIVE_DATA_EXPORTED=False`, `CORRELATION_OK=True`, `BUSINESS_RESULT=ACCEPTED`, `VERIFY_PASS`. Chạy thử lại bằng dữ liệu synthetic khác để kiểm tra chính sách.

## 6. Alternative fixes

- Dùng allowlist cho baggage keys tại ingress và egress; cân nhắc exporter processor redaction như defense-in-depth.
- Chỉ dùng trace ID và server-side lookup có kiểm soát để truy vết booking thay vì gửi thuộc tính hành khách.
- Dùng telemetry sampling và retention policies để giảm exposure, nhưng không coi chúng là biện pháp ngăn phát tán dữ liệu.

## 7. Wrong / tempting fixes

- Chỉ xóa PII khỏi application log không sửa baggage propagation.
- Tắt toàn bộ tracing khiến mất khả năng điều tra và không giải quyết nguyên tắc phân loại dữ liệu.
- Chuyển PII từ baggage sang tags có thể vẫn đưa PII đến collector.
- Hash email không có khóa hoặc với salt cố định vẫn có thể là identifier dễ liên kết; không mặc định là ẩn danh.

## 8. Production implications

Kiểm tra trace context propagator thực tế (W3C baggage), proxy headers, OpenTelemetry collector, downstream vendors, retention, access controls và incident response. Dữ liệu đã xuất cần xử lý theo chính sách bảo mật và retention. Không sử dụng dữ liệu khách hàng thật để test.

## 9. Trade-offs

Opaque correlation ID giữ khả năng truy vết nhưng cần lookup nội bộ có phân quyền. Allowlist giúp rõ contract nhưng đòi hỏi governance và review khi thêm fields. Redaction ở exporter là lớp phòng vệ cuối, không thay thế data minimization tại nguồn.

## 10. Senior engineer should notice

Xác định chính xác trust boundary: thông tin nào tồn tại trong process, thông tin nào đi theo request, thông tin nào vào collector. Đo lường và verify cả chức năng lẫn quyền riêng tư, không chỉ xóa field khỏi output mẫu.

**Giới hạn lab:** Bộ dựng header local là mô hình tối giản, không phải triển khai W3C Baggage hoặc OpenTelemetry exporter hoàn chỉnh. Không kết luận production đã an toàn chỉ từ local PASS.
