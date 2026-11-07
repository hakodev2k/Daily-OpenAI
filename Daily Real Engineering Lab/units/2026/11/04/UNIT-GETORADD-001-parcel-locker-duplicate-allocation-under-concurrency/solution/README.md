# Reference Solution — chỉ xem sau khi tự thử

## Root cause
`ConcurrentDictionary.GetOrAdd` bảo đảm thao tác dictionary an toàn nhưng `valueFactory` có thể chạy nhiều lần ngoài lock nội bộ. Side effect trong factory không có bảo đảm exactly-once. Harness dùng `CountdownEvent` để buộc hai factory cùng bắt đầu; cả hai cấp chỗ, chỉ một kết quả được lưu.

## Fix
Xem [Program.cs](Program.cs). Lưu `Lazy<string>` trong dictionary, thực thi gateway ở `holder.Value` với `LazyThreadSafetyMode.ExecutionAndPublication`. Nhiều candidate `Lazy` có thể được tạo nhưng chỉ winner trong cache được evaluate. Khi factory ném exception, xóa chính xác cặp key/holder lỗi để request tiếp theo có thể thử lại.

## Tự áp dụng
Chuyển logic trong `solution/Program.cs` sang `starter/Program.cs`, rồi chạy `./verify.ps1`. Không đổi script verify để làm test pass.

## Trade-offs
Giải pháp chỉ bảo đảm single execution trong **một process và một lifetime của cache**. Restart, scale-out, timeout sau khi gateway đã commit và response bị mất vẫn có thể tạo duplicate. Production cần idempotency key được gateway lưu bền vững, transaction/unique constraint hoặc reconciliation. Nếu cần async I/O, dùng `Lazy<Task<T>>` và thiết kế eviction/cancellation riêng, tránh gắn cancellation của một caller vào shared work.

## Wrong fixes
- `ConcurrentDictionary` đơn thuần không đảm bảo callback chạy một lần.
- `lock` quanh mọi request có thể serialize tất cả parcel IDs và làm giảm throughput.
- Chỉ kiểm tra `ContainsKey` trước `GetOrAdd` tạo thêm race.
- Nuốt exception rồi giữ faulted holder khiến retry vĩnh viễn thất bại.

## Verification
`reproduce.ps1` phải pass ở starter ban đầu. `verify.ps1` chỉ pass sau khi learner sửa starter. Expected: `CALLS 1`, `CACHE 1`, `SAME True`, `FAILURE_RETRY True`.

## Official reference
Microsoft .NET API: https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2.getoradd
