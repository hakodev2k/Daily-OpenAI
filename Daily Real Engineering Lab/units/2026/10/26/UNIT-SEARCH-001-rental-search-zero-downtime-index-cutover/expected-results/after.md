# After — Acceptance gates cho ADR

Một phương án được xem là **đủ điều kiện trình production review** khi có bằng chứng và owner cho tất cả mục sau:

1. **Availability:** không gây planned query outage; canary theo từng phần trăm traffic; 5xx không vượt baseline được định nghĩa trong runbook.
2. **Latency:** p95 search <= 250 ms ở 1,800 requests/s, có thời gian đo và điều kiện rollback.
3. **Freshness:** end-to-end source-to-search lag <= 90 giây, duy trì ít nhất 30 phút trước cutover; ghi rõ checkpoint.
4. **Correctness:** kiểm tra semantic query và quyền hiển thị trên sample có phân tầng; không chỉ so document count. Kiểm tra create/update/delete và event out-of-order.
5. **Capacity:** backfill, backlog, catch-up, storage peak và CDC retention có phép tính; mô tả điều kiện throughput không đủ.
6. **Rollback:** diễn tập < 10 phút, không mất update trong khoảng thời gian canary/cutover; xác định cửa sổ giữ v1 và compatibility của ứng dụng.
7. **Budget:** tạm thời <= 2.5x trong <= 48 giờ; ghi rõ nếu một kịch bản phải hoãn hoặc xin thay đổi budget.
8. **Security:** tenant/owner visibility regression tests trên schema mới; không có dữ liệu thật trong lab.
9. **Observability:** dashboard và cảnh báo cho query p95, 5xx, replay lag, error rate, missing-listing sample, storage và backlog.
10. **Go/no-go:** ai có quyền bấm cutover, ai xác nhận Product, và trigger rollback cụ thể.

Không có `verify.ps1` tự động cho quyết định kiến trúc này: verification là một review theo acceptance gates với evidence định lượng. Reference solution là một phương án khả thi, không phải đáp án duy nhất.
