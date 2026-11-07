# Reference Solution — Chỉ đọc sau khi tự reproduce và thử sửa

## 1. Symptoms

Worker đánh dấu batch `COMPLETED` ngay khi gọi dispatch xong, dù gateway chưa xác nhận cảnh báo nào. Sau đó gateway mới lần lượt hoàn tất.

## 2. Evidence

Trong harness, `dispatchTaskCompleted=True` trong khi `sentBeforeRelease=0`. `sentAfterRelease=3` cho thấy gateway có thể gửi được, nhưng trạng thái hoàn tất của batch không đại diện cho các thao tác gửi.

## 3. Root cause

`List<T>.ForEach` nhận `Action<T>` (trả về `void`). Lambda `async alert => await ...` bị chuyển thành `async void`; mỗi callback chạy đến điểm await rồi trả về cho `ForEach`. Sau khi duyệt hết, method trả `Task.CompletedTask`, tách biệt với ba lần gửi thực sự. Exception sau await của `async void` không được caller quan sát qua batch Task, và có thể gây unhandled exception.

## 4. Why the fix works

[DispatchService.cs](DispatchService.cs) tạo một `Task` cho mỗi send, lưu toàn bộ vào mảng, rồi trả `Task.WhenAll`. Caller `await` đúng batch Task; batch chỉ hoàn tất khi tất cả send hoàn tất. Lỗi từ một hay nhiều send làm batch faulted; cancellation được chuyển tới gateway qua token. `Task.WhenAll` không tự hủy các send khác khi một send lỗi.

## 5. How to verify

Thay nội dung `starter/DispatchService.cs` bằng implementation tự viết; chạy `./scripts/verify.ps1`. Harness kiểm tra: batch còn pending trước khi mở gate, đủ ba send sau gate, và `InvalidOperationException("partner-unavailable")` được caller bắt qua `await`. Không thay test để khiến nó xanh.

## 6. Alternative fixes

- `foreach` + `await gateway.SendAsync(...)` trong method `async Task`: tuần tự, dễ điều tiết tải, đổi lại throughput thấp hơn.
- `Task.WhenAll`: song song cho batch nhỏ, có thể tạo burst lên partner.
- Với batch lớn: `Parallel.ForEachAsync` hoặc `SemaphoreSlim` để giới hạn số send đồng thời; vẫn phải await kết quả và xác định chính sách partial failure.

## 7. Wrong / misleading fixes

- `Task.Delay(1000)` sau `ForEach`: chỉ che triệu chứng, phụ thuộc tốc độ máy và mạng.
- Đổi `Task.CompletedTask` thành `Task.Run(...)` nhưng vẫn dùng `async void`: không có quan hệ completion đúng.
- `try/catch` bên ngoài `ForEach`: không bắt được lỗi xảy ra sau await trong `async void`.
- Tăng timeout của job: không sửa quan hệ giữa trạng thái batch và công việc.

## 8. Production implications

Không đánh dấu `COMPLETED` trước khi có kết quả send. Ghi metric `started/completed/failed` riêng biệt; xác định retry và idempotency theo alert ID để tránh gửi trùng. Với partner không đáng tin cậy, cần giới hạn concurrency, timeout, và durable outbox nếu không thể mất cảnh báo.

## 9. Trade-offs

`Task.WhenAll` cho độ trễ batch tốt khi gateway độc lập, nhưng có thể vượt quota và tốn connection nếu batch lớn. Tuần tự đơn giản hơn nhưng chậm. Giới hạn concurrency cần thiết khi số cảnh báo lớn; lỗi một send không nhất thiết làm các send khác dừng.

## 10. What a Senior engineer should notice

Completion contract là một phần của correctness, không chỉ performance. Phải quyết định rõ batch thành công khi **tất cả**, **một phần**, hay **đã persist để retry**; phân biệt acknowledgement từ gateway với delivery cuối cùng. Thiết kế regression test có cổng chờ thay vì sleep để không phụ thuộc timing. Không thay đổi trạng thái học tập chỉ vì unit được tạo.

## Tài liệu chính thức

- [C# async return types](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-return-types)
- [List<T>.ForEach](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1.foreach?view=net-8.0)
- [Task.WhenAll](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.whenall?view=net-8.0)
