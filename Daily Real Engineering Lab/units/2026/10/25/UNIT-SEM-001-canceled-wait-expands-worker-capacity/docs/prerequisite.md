# Prerequisites
- `SemaphoreSlim.WaitAsync(CancellationToken)` trả về Task hoàn thành khi giành được permit hoặc bị hủy trong lúc chờ.
- `CancellationToken` biểu diễn yêu cầu hủy, không có nghĩa là operation chắc chắn đã bắt đầu.
- `TaskCompletionSource` cho phép điều khiển thứ tự sự kiện trong test mà không dùng `Thread.Sleep`.
- `try/finally` bảo đảm cleanup, nhưng cleanup phải tương ứng với tài nguyên đã thực sự sở hữu.
- Có thể chạy trên Windows, Linux hoặc macOS với .NET SDK 8; không có package bên thứ ba.
