# Acceptance — tự đánh giá

| Tiêu chí | Pass khi |
|---|---|
| Baseline | W2: B=95, C=5, A=0; W3: B=100, C=0, A=0; giải thích 429 |
| Safety | Mọi cửa sổ accepted tổng <=120 |
| Urgent | A accepted >=30 trong W1–W3 |
| Fairness | B không thể chiếm hết phần dành cho A |
| Borrowing | Định nghĩa rõ capacity nhàn rỗi có được mượn không, khi nào thu hồi |
| Consistency | Giải thích quota atomic xuyên hai gateway replicas hoặc giới hạn overshoot |
| Retry | 429 có Retry-After; client dùng jitter, capped retry; tránh retry storm |
| Failure | Khi quota store unavailable: explicit fail-open/fail-closed theo tenant và safety cap |
| Observability | Counters accepted/rejected theo tenant class + reason, latency, saturation, retry rate |
| Rollout | Canary theo cohort; kill switch; rollback policy; kiểm chứng ở W2/W3 và load test |
| Trade-offs | So sánh A/B/C/D theo availability, cost, complexity, fairness |

Không chấp nhận phương án làm A đạt 30 bằng cách giả định B tự giảm tải, hoặc chỉ scale-out mà không chứng minh downstream capacity.
