# Reference Solution — chỉ đọc sau khi tự reproduce và thử fix

## 1. Symptoms
Worker vẫn sống và không báo exception nhưng thiếu settlement của các giờ đã trôi qua khi xử lý chậm.

## 2. Evidence
Fixture 00:00 → 05:00 có 5 cửa sổ đến hạn; starter chỉ ghi nhận 01:00 sau một wake-up. Không thể dùng số lần timer đánh thức làm thước đo hoàn thành nghiệp vụ.

## 3. Root cause
`PeriodicTimer` coalesces các tick xảy ra khi consumer bận. Starter gắn **một wake-up = một cửa sổ giờ**, nên không đối soát khoảng cách giữa `LastCompletedWindow` và thời gian nghiệp vụ hiện tại. Worker có thể tụt lại mãi khi mỗi lượt xử lý chậm hơn chu kỳ timer.

## 4. Why the fix works
[WindowScheduler.cs](WindowScheduler.cs) tính lần lượt từng cửa sổ còn nợ cho tới snapshot `now`. Sau khi `ProcessAsync` thành công mới cập nhật checkpoint. Khi downstream lỗi, exception đi lên và checkpoint vẫn giữ cửa sổ hoàn thành cuối cùng; lần retry không bỏ qua cửa sổ thất bại.

## 5. How to verify
Sau khi tự làm xong, so sánh với reference:

```powershell
Copy-Item ./solution/WindowScheduler.cs ./starter/WindowScheduler.cs -Force
./verify.ps1
```

**Lưu ý:** lệnh copy ghi đè bản learner; chỉ dùng khi đã lưu lại lời giải của mình. `verify.ps1` luôn chạy starter, không chạy riêng reference.

## 6. Alternative fixes
- Scheduler dựa trên **durable next-due watermark** và truy vấn backlog từ database; phù hợp nhiều worker nhưng phải có lease/claim và idempotency.
- Queue một message cho từng giờ ngay tại nguồn; tăng khả năng retry/scale nhưng cần quản lý ordering, duplicate delivery và vận hành broker.
- Batch các giờ thành một truy vấn nếu nghiệp vụ cho phép, song checkpoint phải phản ánh chính xác tập giờ đã commit.

## 7. Wrong or misleading fixes
- Giảm chu kỳ `PeriodicTimer`: tăng wake-up nhưng không khôi phục nghĩa vụ đã bỏ sót.
- Tăng ThreadPool hoặc scale replica: có thể giảm độ trễ nhưng không sửa semantic gap; nhiều replica còn có nguy cơ chốt trùng.
- Tăng checkpoint **trước** khi gọi downstream: tạo mất dữ liệu khi crash.
- Bọc `ProcessAsync` trong `Task.Run` không giải quyết mapping tick/cửa sổ.

## 8. Production implications
Checkpoint in-memory **không** bền khi crash. Production cần transaction/checkpoint storage, unique key như `(campaignId, windowEnd)`, idempotent sink, bounded catch-up, observability (`oldestUnsettledWindow`, backlog count, lag), và chính sách retry/dead-letter. Nếu một cửa sổ có thể mất hàng giờ, không giữ vòng lặp vô hạn mà cần phân phối công việc có kiểm soát.

## 9. Trade-offs
Loop catch-up đơn giản, đúng cho một worker với số giờ nợ nhỏ. Với backlog lớn, cần budget mỗi lần chạy, giới hạn độ trễ của các tác vụ khác, cân bằng throughput và fairness; dừng giữa chừng phải giữ checkpoint chính xác. Không tuyên bố exactly-once nếu không có durable coordination.

## 10. What a Senior engineer should notice
Timer là **trigger**, không phải nguồn sự thật của các cửa sổ cần xử lý. Hãy định nghĩa rõ business watermark, commit boundary, duplicate policy và hành vi sau crash. Kiểm thử recovery phải kiểm tra trạng thái *sau khi lỗi* chứ không chỉ happy path.