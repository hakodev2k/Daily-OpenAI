# Reference Solution — Spoiler

Chỉ đọc sau khi đã reproduce, ghi giả thuyết và thử sửa `starter/LatencyPolicy.cs`.

## 1. Symptoms
Gateway phân loại 751 ms và 1900 ms là chậm nhưng bỏ qua 1200 ms và 2500 ms, dù budget là 750 ms. Đây là vi phạm tính đơn điệu của bộ phân loại SLA.

## 2. Evidence
Fixture cho kết quả `SLOW_1200_FLAGGED=False`, `SLOW_1900_FLAGGED=True`, `SLOW_2500_FLAGGED=False`. Không có queue backlog, lỗi HTTP hoặc lỗi clock được mô phỏng. Dữ liệu đủ để bác bỏ giải thích đơn thuần rằng đối tác lúc nhanh lúc chậm: duration đã được đo trước khi phân loại.

## 3. Root cause
`TimeSpan.Milliseconds` trả về **thành phần milliseconds trong một giây** (giá trị từ -999 đến 999), không phải toàn bộ số milliseconds đã trôi qua. Ví dụ duration 1200 ms có `Milliseconds == 200`, 1900 ms có `Milliseconds == 900`, 2500 ms có `Milliseconds == 500`. So sánh các thành phần này với budget 750 ms tạo ra dạng sawtooth theo từng giây.

## 4. Why the fix works
Reference so sánh trực tiếp `elapsed > budget`. `TimeSpan` so sánh tổng số ticks, bảo toàn cả phần giây và độ phân giải dưới millisecond, đồng thời thể hiện đúng quy tắc *strictly greater than*. Các kiểm tra input được giữ nguyên.

## 5. How to verify
Thay nội dung `starter/LatencyPolicy.cs` bằng [reference](LatencyPolicy.cs), giữ nguyên các file khác và chạy `./verify.ps1`. Bộ 17 hợp đồng bao gồm ranh giới 750 ms, duration trên một giây, budget 1500 ms, duration một phút, chính xác 1 tick và dữ liệu không hợp lệ. `reproduce.ps1` vẫn chứng minh baseline bất biến.

## 6. Alternative fixes
- Dùng `elapsed.TotalMilliseconds > budget.TotalMilliseconds` cho cùng ngữ nghĩa với các giá trị thông thường; chuyển sang `double` không cần thiết và có thể làm mất độ chính xác ở biên cực nhỏ.
- Dùng `elapsed.Ticks > budget.Ticks` nếu hợp đồng yêu cầu so sánh tick rõ ràng, nhưng kém diễn đạt ý định hơn so sánh trực tiếp `TimeSpan`.
- Nếu duration đến từ upstream dưới dạng integer milliseconds, chuẩn hóa thành `TimeSpan` tại boundary và giữ cùng một representation trong policy.

## 7. Wrong or tempting fixes
- Tăng SLA lên 2000 ms chỉ che triệu chứng và không sửa sai lệch phân loại.
- Cộng thêm `Seconds * 1000` rồi `Milliseconds` là một bản vá mong manh vì bỏ sót phút/giờ và sign.
- Dùng `>=` đổi business contract ở điểm bằng budget.
- Tăng số instance, retry HTTP hoặc chỉnh queue không tác động tới phép tính đang sai.

## 8. Production implications
Phân loại sai làm méo alerting, SLO burn rate, dashboard và quyết định mở incident. Hãy kiểm tra pipeline đo duration bằng monotonic clock (`Stopwatch` hoặc `TimeProvider.GetTimestamp`) và tránh suy ra latency từ chênh lệch wall-clock nếu clock có thể nhảy.

## 9. Trade-offs
So sánh `TimeSpan` trực tiếp đơn giản và chính xác; chuyển sang số floating-point có thể tiện cho biểu đồ nhưng không nên là primitive ra quyết định ở ranh giới SLA. Nếu metric backend chỉ nhận milliseconds dạng số, chuyển đổi tại export boundary, không làm biến dạng policy.

## 10. What a Senior engineer should notice
Một invariant mạnh hơn từng ví dụ là **monotonicity**: cùng budget, nếu duration A đã vi phạm thì mọi duration B > A cũng phải vi phạm. Thêm property-based hoặc table-driven tests quanh các mốc 1 s, 60 s và biên tick; kiểm tra nhãn SLA trước khi sử dụng để kích hoạt retry hay cắt lưu lượng.
