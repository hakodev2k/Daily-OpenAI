# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Batch A gồm 48 việc đến cùng phút 0. Hai hồ sơ khẩn B/C đến phút 1–2 phải chờ tới phút 6 mặc dù hệ thống xử lý đủ 8 việc/phút.

## 2. Evidence
8 worker × 1 việc/phút = 8 starts/phút. Các batch A chiếm hết 48 vị trí phút 0–5. U01 bắt đầu phút 6, wait=5; U02 bắt đầu phút 6, wait=4. Đây là dữ liệu mô phỏng, không phải số đo production.

## 3. Root cause
Một FIFO chung tối ưu thứ tự đến nhưng không có contract fairness theo tenant hay class-of-service. Backlog lớn của A làm tăng head-of-line waiting cho quận khác.

## 4. Why the design works
Dùng scheduler có hàng đợi riêng theo district, weighted round-robin giữa district đang có backlog, và lane urgent có quota (ví dụ tối đa 2/8 slots mỗi phút, cho phép mượn slots khi lane rỗng). Khi U01 đến phút 1, nó có thể nhận slot ở phút 1; U02 nhận ở phút 2. Các slots còn lại xử lý A, nên tổng capacity vẫn là 8 khi đủ việc. Đặt aging để normal lâu ngày không bị bỏ đói. Không hứa SLA vô điều kiện khi urgent arrival vượt capacity.

## 5. Verification
Replay theo minute: tại mỗi phút enqueue arrivals, lấy tối đa 8 jobs từ policy, ghi startMinute; tính wait=start-arrival. Assert tất cả ID xuất hiện đúng một lần, start>=arrival, không quá 8 starts/phút, U01 wait<=3, U02 wait<=3. So sánh p95 từng district và class; chạy thêm adversarial workload: tất cả quận đều urgent liên tục.

## 6. Alternatives
(A) Tăng worker từ 8 lên 16: giảm độ trễ nhưng không bảo đảm fairness khi batch tăng, tăng chi phí. (B) Tách worker pool cứng theo quận: cô lập tốt nhưng lãng phí capacity khi quận rảnh. (C) Weighted fair scheduling với work-conserving capacity: cân bằng hiệu quả và công bằng, cần scheduler state và monitoring.

## 7. Wrong / tempting fixes
Tăng timeout chỉ che triệu chứng; scale-out đơn thuần không sửa fairness; strict priority vô hạn có thể làm normal starvation; chia queue nhưng vẫn có một consumer FIFO toàn cục không đảm bảo isolation.

## 8. Production implications
Queue partitioning, retries và dead-letter không được đánh đổi at-least-once semantics; consumer cần idempotency key theo hồ sơ + phiên bản. Metrics: queue age theo district/priority, p95 wait, dispatch share, saturation, retry count, DLQ. Dùng bounded backlog, admission control và cảnh báo khi arrival rate dài hạn vượt service rate.

## 9. Trade-offs
Weighted fairness tăng độ phức tạp cấu hình, độ trễ scheduling và vận hành; quota khẩn phải dựa trên SLO và arrival distribution. Nếu công việc có thời lượng khác nhau, round-robin theo số job không tương đương fairness theo thời gian CPU; cân nhắc deficit round-robin với estimated cost.

## 10. Senior engineer should notice
Phải phân biệt throughput với latency fairness, nêu workload assumptions, kiểm thử overload, giữ exactly-once *effect* bằng idempotency thay vì giả định broker giao đúng một lần, và có kế hoạch rollback bằng feature flag.

## Sources for further reading
- Microsoft Learn: https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
- Microsoft Learn: https://learn.microsoft.com/en-us/azure/architecture/patterns/priority-queue
