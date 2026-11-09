# Incident dossier (synthetic)

Gateway → Booking API → Payment Adapter → Bank sandbox; gateway trả 504 sau 3 giây nếu adapter timeout.

- 09:00 rollout thay đổi export telemetry.
- 09:20 dashboard payment_timeout_total tăng, truy vấn traces status=ERROR trả rất ít kết quả.
- 09:30 collector CPU 42%, exporter queue <10; không có backlog rõ ràng.
- 09:40 p95 request thành công không đổi; p99 tổng thể tăng nhẹ.
- 09:50 ba correlation IDs khách hàng báo lỗi không tìm được trong trace store.
- 10:00 sampling được cấu hình ở đầu request với tỷ lệ 1%, trước khi Payment Adapter trả kết quả.
- UI deploy lúc 09:12 và DNS latency tăng 3ms; chưa có bằng chứng đây là nguyên nhân.

## Constraints
1.2 triệu requests/ngày; tối đa 36,000 exported traces/ngày bình thường. Error rate bình thường 0.15%. Giả định incident 50,000 requests/30 phút với 8% errors. Collector có 512 MiB RAM cho buffer, decision wait tối đa 5 giây. Không lưu PII, card data, credentials trong telemetry. Error metrics phải độc lập với trace sampling.

## Deliverables
Bảng evidence / hypotheses / disconfirming tests, ADR với ít nhất hai alternatives, budget theo replay cases, error recall target, metric dropped traces, overload policy và rollback.
