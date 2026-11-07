# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, ghi hypothesis và tự thử fix.

## 1. Symptoms
Hàng đợi chứa bốn action; sau khi Drain, cả bốn thông báo lại có cùng PatrolId/RecipientId của mục cuối.

## 2. Evidence
`pendingCorrect=True` và `noEarlySend=True`: số lượng enqueue không sai. Mismatch chỉ xuất hiện sau khi worker gọi các delegate. `reproduce.ps1` luôn phát hiện symptom từ baseline cố định.

## 3. Root cause
Biến cục bộ `selected` được khai báo **ngoài** vòng lặp rồi gán lại. Các lambda được enqueue đóng trên **cùng một biến**; chúng đọc biến tại thời điểm thực thi, sau khi vòng lặp đã gán giá trị cuối. Đây không phải lỗi `foreach` tự động capture sai biến iteration trong C# hiện đại; chính biến ngoài vòng lặp mới là nguồn lỗi.

## 4. Why the fix works
Mỗi vòng lặp giữ một command ổn định trước khi tạo delegate. Xem [reference source](NotificationBatchScheduler.cs). Mỗi deferred action sau đó gắn với payload riêng, không còn phụ thuộc vào một biến bị ghi đè.

## 5. How to verify
Chép nội dung `solution/NotificationBatchScheduler.cs` vào `starter/NotificationBatchScheduler.cs` **chỉ sau khi tự làm**. Chạy `./scripts/verify.ps1` và kiểm tra bốn recipient theo thứ tự. Chạy lại `./scripts/reproduce.ps1` để xác nhận fixture gốc vẫn lỗi.

## 6. Alternative fixes
- Enqueue một `PatrolNotice` bất biến vào typed queue, worker gọi gateway sau; ranh giới dữ liệu rõ hơn.
- Tạo delegate bằng một factory nhận payload như tham số; tránh capture biến được tái sử dụng.
- Dùng `foreach (var notice in notices)` rồi capture trực tiếp `notice` cũng đúng với semantics C# hiện đại, nhưng tạo command riêng làm ownership rõ ràng hơn.

## 7. Wrong / tempting fixes
- Gửi trực tiếp trong vòng lặp: che lỗi nhưng phá hợp đồng deferred dispatch.
- Chỉ tăng số worker hoặc thêm retry: không sửa dữ liệu đã được capture.
- Chỉ assert `PendingCount == 4`: bỏ qua tính đúng đắn recipient.
- Thêm `Task.Delay` trước khi Drain: có thể đổi timing, không sửa closure.

## 8. Production implications
Sai recipient có thể làm lộ thông tin bàn giao hoặc bỏ lỡ nhiệm vụ. Ghi correlation ID theo từng notification, theo dõi expected recipient so với accepted recipient và thiết lập replay idempotent khi có sai lệch.

## 9. Trade-offs
Immutable command tăng một lượng allocation nhỏ nhưng tạo ranh giới dữ liệu dễ kiểm thử. Typed queue phù hợp cho hệ thống lớn và lưu bền, nhưng đòi hỏi thay đổi interface và serialization. Với batch nội bộ nhỏ, capture đúng biến là thay đổi tối thiểu hợp lý.

## 10. Senior engineer should notice
Một bài test chỉ kiểm tra số job enqueue sẽ xanh dù toàn bộ recipient sai. Kiểm thử phải bao phủ dữ liệu **sau** ranh giới bất đồng bộ, cả no-early-send, exact-once trong mô phỏng, thứ tự và định danh. Cần phân biệt bug capture biến tái sử dụng với semantics `foreach` của C# hiện đại.
