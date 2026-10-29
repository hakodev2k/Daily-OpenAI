# Reference Solution — spoiler

**Chỉ xem sau khi reproduce, ghi hypothesis và thử sửa.**

## 1. Symptoms

Các request quanh ngày 2026-11-01 cho ra UTC lệch 60 phút, hoặc bị chấp nhận dù thiếu thông tin cần thiết; request thông thường phần lớn đúng.

## 2. Evidence

`night-101` và `night-102` có cùng wall-clock 01:30 nhưng offset khác nhau. `night-103` không có offset. `day-302` mang offset không phù hợp. `spring-201` là thời điểm không tồn tại và starter đã từ chối đúng.

## 3. Root cause

Starter chuyển `DateTime` local sang UTC mà không áp dụng hay kiểm tra `UtcOffsetMinutes`. Trong DST fall-back, 01:30 xuất hiện hai lần; API chuyển đổi không thể suy ra instant đối tác muốn khi chỉ nhận wall-clock. Với local time không ambiguous, bỏ qua offset cũng cho phép dữ liệu sai đi qua.

## 4. Why the fix works

Kiểm tra `IsInvalidTime` trước; với `IsAmbiguousTime`, buộc offset rõ ràng và thuộc tập `GetAmbiguousTimeOffsets`; với local time thường, xác minh offset (nếu có) bằng `GetUtcOffset`. Sau đó tạo `DateTimeOffset(local, selectedOffset)` và gọi `ToUniversalTime()`. Reference code: [WindowScheduler.cs](WindowScheduler.cs).

## 5. How to verify

Chỉ sửa `starter/WindowScheduler.cs`, chạy `./verify.ps1` tại thư mục unit; cần 7 PASS, exit code 0. Reference code không nằm trong project được build; đừng dùng kết quả compile reference để thay thế kiểm tra starter.

## 6. Alternative fixes

Đối tác có thể gửi ISO-8601 timestamp có offset ngay từ nguồn, hoặc gửi UTC instant kèm timezone metadata. Với recurring schedules, vẫn cần lưu timezone ID và local scheduling policy; chỉ lưu UTC không đủ mô tả lịch tái diễn.

## 7. Wrong / tempting fixes

- Cộng hoặc trừ cứng 60 phút: chỉ đúng tình cờ ở một thời điểm và làm sai mùa khác.
- Dùng `DateTime.Now` hoặc timezone của server: làm kết quả phụ thuộc máy triển khai.
- Luôn chọn standard time cho mọi trường hợp: không đáp ứng ý định của ca đầu trong giờ trùng.
- Bỏ qua offset đối tác vì 'timezone đã đủ': không đủ khi local time ambiguous và bỏ qua được dữ liệu mâu thuẫn.

## 8. Production implications

API cần thông báo lỗi validation rõ ràng, quan sát tỷ lệ request bị từ chối, tài liệu hóa quy tắc offset và lộ trình migration. Cân nhắc cập nhật timezone database; lịch đã lập nên lưu instant đã quyết định cùng input nguồn và version/policy nếu cần audit.

## 9. Trade-offs

Fail-closed có thể khiến đối tác legacy bị từ chối trong một số khung giờ nhưng ngăn lịch chạy sai; auto-select giảm friction nhưng che khuất ý định. Việc lưu UTC instant đơn giản cho one-off jobs, còn recurring wall-clock schedules cần timezone/policy.

## 10. Senior engineering judgment

Phân biệt wall-clock, offset, timezone ID và UTC instant; xác định chính sách khi input không thể ánh xạ duy nhất, không chỉ sửa một biểu thức. Kiểm thử cả hai lần xuất hiện của giờ trùng, giờ không tồn tại, offset sai và ngày bình thường. Tách logic chuyển đổi thuần khỏi side effects và lập kế hoạch rollout hợp đồng đối tác.
