# Reference Solution — Spoiler

## Root cause
`ConcurrentDictionary.GetOrAdd` đảm bảo dictionary update an toàn, **không** đảm bảo value factory chỉ chạy một lần khi nhiều caller đua nhau. Factory có side effect cấp thẻ; những kết quả thua cuộc bị loại khỏi cache nhưng external issuance vẫn xảy ra.

## Fix
Dùng `ConcurrentDictionary<string, Lazy<Pass>>`, tạo `Lazy` bằng `ExecutionAndPublication` và chỉ thực hiện `Issue` khi truy cập `Value`. Các caller thắng/thua `GetOrAdd` cùng sử dụng `Lazy` được lưu. [Program.cs](Program.cs) chứa implementation tham khảo. Khi `Issue` lỗi, xóa đúng cặp key/value lỗi để retry lần sau (nếu hợp đồng cho phép).

## Wrong fixes
- Chỉ kiểm tra `ContainsKey` trước `GetOrAdd`: check-then-act vẫn race.
- Tăng lock quanh `Console.WriteLine`: không bảo vệ side effect.
- Xóa cache entry dư: không thu hồi thẻ đã cấp ở ledger.
- `LazyThreadSafetyMode.PublicationOnly`: vẫn có thể chạy nhiều factory.

## Trade-offs
Single-flight chỉ áp dụng trong một process; nhiều replicas cần idempotency key ở ledger hoặc unique constraint/transaction bên server phát hành. Đánh giá memory retention, failure eviction và TTL trước khi dùng production. Không coi test 10 lần là chứng minh tuyệt đối cho mọi scheduler.

## Verify
Sao chép logic tham khảo vào starter, chạy `./verify.ps1`. Reproduce script chỉ đo baseline; sau fix nó có thể in issued=1, cache=1.
