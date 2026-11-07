# Reference Solution — SPOILER

**Chỉ đọc sau khi reproduce và tự thử sửa.** Tham khảo [LeaseAndPlan.cs](LeaseAndPlan.cs). Có thể copy file tham chiếu sang `starter/LeaseAndPlan.cs` rồi chạy `.\scripts\verify.ps1`; verification luôn build từ `starter/`.

## 1. Symptoms
Hai worker có quyền thực thi tại các thời điểm khác nhau. Worker mới đã lưu `plan-from-B` nhưng kết quả cuối trở thành `plan-from-A` do worker cũ hoàn thành muộn.

## 2. Evidence
Đồng hồ ban đầu tick=0: A nhận lease epoch 1, hết hạn tại tick=10. Tick=11: B nhận lease epoch 2. B commit trước, sau đó A commit. Starter trả `true` cho cả hai và lưu epoch 1 cuối cùng.

## 3. Root cause
Lease expiry chỉ cho phép người khác nhận quyền; nó **không dừng** tác vụ cũ và không tự thu hồi quyền ghi vào resource. `PlanStore.TryCommit` chỉ khóa thao tác ghi về mặt thread safety nhưng không áp dụng thứ tự quyền sở hữu. Một stale owner có thể ghi đè kết quả của owner mới.

## 4. Why the fix works
Lease được cấp **epoch tăng đơn điệu**. Storage giữ `LeaseEpoch` của bản ghi đã chấp nhận; trong cùng một critical section, từ chối write có epoch nhỏ hơn epoch đã lưu. B đã commit epoch 2 thì A epoch 1 không thể thay thế. Kiểm tra và cập nhật phải **atomic**. Trong production, điều kiện này phải được thực thi tại chính durable storage (ví dụ conditional update/CAS hoặc transaction), không phải chỉ kiểm tra ở worker.

## 5. How to verify
`reproduce.ps1` tái hiện lỗi **starter gốc**. Sau khi sửa, `verify.ps1` kiểm tra stale writer, first writer, retry cùng epoch, owner mới và độc lập giữa các route. Kỳ vọng `ALL_VERIFICATIONS_PASSED` và exit 0. Đây là static-reviewed sample, chưa tuyên bố đã build trong môi trường của generator.

## 6. Alternative fixes
- Conditional write dựa trên monotonic fencing token tại DB: phù hợp multi-instance, có thể dùng một transaction/CAS trên row.
- Optimistic concurrency với row version: hữu ích khi business logic có read-modify-write; cần xác định conflict policy và retry.
- Serial queue per route: giảm cạnh tranh nhưng vẫn cần ownership/failure handling khi consumer chết và redelivery.

## 7. Wrong / tempting fixes
- Tăng TTL: giảm tần suất nhưng không loại bỏ pause dài, network delay hay GC stall.
- Chỉ dùng mutex trong một process: không đồng bộ các instance khác.
- Kiểm tra “lease còn hạn” tại worker trước khi ghi: có TOCTOU giữa check và write.
- Chỉ dùng distributed lock mà không có bảo vệ ở resource: lease hết hạn không ngăn stale writer đã bắt đầu.
- Retry write bị từ chối mà không tái tính kế hoạch: có thể tiếp tục cố ghi dữ liệu cũ.

## 8. Production implications
Fencing token phải tăng đơn điệu trong phạm vi resource và không reset khi process restart. Store phải duy trì token đã chấp nhận bền vững. Cần metrics `stale_write_rejected`, `lease_expired_inflight`, tracing với route ID không chứa PII và cảnh báo cho tác vụ vượt TTL.

## 9. Trade-offs and scope limits
Reference chặn **epoch cũ sau khi epoch mới đã commit**. Nó **không** chứng minh mọi stale write đều bị chặn ngay sau lease expiry nếu epoch mới chưa từng được ghi vào store. Để bảo đảm mạnh hơn, storage cần biết epoch quyền sở hữu hiện hành qua một authority chung hoặc cập nhật epoch mới atomically trước khi cho owner mới chạy. In-memory lock trong lab chỉ mô phỏng atomic compare-and-set, không đại diện cho transaction phân tán.

## 10. What a Senior engineer should notice
Phân biệt mutual exclusion lúc cấp quyền, ownership lifetime, write authorization tại resource và idempotency của retry. Định nghĩa invariant cụ thể: sau khi store đã chấp nhận epoch N, không write nào từ epoch < N được thay thế bản ghi. Kiểm thử bằng thứ tự thực thi được điều khiển, không phụ thuộc sleep hoặc random scheduling.
