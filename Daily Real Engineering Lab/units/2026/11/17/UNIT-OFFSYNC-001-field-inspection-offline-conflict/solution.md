# Reference Solution — Spoiler

## Replay policy hiện tại
Ban đầu v10 = crack 4.0, photos [p0], closure false. op-a được áp dụng thành v11: crack 7.2, photos [p0,p1], closure true. op-b là snapshot dựa trên v10 nhưng đến sau, bị LWW ghi đè thành v12: crack 4.0, photos [p0], closure false; làm mất ảnh p1 và vi phạm safety invariant. Retry op-b lại ghi v13 dù cùng operationId (payload notes thay đổi), gây audit trùng. op-c dựa trên v11 đến sau thành v14, thêm p2 và closure true nhưng không chữa được khoảng thời gian dashboard hiển thị sai.

## Phương án
- LWW: triển khai nhanh, nhưng không đáp ứng safety/lost-update; chỉ dùng cho dữ liệu không quan trọng, có thể tái tạo.
- Optimistic concurrency: dùng `UPDATE ... WHERE version = @baseVersion`; khi 0 row affected trả HTTP 409 với currentVersion và snapshot được phép xem. Dễ chứng minh correctness nhưng nhiều conflict cần manual review, nhất là offline dài.
- Field-level merge: patch dựa trên per-field revision; chỉ auto-merge khi các field không giao nhau và rule nghiệp vụ cho phép. `photoIds` là tập append-only với kiểm tra quyền và hash; không xóa ảnh chỉ do snapshot cũ. `closureRequired` là safety-latched: true có thể tự nâng mức, false chỉ được chuyển sau workflow reopen có chữ ký/quyền. Hai thay đổi cùng field hoặc bất đồng đo đạc phải vào conflict queue; không dùng device timestamp để chọn người thắng.

## Recommended contract
`POST /inspections/{id}/operations` với `operationId` UUID, `baseVersion`, typed patch, `deviceId`. Server transaction: xác thực tenant/role; tìm operationId đã xử lý (unique constraint theo tenant + operationId); nếu trùng trả kết quả lưu trước đó, nếu payload hash khác trả 409 operation-id-reuse; kiểm tra version/field revisions và safety rule; ghi inspection, operation receipt và append-only audit trong cùng transaction. Khi conflict trả 409 cùng `currentVersion`, danh sách field conflict đã lọc theo quyền, `resolutionId`; không âm thầm bỏ update. Không mở quyền ảnh chỉ nhờ biết ID.

## Verify bằng replay
- op-a được áp dụng và closure=true.
- op-b từ base v10 bị conflict; closure không trở về false, p1 không biến mất.
- retry op-b trả cùng receipt nếu payload byte-semantic hash giống; dữ liệu khác cùng operationId phải bị từ chối, không tạo audit thứ hai.
- op-c từ base v11 có thể merge p2 nếu field-level revision cho phép, nhưng crack và closure phải được đánh giá theo policy; không dùng `baseVersion` đơn lẻ để tự cho phép toàn bộ snapshot.
- Test đảo thứ tự đến, duplicate, 72h offline, clock skew, 2 người sửa crack cùng lúc, unauthorized image, rollback và race của hai server nodes.

## Wrong fixes
1. Dùng `updatedAt` của điện thoại: clock drift và offline làm timestamp không đáng tin.
2. Chỉ thêm Redis lock: không tạo conflict semantics hoặc idempotency bền vững.
3. Dùng `SERIALIZABLE` nhưng vẫn cho stale snapshot ghi đè: isolation không thay thế business validation.
4. Merge mọi field bằng union: số đo và quyết định đóng cầu không có phép union an toàn.

## Trade-offs và rollout
Ưu tiên optimistic concurrency + safety latch + operation receipts trước, sau đó bổ sung field merge cho photo append-only. Dùng PostgreSQL hiện có, tránh thêm hạ tầng vượt ngân sách. Shadow-evaluate policy trên replay, đo conflict rate, duplicate suppression, stale rejection, time-to-sync p95, manual-review backlog, safety override attempts; canary theo đội kiểm định. Rollback phần auto-merge nhưng giữ immutable operation log; xử lý conflict bằng workflow, không rollback dữ liệu an toàn đã được xác nhận.

## Concepts after lab
Optimistic concurrency bảo vệ update khỏi snapshot cũ; idempotency bảo vệ retry; business invariant bảo vệ trạng thái nguy hiểm. Ba cơ chế độc lập và đều cần thiết.
