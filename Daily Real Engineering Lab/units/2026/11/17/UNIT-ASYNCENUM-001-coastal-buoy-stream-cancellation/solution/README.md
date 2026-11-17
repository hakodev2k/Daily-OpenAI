# Reference Solution — spoiler

`WithCancellation(token)` truyền token đến `GetAsyncEnumerator(token)`. Trong async iterator, tham số `CancellationToken` không tự nhận token từ enumerator nếu không có `[EnumeratorCancellation]`. Compiler có thể phát cảnh báo CS8425. Vì thế `ThrowIfCancellationRequested()` kiểm tra token mặc định và vẫn phát dữ liệu.

Đối chiếu `solution/Program.cs`: thêm `[EnumeratorCancellation]` cho tham số token, giữ nguyên kiểm tra cancellation. Áp dụng thay đổi đó vào starter và chạy `./scripts/verify.ps1`. Kết quả mong đợi `cancelled=0 healthy=3`, `PASS`.

Wrong fixes: (1) chỉ bắt exception ở consumer, không truyền token tới producer; (2) kiểm tra token sau `yield return` làm rò một mẫu; (3) sửa verify để bỏ luồng khỏe. Trong production cần truyền token xuống cả I/O và phân biệt client cancellation với lỗi dependency.
