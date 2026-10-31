# Reference Solution — spoiler

## 1. Symptoms
Tác vụ elapsed 250ms có thể báo duration âm hoặc kéo dài hàng chục giây. UTC audit instants vẫn hiện diện.

## 2. Evidence
Ba fixture cùng elapsed, CPU và queue ổn định. Step UTC -30s cho -29750ms, step +45s cho 45250ms. Sai lệch có hướng và bằng clock adjustment.

## 3. Root cause
'MaintenanceProbe' lấy chênh lệch 'GetUtcNow()' để đo duration. UTC wall clock mô tả mốc tuyệt đối, có thể được điều chỉnh; không bảo đảm đơn điệu.

## 4. Why fix works
[Reference source](JobProbe.cs) lưu UTC instants cho audit nhưng dùng 'TimeProvider.GetTimestamp()' và 'GetElapsedTime()' của cùng provider để đo duration. Clock step không làm thay đổi monotonic elapsed.

## 5. How to verify
Chỉ sau khi tự sửa, có thể chép reference vào 'starter/JobProbe.cs' và chạy './verify.ps1'. Ba duration đều 250ms, audit UTC vẫn phản ánh fixture. Verify luôn chạy learner-editable starter, không chạy solution.

## 6. Alternative fixes
'Stopwatch.StartNew()' cũng phù hợp cho elapsed cục bộ nhưng khó kiểm thử clock-step xác định nếu không inject clock. 'TimeProvider' giúp tách hai clock domains trong cùng abstraction. Với business cutoff tuyệt đối, vẫn dùng UTC instants.

## 7. Wrong / tempting fixes
'Math.Abs(duration)' che lỗi và không xử lý step tiến; clamp âm về 0 cũng sai. Scale worker, tăng timeout không giải quyết semantics. Sửa audit FinishedAtUtc theo elapsed làm sai dữ liệu đối soát.

## 8. Production implications
UTC timestamp phục vụ audit và correlation; monotonic elapsed phục vụ SLA và latency. Monotonic timestamps không phải UTC và không thể trừ giữa hai host/process khác nhau.

## 9. Trade-offs
Đo per-hop duration bằng monotonic clock; distributed trace cần correlation và xem xét clock skew. Với deadline UTC nghiệp vụ, so sánh UTC theo chính sách đã xác định.

## 10. Senior engineer should notice
Trước khi tối ưu throughput, xác minh ý nghĩa metric. Hai invariants audit và SLA độc lập. Regression phải bao gồm step tiến và lùi, không chỉ happy path.

## 11. Requirement change
Khi có cutoff 18:00Z, giữ riêng điều kiện deadline UTC và elapsed SLA. Không cố dùng một phép đo cho cả hai.
