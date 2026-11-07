# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms

Bản ghi cuối mất ghi chú từ tablet B; Status nhận một thay đổi từ snapshot cũ; retry không được phân biệt với thay đổi mới. `reproduce.ps1` chứng minh triệu chứng, không chứng minh hệ thống đã đúng.

## 2. Evidence

Theo timeline, B-201 sửa TechnicianNote trong khi A-101 chỉ sửa Status; hai thay đổi không cạnh tranh. B-202 sửa Status với BaseFieldRevision=0 dù Status đã lên 1. A-102 có BaseFieldRevision=1 nhưng timestamp cũ hơn.

## 3. Root cause

Starter dùng một mốc `LastClientTime` cho toàn Work Order làm tiêu chuẩn chọn bản ghi, dù đồng hồ thiết bị không đồng bộ. Không kiểm tra `BaseFieldRevision` theo field, không lưu danh tính change đã áp dụng. Vì thế vừa loại nhầm thay đổi độc lập, vừa nhận stale update, vừa thiếu idempotency.

## 4. Why the fix works

Xem [SyncEngine.cs](SyncEngine.cs). Mỗi field có revision riêng. Incoming change chỉ được áp dụng nếu revision client đọc bằng revision hiện tại của field. ChangeId đã chấp nhận được lưu cùng payload: retry giống hệt trả AlreadyApplied; reuse ID khác payload trả Conflict. Timestamp không quyết định thứ tự logic. Global Revision chỉ là chỉ số quan sát.

Để thử reference sau khi tự làm, có thể sao lưu `starter/SyncEngine.cs` rồi copy `solution/SyncEngine.cs` vào `starter/`, chạy `./verify.ps1`. Đừng thay Program.cs.

## 5. How to verify

Expected: A-101 Applied; B-201 Applied; B-202 Conflict; B-201 retry AlreadyApplied; A-102 Applied. Status=Completed, TechnicianNote=Valve seal damaged, Revision=3. Kiểm tra thêm ID reuse, unknown field, stale update và không mutation trên reject.

## 6. Alternatives

- **Whole-document optimistic concurrency:** đơn giản hơn nhưng hai field độc lập gây conflict giả; phù hợp tài liệu nhỏ, chỉnh sửa hiếm.
- **Field-level optimistic concurrency (tham chiếu):** hợp nhất độc lập, trả conflict khi cùng field stale; cần version per field và kiểm tra invariant xuyên field.
- **Server timestamp LWW:** dễ triển khai nhưng có thể làm mất update hợp lệ; chỉ phù hợp dữ liệu mà mất update được chấp nhận.
- **Operation log / CRDT:** hữu ích cho collaborative edits nhất định, nhưng không mặc nhiên giải quyết workflow transitions hoặc kiểm soát nghiệp vụ.

## 7. Wrong / tempting fixes

Tăng timeout/retry không sửa được chính sách conflict. Đồng bộ NTP tốt hơn không biến client time thành thứ tự giao dịch đáng tin cậy. Chỉ dùng một global version cho mọi field ngăn lost update nhưng làm tăng conflict không cần thiết. Dùng `ConcurrentDictionary` không bảo đảm atomic transaction giữa field update và dedup record.

## 8. Production implications

Fixture chỉ chạy trong một process. Trong production cần lưu `(TenantId, WorkOrderId, FieldName, FieldRevision)` và `(TenantId, ChangeId, PayloadHash, Outcome)` trong storage bền vững, cùng transaction với mutation. CAS/unique constraint bảo vệ race; chỉ trả Applied sau commit. Tái sử dụng change ID khác payload phải bị từ chối. Audit và tenant isolation là bắt buộc.

## 9. Trade-offs

Field-level revisions tăng schema và logic conflict, nhưng giảm false conflicts cho ghi chú và trạng thái độc lập. Với Status, validation state machine phải chạy trong cùng transaction; hai field có quan hệ nghiệp vụ có thể cần một aggregate-level version hoặc command model thay vì merge tự động.

## 10. What a Senior engineer should notice

Cần xác định chính xác **đơn vị concurrency**, **nguồn thứ tự đáng tin**, **phạm vi idempotency**, **transaction boundary**, **response contract** và **quy tắc nghiệp vụ xuyên field**. Một test xanh trên in-memory không chứng minh crash recovery hay multi-instance correctness; cần integration tests với database thật và concurrent requests.
