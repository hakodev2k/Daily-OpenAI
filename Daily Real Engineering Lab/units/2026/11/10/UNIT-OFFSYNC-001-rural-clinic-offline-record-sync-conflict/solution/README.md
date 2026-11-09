# Reference solution — spoiler

## Root cause
Full-document last-write-wins dựa vào `clientUpdatedAt` không thể biểu diễn quan hệ nhân quả. Clock B chạy nhanh, nên snapshot từ revision 7 của B ghi đè allergy mà A đã cập nhật. Dù B1 chỉ có ý định đổi note, full-document PUT vẫn mang giá trị allergy cũ. B2 có thể đưa medication mâu thuẫn với allergy đã ghi trước đó. Retry A1 không được tạo thêm audit update.

## Thiết kế khuyến nghị
Dùng server-issued monotonic revision và optimistic concurrency trong transaction PostgreSQL. Client gửi patch với `baseRevision`, `operationId` (UUID), field mask và intent. Server kiểm tra authorization, unique key `(tenant_id,device_id,operation_id)`, đọc current revision với lock/conditional UPDATE, rồi ghi revision mới + audit trong cùng transaction. Nếu baseRevision cũ, trả 409 và conflict envelope; không âm thầm chấp nhận full-document overwrite. Khi conflict chứa allergy/medication, yêu cầu review bởi nhân sự được phân quyền, kể cả khi thay đổi ở các field khác có thể merge.

Ví dụ request:
```http
PATCH /visits/V-204
If-Match: "7"
Idempotency-Key: op-A1
Content-Type: application/json

{"baseRevision":7,"changes":{"allergy":"penicillin"}}
```
Response A1: `200 {"revision":8,"applied":true}`; retry A1 trả cùng kết quả (không tăng revision). B1/B2 có `baseRevision=7` trả 409; server không ghi đè allergy. Một 409 gồm `serverRevision:8`, `conflictFields`, `submittedChange`, `correlationId` và snapshot theo quyền. Reviewer quyết định merge và phát hành revision mới dựa trên current revision.

## Trade-offs
1. Strict CAS + manual review: dễ kiểm chứng, an toàn dữ liệu; tăng hàng đợi và độ trễ khi reconnect.
2. Field-level merge có vector/field revision: giảm conflict giả nhưng phức tạp schema và audit; tuyệt đối không tự merge medication/allergy nếu thiếu clinical policy.
3. CRDT cho toàn bộ document không phù hợp khi semantics y khoa đòi quyết định có trách nhiệm, dù hữu ích cho ghi chú cộng tác không nhạy cảm.

## Verification
- Replay 2 request đồng thời từ revision 7: đúng một commit, request còn lại 409.
- Retry cùng operationId: cùng response và audit count không đổi.
- Đồng hồ lệch 90 phút không đổi quyết định CAS.
- Out-of-order delivery và burst 50 rps vẫn giữ invariant mỗi revision có một predecessor.
- Auth/tenant isolation và audit redaction; rollout canary 5%→25%→100%, đo conflict/review age.
- Rollback về read-only hoặc disable write trên client cũ; không quay về unsafe last-write-wins.
## Wrong fixes
- NTP cho tablet: giảm lệch đồng hồ, không giải quyết concurrent write.
- Dùng serverReceivedAt thay clientUpdatedAt: vẫn mất update nếu full document overwrite.
- Retry 409 tự động với revision mới: biến xung đột thành silent overwrite.
- Chỉ lưu audit ngoài transaction: có thể có write không audit khi crash.
