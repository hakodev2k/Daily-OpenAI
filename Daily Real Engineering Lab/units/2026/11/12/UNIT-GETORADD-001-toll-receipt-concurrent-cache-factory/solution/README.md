# Reference solution — Spoiler

## Root cause
`ConcurrentDictionary<TKey,TValue>.GetOrAdd` bảo vệ tính nhất quán của dictionary, **không** đảm bảo delegate `valueFactory` chỉ chạy một lần. Khi hai caller cạnh tranh cùng key, cả hai có thể phát hành serial bên ngoài, dù dictionary chỉ lưu một giá trị.

## Fix
Lưu `Lazy<string>` trong dictionary và dùng `LazyThreadSafetyMode.ExecutionAndPublication` để bảo đảm chỉ một caller thực thi factory của Lazy đã thắng cuộc. Các Lazy bị loại không bị evaluate nên không gây side effect. Xem `Program.cs` của thư mục này.

## Verification
`dotnet run --project solution/TollReceiptLab.csproj -- --verify` phải in `issued=1` và `CONTRACT_PASSED`. Script `scripts/verify.ps1` **cố ý chạy starter**, không chạy solution.

## Wrong fixes
- Dùng `ConcurrentDictionary` nhưng tiếp tục đặt side effect trực tiếp trong factory: vẫn có thể phát hành trùng.
- Chỉ kiểm tra `results.Distinct().Count() == 1`: bỏ qua side effect thực tế.
- Khóa toàn bộ hệ thống bằng một global lock: có thể đúng nhưng giảm throughput không cần thiết giữa các key.
- Chỉ retry khi thất bại: không giải quyết issuance trùng đã xảy ra.

## Production trade-offs
`Lazy<T>` với ExecutionAndPublication cache cả exception; cần chiến lược evict/retry có kiểm soát. Nếu có nhiều process/replica, in-memory single-flight không đủ: dùng transaction/idempotency store hoặc unique constraint ở service phát hành. Đặt business invariant tại hệ thống authoritative; không dựa hoàn toàn vào cache.
