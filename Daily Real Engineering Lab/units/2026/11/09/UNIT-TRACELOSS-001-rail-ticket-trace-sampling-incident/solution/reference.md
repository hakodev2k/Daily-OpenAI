# Reference Solution — spoiler

## 1. Symptoms
Trong 60 phút có 81 timeout theo metrics nhưng chỉ 1 exported timeout trace. Tổng 50,000 requests và 501 exported traces ≈ 1.002%.

## 2. Evidence
Error metrics độc lập với trace export; collector không có backlog rõ ràng; correlation IDs thiếu trace. Chưa đủ bằng chứng để quy lỗi cho UI deploy hoặc DNS.

## 3. Root cause
Head-based sampling 1% ra quyết định trước khi biết trạng thái lỗi. Timeout hiếm nên phần lớn bị loại, không phải request không tồn tại.

## 4. Why fix works
Một phương án có thể bảo vệ được: metrics request/error/latency độc lập, low-cardinality; collector tail sampling giữ error traces ưu tiên và sample 1% successful traces. Giới hạn memory, queue, decision wait và daily export budget; đo `traces_dropped_total{reason}`, `sampling_decisions_total`, queue size, collector RSS. Correlation ID không chứa PII. Redact payment data.

## 5. How to verify
Ngày thường: 1,800 errors + 1% × 1,198,200 successes ≈ 13,782 traces, dưới 36,000. Incident 30 phút: 4,000 errors + 1% × 46,000 successes ≈ 4,460 traces, chưa tính traffic ngày còn lại. Low traffic: 2 + 1% × 1,198 ≈ 14 traces kỳ vọng (sampling có biến động). Collector-pressure: 10,000 errors + 1% × 90,000 successes ≈ 10,900 traces nếu buffer/export không nghẽn. Chạy synthetic staging replay, đo error-trace recall, queue, RSS, dropped reasons, chi phí và tail latency.

## 6. Alternatives
Tăng head sampling: đơn giản, vẫn bỏ sót lỗi hiếm. Ưu tiên sampling theo endpoint payment: giảm mất trace nhưng chưa biết outcome cuối. Redacted error logs: hỗ trợ audit, không thay full trace.

## 7. Wrong fixes
Dùng trace count tính tỷ lệ lỗi; tăng tỷ lệ sampling mà không có recall SLO; giữ 100% mọi trace không giới hạn; log toàn bộ payment payload.

## 8. Production implications
Tail sampling giữ spans trong memory trước quyết định; 512 MiB và wait 5s phải kiểm chứng bằng peak concurrent traces và bytes/trace, không suy ra từ requests/day. Route toàn bộ spans cùng trace tới cùng collector decision point. Khi quá tải, giảm success sampling trước, sau đó báo dropped errors minh bạch. Crash trước flush vẫn có thể mất trace.

## 9. Trade-offs
Tail sampling tăng khả năng điều tra nhưng thêm state, memory, delay, collector affinity và vận hành. Không cam kết 100% end-to-end trace capture khi service crash.

## 10. Senior engineer should notice
Observability pipeline cũng có failure modes và SLO riêng; không dùng chính dữ liệu sampled để chứng minh coverage tuyệt đối.

## References
- https://opentelemetry.io/docs/concepts/sampling/
- https://opentelemetry.io/docs/collector/
