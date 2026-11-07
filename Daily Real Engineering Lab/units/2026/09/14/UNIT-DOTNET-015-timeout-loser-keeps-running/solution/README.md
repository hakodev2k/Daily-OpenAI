# Reference Solution

> Chỉ xem sau khi đã reproduce và tự thử fix.

## 1. Symptoms

Caller nhận timeout đúng hạn, nhưng simulated provider vẫn báo nhiều operation active sau khi timeout đã được trả về.

## 2. Evidence

- `TIMEOUTS=6`
- `ACTIVE_AFTER_TIMEOUTS` lớn hơn 0 ở starter.
- Active counter chỉ giảm khi downstream delay tự kết thúc.

## 3. Root cause

`Task.WhenAny` chỉ cho biết task nào hoàn thành trước. Nó không tự cancel task thua cuộc. Timeout branch trả về ngay nhưng `work` vẫn tiếp tục chạy vì cancellation token chưa được signal và task chưa được observe đến completion.

## 4. Why the fix works

Timeout branch chủ động cancel operation thông qua token đã truyền xuống provider, sau đó await task thua cuộc để lifetime của work không vượt ra ngoài timeout boundary. Caller vẫn nhận `TimeoutException`, nhưng resource ownership được kết thúc rõ ràng trước khi method rời scope.

## 5. How to verify

Áp dụng cách xử lý từ `solution/Program.cs` vào starter rồi chạy:

```powershell
./verify.ps1
```

Expected: `TIMEOUTS=6` và `ACTIVE_AFTER_TIMEOUTS=0`.

## 6. Alternative fixes

- Trên .NET 6+, `WaitAsync(timeout, cancellationToken)` có thể đơn giản hóa timeout cho việc chờ, nhưng vẫn phải hiểu rằng timeout của waiter không nhất thiết cancel underlying operation nếu operation có lifetime riêng.
- Dùng `CancellationTokenSource.CancelAfter` khi timeout chính là cancellation policy của operation và token được propagate đầy đủ.
- Ở boundary HTTP, có thể link request-abort token với timeout token nếu semantics yêu cầu cả hai cùng dừng work.

## 7. Wrong or misleading fixes

- Chỉ tăng timeout: giảm số timeout nhưng không sửa lifetime leak khi timeout vẫn xảy ra.
- Chỉ bỏ reference tới task: GC không phải cancellation mechanism.
- `Task.Run` quanh downstream async call: không làm work biến mất và còn có thể tăng scheduling overhead.
- Retry ngay sau timeout mà không dừng attempt cũ: có thể nhân đôi work và làm capacity pressure nặng hơn.

## 8. Production implications

Orphaned timeout losers có thể tiếp tục giữ connection, semaphore slot, memory, rate-limit budget hoặc third-party quota. Khi retry được thêm vào, số work thực tế có thể lớn hơn nhiều so với request concurrency mà telemetry phía caller thể hiện.

## 9. Trade-offs

Không phải operation nào cũng hỗ trợ cancellation tức thời. Sau khi signal cancellation, code có thể cần chọn giữa chờ cleanup bounded, detach có giám sát, hay chuyển ownership cho background component. Điều quan trọng là lifetime phải explicit.

## 10. What a Senior engineer should notice

Timeout là policy của caller; cancellation là signal tới operation; completion/observation là lifecycle management. Ba khái niệm này liên quan nhưng không đồng nghĩa. Một implementation trả timeout đúng vẫn có thể sai về capacity và resource lifetime.
