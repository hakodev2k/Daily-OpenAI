# Reference decision — chỉ đọc sau khi tự làm

## Capacity replay
Peak = 1,800 / 60 = **30 attempts/s**. Với mean 12 MiB, payload ingress = **360 MiB/s** = 360 × 1,048,576 × 8 / 1e9 = **3.02 Gbit/s**, lớn hơn budget API inbound **1.2 Gbit/s** khoảng 2.5 lần, chưa tính TLS/protocol/retries. Đây là throughput demand ước tính, không phải throughput đạt được. 2 GiB / 40 MiB = **51** buffered uploads tối đa về mặt số học, thực tế thấp hơn do overhead và các workload khác. 7,200 / 600 = 12 attempts/s average, nên sizing theo mean che mất burst.

## Recommended conditional design
Ưu tiên **direct-to-Blob staged upload** nếu staging chứng minh Blob capacity, client compatibility và token issuance đáp ứng peak. API tạo upload intent (authenticated + enrollment + quota + deadline policy), cấp short-lived SAS hoặc user-delegation SAS chỉ ghi object staging duy nhất, không có list/read toàn container. Browser upload block/resumable, có checksum. API finalize sau upload: đọc blob properties server-side, kiểm tra ownership/attempt, content length, hash và deadline theo chính sách công bố; ghi nhận trạng thái idempotent bằng DB transaction/unique key. Chỉ chuyển sang ACCEPTED sau policy validation và malware scan nếu quy định bắt buộc; trả receipt ID và timestamp từ server. Không coi HTTP 201 từ Blob là nộp bài thành công.

## Security and failure policy
Không tin filename, MIME hay timestamp phía client. Chống overwrite bằng object version/ETag hoặc unique immutable attempt key; không cấp token có quyền vượt phạm vi. Scan trong vùng quarantine, không expose file cho giám khảo trước khi clean. Scanner outage: giữ PENDING_SCAN, không silent accept; deadline receipt semantics phải được thống nhất với nghiệp vụ. Cleanup staging orphan sau grace period, retention 30 ngày từ mốc hợp lệ và audit immutable receipt.

## Alternatives and reversal triggers
**Streaming API proxy** giảm RAM nhưng vẫn đi qua API ingress, nên không giải quyết giới hạn 1.2 Gbit/s hiện tại nếu peak thực sự đạt 3.02 Gbit/s. Có thể phù hợp khi file nhỏ, traffic thấp hoặc network/security policy cấm client direct. **Dedicated resumable gateway** hỗ trợ client đặc thù nhưng tăng vận hành và capacity management. Direct Blob tăng complexity ở SAS issuance, CORS, orphan cleanup và finalize reconciliation; nếu Blob throttling hoặc thiết bị client không tương thích, cần benchmark rồi cân nhắc gateway.

## Acceptance / verification matrix
- 30 attempts/s với phân bố size p50/p95: không vượt error budget; đo p95 finalize và accepted receipts.
- Duplicate finalize cùng attempt: một accepted submission, receipt ổn định.
- 41 MiB: reject, không có accepted receipt.
- Wrong student/object, expired/overprivileged token: deny.
- Network interruption: resumable/retry không tạo accepted duplicates.
- Scanner outage: PENDING_SCAN rõ ràng, có alert và recovery.
- Upload bắt đầu trước deadline nhưng finalize sau deadline: xử lý theo **chính sách nghiệp vụ công bố** (không tự giả định), test boundary theo server time.
- Staging orphan: được thu hồi, không xóa bài accepted.

## Wrong fixes
- **Tăng replica API** mà không nâng shared ingress: không sửa bottleneck chung.
- **Tăng timeout lên 10 phút**: kéo dài kết nối, không sửa throughput và memory.
- **Tin client timestamp**: cho phép backdate submission.
- **SAS quyền write toàn container**: mở rộng blast radius.
- **Đánh dấu accepted ngay sau upload**: bỏ qua kiểm tra quyền, scan và race.

## Decision quality
Một câu trả lời chọn streaming proxy cũng có thể đúng nếu chứng minh peak demand thấp hơn, ngân sách ingress được nâng, streaming end-to-end và vận hành được kiểm thử. Chấm điểm dựa trên evidence, risk và trade-off chứ không chỉ công nghệ.
