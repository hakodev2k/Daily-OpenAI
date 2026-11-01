# Reference Solution — SPOILER

## Symptoms
Yêu cầu shutdown vẫn pending khi gate downstream đóng, dù callback đã vào. Mở gate thì yêu cầu trả về và flush hoàn tất.

## Evidence
`CallbackEntered` hoàn thành trước `Cancel()`. Gate đóng giữ lời gọi lại. Đếm flush vẫn bằng 0 trước gate, bằng 1 sau gate.

## Root cause
`CancellationTokenSource.Cancel()` chạy callback đăng ký qua `CancellationToken.Register` đồng bộ. Starter gọi `downstreamRelease.GetAwaiter().GetResult()` ngay trong callback, nên thread yêu cầu hủy phải chờ cleanup.

## Why the fix works
[Code tham khảo](ShutdownCoordinator.cs) tạo một Task cleanup độc lập. Callback chỉ phát tín hiệu `TaskCompletionSource` với `RunContinuationsAsynchronously`; Task cleanup `await` tín hiệu rồi `await` downstream. `CleanupCompleted` là Task thực sự để caller quan sát completion và fault.

## How to verify
```powershell
Copy-Item ./solution/ShutdownCoordinator.cs ./starter/ShutdownCoordinator.cs -Force
./verify.ps1
```
Chỉ copy sau khi đã tự thử. `verify.ps1` luôn kiểm tra starter.

## Alternative fixes
Hosted worker với queue và Task ownership có thể phù hợp khi có nhiều bước cleanup. Với một tín hiệu, TaskCompletionSource là đơn giản hơn. Callback synchronous chỉ phù hợp với công việc thực sự bounded, không chờ dependency.

## Wrong / tempting fixes
- Tăng timeout chỉ che triệu chứng.
- `Task.Run` không theo dõi hoặc `async void` có thể làm mất lỗi và mất dữ liệu.
- Bỏ flush làm sai contract.
- Tăng ThreadPool không xóa dependency giữa callback và gate.

## Production implications
Đo riêng acknowledgement và cleanup, đặt deadline cho flush, await Task cleanup trước khi process thoát, truyền lỗi downstream cho host.

## Trade-offs
Thêm một Task và tín hiệu phối hợp nhưng tránh giữ thread gọi hủy. Caller phải quyết định rõ deadline, retry và cách báo lỗi nếu cleanup thất bại.

## Senior engineering judgment
“Cancel trả về nhanh” chưa đủ: cần bảo toàn dữ liệu, lỗi có thể quan sát, exactly-once trong phạm vi process và hành vi đúng khi token đã cancelled trước lúc đăng ký.
