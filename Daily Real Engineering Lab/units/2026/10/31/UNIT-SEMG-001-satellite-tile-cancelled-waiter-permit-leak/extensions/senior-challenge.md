# Senior extension — Từ in-process gate đến multi-replica admission

Không sửa baseline khi chưa hoàn tất lab chính. Thiết kế cho 3 replica cùng gửi job đến một GPU worker có giới hạn 1 job. So sánh:
- SemaphoreSlim trong mỗi replica (không bảo vệ global capacity).
- Một queue consumer duy nhất có acknowledgement và retry.
- Distributed lease có fencing token và timeout.

Đưa ra decision record gồm invariant, failure recovery sau crash, idempotency, cancellation semantics, observability, chi phí và test plan. Không mặc định chọn Redis hoặc microservices khi chưa có bằng chứng về topology và yêu cầu.
