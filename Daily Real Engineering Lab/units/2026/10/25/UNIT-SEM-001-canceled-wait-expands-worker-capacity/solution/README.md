# Reference Solution — SPOILER
Chỉ đọc sau khi reproduce, đưa hypothesis và thử fix.

## 1. Symptoms
Sau khi B bị hủy trong queue, C bắt đầu render khi A chưa xong dù `MaxWorkers=1`. CPU không quá tải; vấn đề nằm ở contract concurrency, không phải hiệu năng máy.

## 2. Evidence
Harness giữ A bằng `TaskCompletionSource`, đặt B vào queue, hủy B rồi gọi C **trước khi giải phóng A**. Vì vậy `thirdEntered.Task.IsCompleted` là bằng chứng về thứ tự, không phải một phép đo timing thiếu ổn định. B vẫn ném `OperationCanceledException`.

## 3. Root cause
`try/finally` trong starter bao trùm cả `await _slots.WaitAsync(token)`. Khi B bị hủy trong lúc chờ, `WaitAsync` ném trước khi acquire; `finally` vẫn `Release()` một permit mà B chưa sở hữu. `SemaphoreSlim(maxWorkers)` dùng max count mặc định lớn, nên lần release thừa có thể tăng capacity lên trên cấu hình. C vào khi A còn chạy.

## 4. Why the fix works
Chỉ đặt `try/finally` **sau khi `WaitAsync` thành công**. Khi đó mỗi acquire có đúng một release, kể cả callback fault hoặc bị hủy trong lúc đang chạy. Tham số `maxCount` bằng `maxWorkers` bổ sung kiểm tra invariant, nhưng không thay thế việc sửa ownership.

Tham khảo [WorkGate.cs](WorkGate.cs). Thay nội dung `starter/WorkGate.cs` bằng bản này để thử nghiệm, không sửa harness.

## 5. Verification
`./scripts/verify.ps1` phải exit 0. Probe xác minh B hủy đúng, C chưa vào khi A còn giữ permit, và exception trong callback không chặn work kế tiếp. `./scripts/reproduce.ps1` sẽ exit 1 sau fix — đúng semantics.

## 6. Alternative fixes
Một biến `acquired` với điều kiện `if (acquired) Release()` cũng đúng nếu được thiết lập **chỉ sau** khi await thành công; cần giữ code rõ ràng. Với pipeline phức tạp có thể dùng bounded Channel/worker loop, nhưng đó là thiết kế khác, không phải yêu cầu lab này.

## 7. Wrong / tempting fixes
- Tăng `MaxWorkers`: che triệu chứng, tăng áp lực memory và phá invariant.
- Chỉ đổi thành `new SemaphoreSlim(1, 1)`: có thể biến lỗi capacity thành `SemaphoreFullException` khi B bị hủy, che exception cancellation.
- Bắt `OperationCanceledException` rồi tiếp tục: làm request bị hủy có thể chạy công việc không cần thiết.
- Dùng `Thread.Sleep` trong test: tăng flakiness, không chứng minh ownership.

## 8. Production implications
Một cancel storm có thể phá giới hạn xử lý và gây memory pressure dù telemetry queue/CPU ban đầu trông bình thường. Cần metrics về active callbacks, queued waits, canceled waits, admission và capacity violations. Đừng log dữ liệu GIS nhạy cảm vào metric labels.

## 9. Trade-offs
`SemaphoreSlim` in-process đơn giản, ít dependency nhưng không điều phối được nhiều replica và không cung cấp fairness SLA. Channel bounded hữu ích nếu cần backpressure và hàng đợi tường minh; distributed lease/queue cần thiết khi giới hạn phải áp dụng toàn cluster.

## 10. What a Senior engineer should notice
Tính đúng đắn của resource cleanup dựa trên **ownership**, không phải chỉ có `finally`. Phải kiểm thử cancellation **trước acquire**, cancellation **sau acquire**, fault và normal completion. Capacity contract cần test deterministic và alert khi active work vượt configured limit.
