# Reference Solution — SPOILER

## Root cause
`WithCancellation(stop.Token)` cung cấp token cho `GetAsyncEnumerator`, nhưng async iterator có tham số `CancellationToken` mà thiếu `[EnumeratorCancellation]`. Compiler không chuyển token enumerator vào tham số này. Lời gọi `ReadAsync()` dùng token mặc định, vì thế `Task.Delay` không bị hủy; warning CS8425 là manh mối.

## Fix
So sánh [reference source](ShipmentScanStream.cs) với `starter/ShipmentScanStream.cs`: import `System.Runtime.CompilerServices`, gắn `[EnumeratorCancellation]` vào tham số token. Chỉ thay đổi starter, không chỉnh harness.

## Mechanism
Async iterator do compiler sinh ra chuyển token của enumerator vào tham số đánh dấu, xử lý cả trường hợp method có token riêng. `Task.Delay` nhận token đã được chuyển, nên sau bản ghi đầu tiên lần `MoveNextAsync` tiếp theo bị hủy bằng `OperationCanceledException`.

## Wrong fixes
- Chỉ gọi `WithCancellation`: starter đã có; thiếu binding ở producer.
- Chỉ gọi `Cancel()`: không tự động hủy công việc không nhận token.
- Chỉ `break` tại consumer: có thể che output nhưng không dừng side effects/I/O trong producer.
- Thay `Task.Delay` bằng `Thread.Sleep` hoặc `Task.Run`: không sửa contract.
- Nuốt `OperationCanceledException` thành thành công: sai semantics vận hành.

## Trade-offs
Cancellation là cooperative; mỗi tầng I/O phải dùng token. Nếu truyền cả token trực tiếp vào iterator và qua enumerator, compiler có thể cần linked token source. Kiểm thử cancel-before-first, cancel-between-items, normal completion, và I/O failure riêng biệt. Đừng suy luận cancellation bảo đảm rollback side effects đã commit.

## Verification
`reproduce.ps1` phải exit 0 trước sửa; `verify.ps1` phải exit 0 sau sửa. Verification chỉ build `starter/`, không build reference.
