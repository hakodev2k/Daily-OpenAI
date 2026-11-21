# Reference Solution — chỉ xem sau khi đã tự thử sửa

## Symptoms
Consumer hủy sau bản tin đầu nhưng starter vẫn trả 2 và 3.

## Evidence
`reproduce.ps1` in `CANCEL_IGNORED`; `verify.ps1` thất bại; luồng bình thường vẫn chạy.

## Root cause
Trong C# async iterator, `WithCancellation(token)` cung cấp token cho `GetAsyncEnumerator(token)`. Khi tham số `CancellationToken` của iterator không có `[EnumeratorCancellation]`, compiler không liên kết token này với tham số của iterator. Token mặc định của `GetCapacityUpdatesAsync()` vẫn là `CancellationToken.None`; `Task.Delay` không nhận tín hiệu hủy. Compiler có thể phát warning CS8425.

## Why the fix works
Thêm `[EnumeratorCancellation]` vào tham số token, giữ `Task.Delay(15, cancellationToken)`. Compiler nối token từ enumerator vào iterator (và xử lý kết hợp token khi cả hai nguồn được cung cấp).

## How to verify
Thay nội dung `starter/ShelterFeed.cs` bằng `solution/ShelterFeed.cs`, chạy `./verify.ps1`; mong đợi `CANCEL_OBSERVED` và normal PASS. Sau đó tự khôi phục starter nếu muốn làm lại.

## Alternative fixes
Truyền token trực tiếp vào `GetCapacityUpdatesAsync(cts.Token)` cũng khiến riêng call site này hoạt động, nhưng không bảo đảm contract cho mọi consumer dùng `WithCancellation`. API cần định nghĩa nhất quán hai cách sử dụng.

## Wrong or misleading fixes
- Chỉ gọi `WithCancellation` ở consumer mà không kiểm tra iterator: vẫn lỗi.
- Chỉ `ThrowIfCancellationRequested` trong consumer sau mỗi yield: có thể phát hiện hủy muộn, không dừng các I/O đang chờ.
- Dùng `Task.Run` để tách producer: không sửa propagation và làm khó quản lý lifecycle.

## Production implications
Khi client ngắt SSE/streaming HTTP, cần nối `HttpContext.RequestAborted` tới consumer và producer, đồng thời bảo đảm I/O downstream tôn trọng token. Không đánh đồng cancellation với lỗi cần retry.

## Trade-offs
Cancellation là cooperative: một dependency bỏ qua token vẫn có thể tiếp tục. Thêm deadline, metrics số stream còn hoạt động và logging phù hợp; không log thông tin cá nhân trong payload.

## What a Senior engineer should notice
Đánh giá contract của `IAsyncEnumerable<T>` ở cả producer và consumer, compiler-generated enumerator, và sự khác nhau giữa ngừng đọc output với dừng side effects. Kiểm thử hủy trong khi đang chờ I/O, không chỉ trước lần gọi đầu.
