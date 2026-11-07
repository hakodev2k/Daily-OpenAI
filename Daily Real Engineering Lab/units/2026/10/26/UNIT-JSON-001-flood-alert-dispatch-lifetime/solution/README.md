# Reference Solution — chỉ đọc sau khi reproduce và thử fix

## 1. Symptoms
Hai bulletin JSON hợp lệ, `accepted=2` nhưng `delivered=0`, `failed=2`; dispatch log ghi `ObjectDisposedException`.

## 2. Evidence
`FloodAlertIntake.IsValidControl` đọc cùng dữ liệu **bên trong** scope `JsonDocument` và pass. `Accept` trả về một `JsonElement` rồi thoát scope; dispatch đọc lại payload **sau đó**.

## 3. Root cause
`JsonElement` là một value type nhưng bản copy thông thường vẫn tham chiếu dữ liệu thuộc `JsonDocument` sở hữu. `using var document` dispose document khi `Accept` kết thúc. `PendingAlert.Alert` không phải bản snapshot độc lập; đọc nó ở giai đoạn dispatch dẫn tới `ObjectDisposedException`.

## 4. Why the fix works
Trước khi rời owner scope, gọi `alert.Clone()` để tạo một `JsonElement` có dữ liệu độc lập, an toàn khi document ban đầu bị dispose. Thay đổi đúng một dòng trong `starter/Program.cs` tại boundary tạo `PendingAlert`. Không cần giữ document sống xuyên qua queue.

## 5. How to verify
Chạy `./verify.ps1` trên **starter đã sửa**; phải có `accepted=2 delivered=2 failed=0 controlValidated=2` và đúng hai payload:
- `F-101|Delta|high`
- `F-102|Harbor|critical`

Tùy chọn: `dotnet run --project ./solution/FloodAlertLab.csproj -- --assert-fixed` để self-check reference riêng biệt; không thay thế learner verification.

## 6. Alternative fixes
- Deserialize sang typed immutable DTO trước khi queue: thường tốt hơn khi contract ổn định, hỗ trợ validation và giảm JSON DOM coupling.
- Giữ `JsonDocument` sống cho tới khi consumer xử lý: có thể hợp lệ nhưng yêu cầu ownership transfer và disposal nghiêm ngặt, khó bảo trì trong asynchronous queue.

## 7. Wrong / tempting fixes
- `try/catch` rồi retry cùng `JsonElement`: owner vẫn đã dispose, retry không giải quyết nguyên nhân.
- Bỏ `using` mà không thiết kế ownership: che triệu chứng nhưng gây memory/resource retention.
- `var copied = alert;`: chỉ copy struct, không tách lifetime của dữ liệu.
- Chuyển queue sang Redis/Service Bus: không giải quyết lifetime của object trong producer.

## 8. Production implications
Payload đi qua async boundary, cache, event bus, callback hoặc background worker cần có ownership contract rõ. Theo dõi accepted/delivered/failed bằng cùng correlation ID; không log nội dung cảnh báo nhạy cảm không cần thiết.

## 9. Trade-offs
`Clone()` tạo snapshot với chi phí memory/CPU theo kích thước JSON. DTO projection thường gọn và an toàn hơn nếu chỉ cần `zone` và `severity`. Đo kích thước và throughput trước khi tối ưu.

## 10. What a Senior engineer should notice
API trả về value type không bảo đảm dữ liệu deep-owned. Phân biệt copy của wrapper với copy dữ liệu; kiểm thử qua boundary nơi tài nguyên gốc đã bị giải phóng, không chỉ unit test trong cùng scope.

## Regression extension
Thêm một payload có nested object/array, xác nhận consumer đọc được sau khi producer kết thúc; thêm kiểm tra malformed JSON phải bị reject ngay tại intake thay vì ghi accepted giả.
