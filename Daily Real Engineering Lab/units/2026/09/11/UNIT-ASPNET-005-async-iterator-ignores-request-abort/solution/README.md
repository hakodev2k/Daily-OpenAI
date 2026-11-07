# Reference Solution

> Chỉ đọc sau khi đã reproduce và tự thử sửa.

## 1. Symptoms

Consumer phát cancellation nhưng sau grace period vẫn còn producer chạy, slot trong pool vẫn bị giữ và các consumer khác có thể tiếp tục chờ tài nguyên.

## 2. Evidence

Điểm quan trọng là ba metric `ACTIVE_PRODUCERS_AFTER_GRACE`, `POOL_IN_USE_AFTER_GRACE` và `WAITING_FOR_POOL_AFTER_GRACE` không trở về 0 dù client đã hủy.

## 3. Root cause

`StreamAsync` là async iterator có parameter `CancellationToken`, nhưng parameter đó không được đánh dấu bằng `[EnumeratorCancellation]`.

Consumer gọi:

```csharp
streamer.StreamAsync().WithCancellation(cancellationToken)
```

Token của consumer được đưa vào `GetAsyncEnumerator(CancellationToken)`. Với async iterator trong starter, compiler không dùng token từ enumeration để thay thế/kết hợp với parameter token của method. Vì method được gọi mà không truyền token, thân iterator tiếp tục dùng `CancellationToken.None`.

Kết quả: `AcquireAsync` và `Task.Delay` bên trong producer không quan sát cancellation của consumer.

## 4. Why the fix works

Đánh dấu parameter bằng:

```csharp
[EnumeratorCancellation] CancellationToken cancellationToken = default
```

cho phép token dùng khi enumerate trở thành token mà iterator body quan sát. Token đó tiếp tục được truyền vào acquisition và các async operation bên trong.

Khi client hủy, producer đang chạy nhận `OperationCanceledException`, đi qua `finally`, sau đó `await using` dispose lease và trả slot về pool. Consumer đang chờ pool cũng được cancel thay vì tiếp tục chờ tài nguyên không còn cần thiết.

File tham chiếu: `solution/AuditStreamer.cs`.

## 5. How to verify

Áp dụng thay đổi tương đương vào `starter/AuditStreamer.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kỳ vọng:

```text
ACTIVE_PRODUCERS_AFTER_GRACE=0
POOL_IN_USE_AFTER_GRACE=0
WAITING_FOR_POOL_AFTER_GRACE=0
RESULT=FIX_VERIFIED
```

## 6. Alternative fixes

Một API có thể truyền token trực tiếp khi gọi iterator:

```csharp
streamer.StreamAsync(cancellationToken)
```

nếu contract của codebase quy định như vậy. Tuy nhiên nếu API được thiết kế để consumer dùng `WithCancellation`, async iterator vẫn nên khai báo semantics enumeration cancellation rõ ràng.

Trong ASP.NET Core endpoint thật, token thường bắt nguồn từ `HttpContext.RequestAborted` hoặc parameter `CancellationToken` được model binding cung cấp. Token đó cần được truyền xuyên suốt xuống database/client call có hỗ trợ cancellation.

## 7. Wrong / tempting fixes

### Tăng pool size

Có thể giảm symptom tạm thời nhưng producer không cần thiết vẫn sống sau client abort. Khi traffic tăng, cùng leak-of-capacity pattern quay lại ở quy mô lớn hơn.

### Giảm `Task.Delay`

Chỉ làm incident khó thấy hơn trong simulation. Nó không sửa contract cancellation.

### Poll `IsCancellationRequested` ở outer consumer

Outer consumer đã có token; vấn đề là producer không quan sát đúng token. Polling thêm ở nơi sai không giải quyết các async operation đang giữ tài nguyên.

### Catch và bỏ qua mọi `OperationCanceledException`

Catch có thể phù hợp ở boundary, nhưng không thể thay thế việc truyền token đúng. Nếu cancellation không tới producer thì exception cũng không xuất hiện ở nơi cần thiết.

## 8. Production implications

Trong production, tài nguyên bị giữ có thể là:

- SQL connection / data reader
- HTTP connection
- file handle
- rate-limited downstream slot
- semaphore permit
- memory buffer lớn

Client disconnect là failure path bình thường của streaming API. Nếu server không dừng downstream work, capacity có thể bị tiêu hao bởi request mà không còn người nhận kết quả.

## 9. Trade-offs

Cancellation cooperative, không phải rollback tự động. Một operation đã tạo side effect trước khi nhận cancellation vẫn có thể cần idempotency hoặc compensating behavior.

Không nên truyền cancellation vào mọi critical side effect một cách máy móc. Sau một commit point, business operation có thể cần hoàn tất dù client đã biến mất. Boundary đó phải là quyết định nghiệp vụ rõ ràng.

## 10. What a Senior engineer should notice

Senior engineer không chỉ thêm attribute. Họ kiểm tra toàn bộ cancellation path:

`RequestAborted → async enumeration → resource acquisition → dependency call → cleanup`.

Họ cũng phân biệt:

- request lifetime
- business operation lifetime
- resource lifetime

và quyết định nơi cancellation nên được tôn trọng, nơi nào cần shield/complete operation, cùng cách đo resource retention trong production.