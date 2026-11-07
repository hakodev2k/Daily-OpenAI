# Reference Solution — UNIT-TRACE-001

**Spoiler: chỉ đọc sau khi tự reproduce và thử fix.**

## 1. Symptoms
Business trả `accepted`, nhưng `dispatch` span kết thúc trước `gateway-send`, khiến latency end-to-end bị đo thiếu.

## 2. Evidence
Timeline baseline: `START dispatch → STOP dispatch → START gateway-send → STOP gateway-send`. Quan hệ TraceId/ParentSpanId vẫn đúng, nhưng quan hệ cha-con không bảo đảm parent bao trùm thời gian của child.

## 3. Root cause
`DispatchAsync` trả `Task` chưa hoàn tất ngay trong `using`. Scope bị dispose khi phương thức trả Task, không phải khi gateway xử lý xong. Gate bảo đảm chứng cứ về thứ tự, không phụ thuộc tốc độ máy.

## 4. Why the fix works
Dùng `async Task<DispatchResult>` và `return await _gateway.SendAsync(specimenId).ConfigureAwait(false)` bên trong `using`. Scope chỉ kết thúc sau khi await hoàn tất, kể cả đường exception. Xem [reference source](DispatchService.cs), sau đó áp dụng thay đổi vào `starter/DispatchService.cs`.

## 5. Verification
Chạy `.\verify.ps1` trên learner-edited starter. Probe yêu cầu parent-child đúng, nghiệp vụ không đổi và `START dispatch → START gateway-send → STOP gateway-send → STOP dispatch`. `reproduce.ps1` dùng để chứng minh lỗi baseline; sau khi sửa, nó không còn được kỳ vọng PASS.

## 6. Alternative fixes
Đưa toàn bộ công việc vào một `async` method có span scope bao trùm `await`. Nếu cố ý fire-and-forget, cần tách transaction và span cho background operation với durable handoff, correlation và ownership riêng.

## 7. Wrong / tempting fixes
- Tăng timeout hoặc thêm `Task.Delay` chỉ che symptom.
- Gán lại ParentSpanId: topology vốn đã đúng, lifetime mới sai.
- Xóa span gateway: làm mất evidence.
- Chỉ thêm `Stopwatch` tại controller: metric khác không sửa trace semantics.

## 8. Production implications
Trace sai làm p95 của bước dispatch trông quá tốt, gây sai chẩn đoán, sampling và alerting. Với background work phải phân biệt `accepted` và `completed`.

## 9. Trade-offs
Chờ gateway giúp đo end-to-end nhưng tăng response latency. Nếu cần 202 sớm, dùng queue bền vững và span riêng; chỉ ACK sau khi đã nhận trách nhiệm xử lý.

## 10. Senior engineering judgment
Đừng kết luận trace đúng chỉ vì SpanId hợp lệ. Kiểm tra semantic boundary, lifecycle resource, Task completion và telemetry theo cùng một timeline.
