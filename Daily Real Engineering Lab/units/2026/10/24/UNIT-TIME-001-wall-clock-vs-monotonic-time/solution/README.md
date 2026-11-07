# Reference Solution — chỉ xem sau khi tự sửa

## 1. Symptoms
Batch 10 giây bị kết thúc ở giây 4 hoặc còn active sau giây 12 khi host time thay đổi.

## 2. Evidence
Control cases không điều chỉnh giờ đều PASS. Forward và backward adjustment thất bại theo hai hướng đối nghịch. Harness đo elapsed timestamp độc lập với UTC wall time.

## 3. Root cause
Starter lấy hiệu `GetUtcNow()` để đo duration. UTC wall clock phù hợp cho instant/log/lịch nhưng có thể nhảy khi đồng bộ thời gian. Hiệu hai instant không bảo đảm elapsed duration.

## 4. Why the fix works
Lưu `GetTimestamp()` lúc bắt đầu, rồi dùng `GetElapsedTime(start)` để so với budget. Monotonic elapsed không đổi khi UTC wall clock bị chỉnh.

## 5. How to verify
Thay logic `starter/LeaseWatchdog.cs` bằng [reference source](LeaseWatchdog.cs) hoặc giải pháp tương đương. Chạy `./verify.ps1`: 5 PASS, exit 0. `./reproduce.ps1` cố tình tìm lỗi starter nên không dùng để đánh giá fix.

## 6. Alternative fixes
`Stopwatch` cũng phù hợp đo elapsed trong một process; `TimeProvider` giúp deterministic tests không cần `Thread.Sleep`. Với deadline cần persist qua restart, thiết kế một cơ chế khác, không persist raw monotonic timestamp.

## 7. Wrong or tempting fixes
Tăng budget chỉ che triệu chứng; `Math.Abs(now-start)` biến clock jump thành false timeout; `DateTime.Now` còn chịu timezone/DST; đồng bộ NTP thường xuyên không biến wall clock thành monotonic. Wall clock vẫn đúng cho calendar deadline nếu requirement thực sự là calendar time.

## 8. Production implications
Dùng monotonic duration cho local timeout, UTC timestamps cho log correlation. Không so raw timestamps giữa hai process hoặc sau restart. Distributed lease cần authority và fencing semantics riêng.

## 9. Trade-offs
Monotonic clock không cung cấp giờ dân sự; absolute expiry có ích trong distributed coordination nhưng phải xử lý skew và lease ownership. Timeout cục bộ không thay thế distributed fencing token.

## 10. Senior engineer should notice
Hai triệu chứng đối nghịch là evidence quan trọng. Regression suite phải có forward/backward jumps, boundary equality, rearm. Tránh test dựa vào NTP thật hoặc `Thread.Sleep`.
