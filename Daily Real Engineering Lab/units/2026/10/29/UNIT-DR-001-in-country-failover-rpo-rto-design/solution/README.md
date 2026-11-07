# Reference Solution — SPOILER

**Chỉ đọc sau khi tự làm ADR.** Đây là một phương án có thể bảo vệ dưới ràng buộc giả lập, không phải đáp án kiến trúc duy nhất.

## 1. Symptoms
Kế hoạch DR được gọi là “multi-region ready” nhưng không chứng minh đồng thời RPO, RTO, data residency, latency, cost và single-writer safety.

## 2. Evidence
| Option | Residency | RPO | RTO | Write p99 | Budget | Fencing |
|---|---|---|---|---|---|---|
| A zone-only | PASS | UNKNOWN/FAIL cho region loss | FAIL 57m | PASS 112ms | PASS 16k | UNKNOWN |
| B Region Y | **FAIL** | observed 35s | observed 17m | observed 125ms | observed 19k | UNKNOWN |
| C X2 async | PASS | **FAIL ở C3: 425s** | observed 22–27m | observed 155–165ms | provisional 23.5k | UNKNOWN |
| D X2 sync | PASS | observed 0s | observed 28m | **FAIL 315ms** | **FAIL 28k** | UNKNOWN |
| E backups | PASS | **FAIL 900s** | **FAIL 51m** | PASS 112ms | PASS 9k | UNKNOWN |

C3 vượt RPO: **425 − 300 = 125 giây**. p95 lag 52 giây trên dashboard không chứng minh upper bound. RPO phải kiểm tra các acknowledged writes chưa durable ở site dự phòng, theo commit positions tại failure, không chỉ lag percentile.

## 3. Root cause
Không có phương án hiện tại được chứng minh đáp ứng **đồng thời** P1–P6. Slide dùng các chỉ số đẹp thay cho worst-case evidence và bỏ qua hard constraints.

## 4. One defensible decision
**NO-GO đối với lời cam kết DR hiện tại; CONDITIONAL GO cho việc hardening Site X2 trước khi production sign-off.** X2 ở trong X. Cần cải thiện replication, đo durable positions, thử backlog và network partition, định nghĩa write-admission policy khi lag tiến gần ngưỡng RPO (ví dụ safety trigger 240s, phải hiệu chỉnh theo telemetry delay), cùng fencing epoch trước promotion.

C chưa được coi là PASS sau khi đề xuất cải thiện; phải chạy drill chứng minh. Chặn write để giữ RPO có thể giảm availability, cần Product/SRE phê duyệt. 23.5k USD chỉ còn 1.5k USD headroom cho chi phí chưa tính.

## 5. Why this works conditionally
In-jurisdiction standby đáp ứng phạm vi dữ liệu. Admission control hạn chế write đã xác nhận nhưng chưa durable khi replica không theo kịp. Monotonic epoch/fencing ở authoritative write boundary ngăn old primary ghi sau promotion. Cần đo đúng và diễn tập, không chỉ có sơ đồ.

## 6. How to verify
1. Region-loss drills ở peak load, lag thấp/cao, network partition, old-primary rejoin.
2. Lưu last acknowledged commit và last durable replica position, xác định accepted-write loss ≤300s trong các fault scenario đã định; nếu không đo được, fail closed.
3. Đo RTO từ failure đến **safe writes resumed**, gồm detect/fence/checkpoint/promote/route/verify.
4. Prove fencing tại write boundary; test split brain và failback.
5. Write p99 ≤180ms; all-in cost ≤25k USD/tháng; Compliance sign-off cho PII flow.
6. Repeatable runbook có owner, rollback, integrity reconciliation và audit trail.

## 7. Alternative fixes
- B nhanh và rẻ nhưng PII ở ngoài X: không hợp lệ nếu chưa có ngoại lệ chính thức.
- D có RPO tốt nhưng đang vượt write p99 và budget; có thể xem lại nếu đo tối ưu hoặc được duyệt thay đổi.
- A chịu zone outage, không đủ cho mất cả region.
- E hữu ích cho backup/corruption, không thay thế hot standby đáp ứng RPO/RTO.
- C có tiềm năng, **chưa đạt** trong dữ liệu hiện có.

## 8. Wrong / tempting fixes
- “p95 lag xanh nên RPO pass”: bỏ sót tail và durable state.
- “Mã hóa rồi chuyển PII sang Y”: không tự miễn data residency.
- “Promote ngay cho nhanh”: có thể split brain.
- “Thêm replica CPU là xong”: không có proof về lag, partition, fencing.
- “Đếm RTO đến HTTP 200”: không chứng minh write an toàn.
- “Backup mỗi 15 phút là đủ”: vi phạm cả RPO lẫn RTO theo dữ liệu.

## 9. Production implications
Cần durable-position telemetry, lag alerts, admission control, explicit promotion authority, immutable backups, data-integrity audit, retry deduplication và failback drill. Nếu lag quá cao, ưu tiên nhất quán có thể phải từ chối write thay vì âm thầm phá RPO.

## 10. Trade-offs
Consistency, write availability, residency, latency, budget không thể tối ưu độc lập. Một thiết kế active-passive đơn giản, đo được và fail-closed có thể tốt hơn active-active phức tạp không chứng minh được correctness.

## 11. What a Senior engineer should notice
Phải dám nói “chưa đủ bằng chứng”, phân biệt zone với region, percentile với upper bound, durable với visible, failover với failback, và gắn owner/threshold cho mỗi gate. Không ký GO bằng slide.

## 12. Requirement change
RPO 60s khiến C càng không đạt. Write-admission sớm hơn có thể xung đột giới hạn 2 phút downtime/tháng. Cần đo cải thiện replica/network, xem xét synchronous acknowledgement trong X nếu latency/budget cho phép, hoặc thương lượng RPO/availability/budget có phê duyệt. Thử lag 90s + abrupt failure + old-primary return trước khi ký.
