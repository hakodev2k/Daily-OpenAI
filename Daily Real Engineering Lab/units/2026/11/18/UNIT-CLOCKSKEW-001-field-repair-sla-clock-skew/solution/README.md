# Reference solution — spoiler

## Root cause
Wall clock (DateTimeOffset.UtcNow) có thể được hiệu chỉnh; hiệu hai timestamp không bảo đảm elapsed duration monotonic. NTP và VM clock correction có thể tạo duration âm.

## Fix
Đo duration bằng monotonic timer, giữ UTC timestamps riêng cho audit. Production có thể dùng `Stopwatch.GetTimestamp()` và `Stopwatch.GetElapsedTime(start,end)`. Reference dùng scripted monotonic ticks để deterministic replay, 125ms.

## Verification
`./verify.ps1 ./solution/ClockLab.csproj` phải PASS. `./reproduce.ps1` phải xác nhận starter FAIL.

## Wrong fixes
`Math.Abs(duration)` và `Math.Max(0,duration)` che sai số; đổi sang `DateTime.UtcNow` vẫn là wall clock.

## Trade-offs
Monotonic ticks chỉ có ý nghĩa trong phạm vi phù hợp của clock/process, không dùng để so timestamp giữa máy hoặc sau restart. Audit vẫn cần UTC và clock-sync observability. Khi work throw, thu duration trong finally.

## Senior challenge
Tách metric `processing_duration_ms` khỏi `completed_at_utc`, thiết kế regression cho wall clock tiến/lùi và exception path.
