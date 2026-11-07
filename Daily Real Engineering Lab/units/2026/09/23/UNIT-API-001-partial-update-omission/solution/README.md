# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms
Client chỉ đổi DisplayName, nhưng EmailEnabled=true trở thành false.

## 2. Evidence
Không có exception. Request object luôn chứa một giá trị cho EmailEnabled, kể cả khi client không có ý định cập nhật field đó.

## 3. Root cause
DTO dùng non-nullable bool. Ở boundary partial update, trạng thái omitted bị collapse thành default false. Update logic vì vậy không thể phân biệt không gửi với gửi false.

## 4. Why the fix works
bool? dùng null để biểu diễn absence và giữ true/false cho explicit value. Apply logic chỉ mutate entity khi request có value.

## 5. How to verify
Case A: chỉ đổi tên → EmailEnabled giữ nguyên. Case B: gửi explicit false → setting chuyển thành false.

## 6. Alternative fixes
JSON Patch, presence-aware wrapper, command model có explicit field-presence metadata, hoặc endpoint command-specific. Chọn theo API contract và độ phức tạp domain.

## 7. Wrong or misleading fixes
- Bỏ assignment của EmailEnabled: hết bug hiện tại nhưng client không thể update field.
- Nếu false thì ignore: làm explicit false không còn khả thi.
- Đọc entity lại sau update: không khôi phục được intent đã mất ở request boundary.

## 8. Production implications
Silent data corruption nguy hiểm hơn validation failure vì request vẫn báo success. Contract tests nên cover omitted, explicit false, explicit true và unknown fields theo policy.

## 9. Trade-offs
Nullable DTO đơn giản khi domain field không nullable; nếu domain tự thân cho phép null thì cần representation khác để phân biệt explicit null với omission.

## 10. What a Senior engineer should notice
Partial update là bài toán về presence semantics, không chỉ mapping DTO. Contract phải giữ đủ thông tin để application layer quyết định field nào thực sự được client yêu cầu mutate.
