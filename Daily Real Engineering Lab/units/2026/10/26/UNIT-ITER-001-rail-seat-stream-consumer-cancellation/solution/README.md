# Reference Solution — SPOILER

**Chỉ đọc sau khi tự reproduce, điều tra và verify.**

## Symptoms
Stream bình thường phát đủ ba cập nhật; đường truyền token trực tiếp dừng đúng; subscriber hủy qua enumeration nhưng stream vẫn tiếp tục.

## Evidence
`direct_parameter_canceled=True` cho thấy logic kiểm tra token bên trong iterator hoạt động khi nhận đúng token. Hai phép thử subscriber cho thấy khác biệt nằm ở hợp đồng truyền token, không phải số lượng dữ liệu.

## Root cause
`WatchAsync()` nhận tham số token mặc định khi tạo enumerable. Token đưa vào `GetAsyncEnumerator` thông qua `WithCancellation` không được liên kết với tham số đó nếu async iterator không khai báo contract compiler thích hợp.

## Fix và vì sao hoạt động
Trong [reference code](SeatAvailabilityFeed.cs), thêm `using System.Runtime.CompilerServices;` và `[EnumeratorCancellation]` trên tham số token của async iterator. Compiler dùng token từ enumerator (hoặc kết hợp token caller đã truyền) khi chạy iterator.

## Verification
Chỉ sau khi tự sửa, có thể sao chép reference source vào `starter/SeatAvailabilityFeed.cs` để đối chiếu. Chạy `scripts/verify.ps1`, kiểm tra bốn giá trị trong `expected-results/after.md`. Verify phải chạy trên learner-editable starter.

## Alternative fixes
Truyền token trực tiếp qua `WatchAsync(token)` có thể đúng khi mọi caller đều kiểm soát được; nhưng không giải quyết contract của consumer dùng `WithCancellation`.

## Wrong / tempting fixes
- Thêm `Task.Delay` hoặc tăng timeout: không sửa đường truyền token.
- Kiểm tra token tự tạo: không đại diện cho subscriber.
- Nuốt `OperationCanceledException`: che giấu cancellation.
- Chỉ sửa test thành direct token: không sửa API contract thực tế.

## Production implications
Với streaming HTTP/gRPC, cần propagate token tới I/O downstream, đo subscription còn tồn tại sau disconnect và cleanup resource trong `finally` khi cần.

## Trade-offs
Annotation nhỏ, không thêm dependency; compiler có thể link hai token nếu caller dùng cả hai. Cancellation cooperative: I/O không hỗ trợ token vẫn có thể chạy tiếp.

## Senior engineer should notice
Cần phân biệt lifecycle của enumerable và enumerator, test cả hủy trước/sau lần đọc đầu, giữ behavior regression và không suy diễn từ một test pass rằng toàn pipeline đã propagate token.
