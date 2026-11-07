# Reference Solution — inspect only after reproducing and attempting your own fix

## 1. Symptoms
Request trước outage hoạt động bình thường. Khi cache timeout ở request 4–5, starter ném exception dù origin vẫn khỏe.

## 2. Evidence
Hai request failure trùng với cache read timeout; authoritative origin không có failure.

## 3. Root cause
Cache đang bị coi như dependency bắt buộc trên critical success path. Cache read/write availability chưa được tách khỏi correctness của authoritative origin.

## 4. Why the fix works
Reference solution degrade riêng các TimeoutException đại diện cache unavailability: read failure chuyển sang origin; write failure sau khi có dữ liệu hợp lệ chỉ được ghi evidence. Cancellation không bị catch. Khi cache phục hồi, read path lại dùng cached value.

## 5. How to verify
Chạy `verify.ps1` trên learner-editable `starter/`. Kỳ vọng 8/8 request đúng, origin chỉ chịu thêm tải trong outage và recovery probe không tăng originCalls.

## 6. Alternative fixes
Circuit breaker, stale-data policy, rate limiting hoặc load shedding có thể phù hợp tùy capacity và consistency requirement.

## 7. Wrong / Tempting Fixes
- `catch (Exception)`: có thể nuốt cancellation hoặc programming errors.
- Retry cache vô hạn: tiêu thụ request deadline trong outage.
- Tăng timeout lớn: đổi fast failure thành tail-latency spike.
- Bỏ cache vĩnh viễn: mất lợi ích bình thường.
- Trả default content mà không đọc authoritative source: có thể sai correctness.

## 8. Production implications
Fallback làm tăng tải origin đúng lúc cache incident, vì vậy cần capacity planning, observability và bounded timeout/degradation policy.

## 9. Trade-offs
Fallback-to-origin tăng availability nhưng tăng origin load. Circuit breaker giảm pressure cache nhưng kéo dài origin fallback. Stale fallback giảm tải nhưng thay consistency semantics.

## 10. What a Senior engineer should notice
Optional dependency cần failure contract rõ ràng: xác định source of truth, recoverable errors, deadline budget, capacity và recovery behavior trước incident.
