# Reference Solution — chỉ xem sau khi tự reproduce và thử sửa

## Symptoms

QA điều khiển `ManualClock` nhưng `PassGate.Evaluate` vẫn trả `NotYetValid` cho vé hợp lệ trong thời gian mô phỏng. `DiagnosticNowUtc` hiển thị mốc giả lập, khiến chẩn đoán càng khó.

## Evidence

`harness/Program.cs` đặt mốc test tương lai hai ngày, nên đồng hồ thực vẫn ở trước `ValidFromUtc`. `DiagnosticNowUtc` lấy thời gian từ dependency, trong khi `Evaluate` không lấy từ cùng nguồn. Test không dùng sleep hoặc thay đổi giờ hệ điều hành.

## Root cause

`Evaluate` sử dụng trực tiếp `DateTimeOffset.UtcNow` thay vì `TimeProvider` đã inject. Service có hai nguồn thời gian, nên virtual-time replay không thể điều khiển quyết định.

## Why the fix works

Thay một dòng lấy `now` bằng `_clock.GetUtcNow()`. Bản tham chiếu đầy đủ nằm tại [PassGate.cs](PassGate.cs). Giữ nguyên kiểm tra `ValidFromUtc >= ExpiresAtUtc`, `now < ValidFromUtc`, và `now >= ExpiresAtUtc`. Hợp đồng là nửa mở `[start, end)`, không có một tick gia hạn ở mốc expiry.

## How to verify

Sửa file `starter/PassGate.cs` (không thay `harness/`), chạy `verify.ps1`. Test kiểm tra sáu điểm, gồm expiry boundary, invalid window và clock rewind. `reproduce.ps1` sau sửa sẽ không còn tìm thấy triệu chứng ban đầu.

## Alternative fixes

Có thể inject `Func<DateTimeOffset>` cho codebase nhỏ; tuy nhiên `TimeProvider` là abstraction chuẩn .NET 8, thuận lợi khi mở rộng sang timer/delay test. Với logic thuần túy, một cách khác là truyền `now` như tham số cho `Evaluate`, miễn là toàn bộ caller dùng một nguồn thời gian thống nhất.

## Wrong / tempting fixes

- Đổi `>=` thành `>`: làm vé còn hợp lệ tại đúng expiry, phá contract và không giải quyết clock mismatch.
- Thêm `Task.Delay` hoặc chỉnh đồng hồ OS: chậm, không ổn định và gây side effect.
- Đổi `DateTimeOffset` sang `DateTime` không có timezone: không xử lý vấn đề nguồn clock, còn tăng rủi ro diễn giải UTC.
- Hardcode `AddDays(2)` theo harness: chỉ khiến một fixture qua test, không phải production fix.

## Production implications

Inject `TimeProvider.System` qua composition root cho production; dùng fake/manual provider trong test. Đối với expiry tuyệt đối, UTC wall-clock là phù hợp, nhưng cần chính sách cho clock skew giữa các node. Với đo elapsed time và timeout, ưu tiên timestamp/elapsed API có semantics monotonic thay vì trừ wall-clock UTC.

## Trade-offs

Đưa clock thành dependency tăng một chút cấu hình DI nhưng giảm flaky tests và làm hành vi thời gian kiểm chứng được. Nếu domain có nhiều loại thời gian (business local time, UTC expiry, monotonic duration), định nghĩa rõ từng contract thay vì dùng một phép trừ DateTimeOffset cho mọi trường hợp.

## Senior engineer should notice

Không chỉ thay một dòng: cần bảo đảm một decision không trộn nhiều clock, test tại boundary chính xác, và phân biệt virtual clock test với production clock synchronization. Code review phải đánh giá contract, không chỉ kiểm tra test xanh.
