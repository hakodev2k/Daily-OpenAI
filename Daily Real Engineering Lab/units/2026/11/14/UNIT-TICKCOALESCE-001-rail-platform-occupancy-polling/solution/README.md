# Reference Solution — chỉ đọc sau khi reproduce và tự sửa

## Symptoms
Số lần camera được poll nhỏ hơn số cửa sổ 100 ms đã trôi qua.

## Evidence
Log starter có một chu kỳ camera chậm 460 ms. `expectedSlots` tính từ thời gian thực; số lần await hoàn thành ít hơn.

## Root cause
`PeriodicTimer` coalesces ticks: khi consumer chưa chờ, các ticks đến trong lúc xử lý không được queue thành từng sự kiện. Worker dùng số poll như số cửa sổ đã giám sát.

## Why the fix works
Giữ hai khái niệm độc lập: `observedSlots` (slot có observation) và `missedSlots` (slot đã trôi qua nhưng không được lấy mẫu). Đánh dấu slot bằng elapsed monotonic time, không tự điền camera samples. Với mỗi observation, tính slot index bằng elapsed/period; tăng missed cho khoảng trống kể từ slot trước; cuối run tính các slot còn lại đến expectedSlots. Khi chạy thực tế, nên dùng một clock abstraction để test deterministic và metric riêng cho độ trễ.

## How to verify
Chạy `./verify.ps1` sau khi sửa starter; kiểm tra summary và invariant. Kiểm thử bổ sung: không stall, stall nhiều slot, cancellation trước tick đầu, deadline đúng biên. PowerShell verification là smoke check, không thay thế các deterministic unit tests.

## Alternative fixes
Nếu yêu cầu bắt buộc một sample mỗi 100 ms, cần xử lý capacity và backpressure: scale camera gateway, giới hạn thời gian mỗi poll, hoặc chuyển sang producer có timestamp độc lập. Không thể tái tạo observation đã mất chỉ bằng timer.

## Wrong or misleading fixes
Tăng số lượng vòng lặp sau stall chỉ tạo poll trễ, không phải dữ liệu lịch sử; `Task.Run` không thay đổi contract tick; tăng CPU/pod không bảo đảm sampling SLO.

## Production implications
Đặt alert trên missed-slot ratio, p95 poll duration, camera timeout và observation freshness. Sử dụng monotonic clock cho elapsed intervals; UTC timestamps phục vụ audit.

## Trade-offs
Bỏ slot giữ tính thời gian thực nhưng giảm coverage; queue mọi slot tạo backlog và dữ liệu stale; song song hóa poll có thể vi phạm rate limit/camera ordering.

## Senior engineer should notice
SLO phải định nghĩa coverage theo *time windows*, không theo worker heartbeat. Phân biệt missing measurement với measurement bằng zero.

## Reference implementation sketch
```csharp
// Maintain lastAccountedSlot and compute slot = elapsedTicks / periodTicks.
// On each successful poll: missed += max(0, slot - lastAccountedSlot - 1);
// observed++; lastAccountedSlot = slot;
// At shutdown: missed += max(0, expectedSlots - lastAccountedSlot);
// Never invent an observation for an unobserved slot.
```
