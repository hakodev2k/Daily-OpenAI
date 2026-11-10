# Reference decision — Spoiler

## Root cause và phép tính
Head sampling tại ingress chỉ thấy trace-id và thông tin đầu request, chưa biết request sẽ lỗi hay thành công. Do đó không thể bảo đảm giữ lỗi hiếm. Việc metrics ghi nhận 1.000 errors nhưng trace store chỉ có 47 error traces là hợp lý với mẫu ngẫu nhiên 5%.

| Candidate | Expected error traces | Expected success traces | Total | Budget |
|---|---:|---:|---:|---|
| A: head 5% | 50 | 49.950 | 50.000 | PASS |
| B: head 8% | 80 | 79.920 | 80.000 | PASS, không còn headroom |
| C: all errors + 5% successes | 1.000 | 49.950 | 50.950 | PASS |
| D: all errors + 8% successes | 1.000 | 79.920 | 80.920 | FAIL |

**Decision:** ưu tiên C trong điều kiện collector thực sự có đủ năng lực giữ spans đến outcome và không làm gián đoạn delivery. Phần 29.050 traces còn dư là headroom, không phải lời hứa đảm bảo 100% errors trong thực tế. Tail sampling có thể mất trace khi overload, restart, late span, hoặc propagation lỗi.

## Thiết kế triển khai
1. Giữ metrics error-rate, latency và delivery outcome không bị sampling; logs có correlation ID nhưng không lưu payload nhạy cảm.
2. Propagate W3C trace context nhất quán qua ingress, routing, notification. Sampling decision phải áp dụng ở collector và tránh upstream head-drop khiến collector không bao giờ thấy spans.
3. Buffer có giới hạn theo bytes, số trace và tuổi trace. Theo dõi pending traces, dropped spans, late spans, collector RSS, exporter failures, queue saturation và completeness.
4. Tail rule: giữ errors, ưu tiên P0, lấy 5% success theo trace-id; nếu quota bị đe dọa, giảm success sample trước, không chặn business request. Không hứa tuyệt đối giữ 100% errors khi collector mất dữ liệu.
5. Redact PII trước export, allowlist attributes, giới hạn cardinality và retention. Triển khai canary theo service/region, kiểm tra p95/p99 và chi phí.
6. Rollback về head 5% khi collector queue vượt 80% liên tục 5 phút, incomplete traces tăng trên 2%, hoặc p99 delivery tăng trên 10% so baseline có đối chứng. Ngưỡng là đề xuất cần hiệu chỉnh bằng số liệu thật.

## Wrong fixes
- **Bật 100% sampling ở mọi service:** phá ngân sách và tăng chi phí collector; không xử lý privacy.
- **Tăng head sampling từ 5% lên 8%:** chỉ kỳ vọng bắt 80 errors; vẫn bỏ sót lỗi hiếm, dùng hết quota.
- **Giữ 100% errors bằng cách kiểm tra status tại ingress:** outcome chưa tồn tại khi ingress quyết định; không thể thực hiện bằng head-only.
- **Chỉ tăng log verbosity:** có thể giúp điều tra cục bộ nhưng không thay thế distributed span causality; còn rủi ro PII.
- **Bật tail collector không giới hạn buffer:** rủi ro OOM và mất telemetry khi traffic tăng.

## Verification
Recompute A/B/C/D từ bộ 1.000.000 requests; kiểm tra 5 invariants trong worksheet. Mô phỏng overload 15.001 in-flight traces và delayed span sau 20 giây; mô tả expected degradation. So sánh metrics counters độc lập với retained traces; xác nhận PII scrub trên cả success/error paths. Không tuyên bố runtime test: lab được static-reviewed.
