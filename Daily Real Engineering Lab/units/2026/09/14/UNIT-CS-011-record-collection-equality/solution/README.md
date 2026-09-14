# Reference Solution

> Chỉ xem sau khi đã reproduce và tự thử fix.

## Symptoms

Hai request có cùng `CustomerId` và cùng ordered columns vẫn tạo hai entry trong `HashSet<ExportRequest>`.

## Evidence

`record` sinh value equality ở cấp object, nhưng member `Columns` có type `IReadOnlyList<string>`. Equality mặc định của hai array/list instance khác nhau không tự động so sánh từng phần tử theo structural semantics mà business đang cần.

## Root cause

Business identity là composite value gồm `CustomerId` và nội dung ordered columns, trong khi generated equality của record sử dụng equality contract mặc định của member collection. Vì hai collection instance khác nhau không equal theo contract đó, hai request logic-equivalent có hash/equality khác nhau.

## Why the fix works

Reference solution định nghĩa equality rõ ràng: `CustomerId` dùng ordinal string equality, còn `Columns` dùng `SequenceEqual`. `GetHashCode` áp dụng cùng rule và cùng thứ tự phần tử. Vì `Equals` và `GetHashCode` nhất quán, `HashSet<T>` có thể deduplicate đúng.

## How to verify

Chạy `verify.ps1`. Kết quả cần có:

- `first-equals-equivalent:True`
- `equivalent-count:1`
- `total-count:2`

## Alternative fixes

- Tạo một immutable value object riêng cho ordered columns và implement structural equality tại boundary đó.
- Canonicalize business key thành một immutable key object trước khi đưa vào `HashSet`.
- Nếu thứ tự columns không có ý nghĩa nghiệp vụ, normalize/sort trước khi so sánh; chỉ làm vậy khi contract thực sự coi order là irrelevant.

## Wrong or misleading fixes

- Chuyển từ `HashSet` sang `List` rồi tự gọi `Contains` mà không sửa equality: root cause vẫn còn.
- Dùng cùng một collection instance cho cả hai request: chỉ làm test pass tình cờ.
- Chỉ override `Equals` nhưng không đồng bộ `GetHashCode`: phá contract của hash-based collection.
- Serialize object thành JSON rồi dùng string làm key mặc định: có thể hoạt động nhưng tăng allocation/cost và dễ biến serialization detail thành identity contract.

## Production implications

Equality contract sai có thể gây duplicate jobs, duplicate commands, cache misses hoặc inconsistent dictionary lookups. Với composite value objects, Senior engineer cần review semantics của nested reference members thay vì giả định `record` luôn cho deep structural equality.

## Trade-offs

Custom equality rõ ràng nhưng cần maintenance khi identity fields thay đổi. Một value object nhỏ, immutable và có contract riêng thường dễ audit hơn việc để DTO phức tạp trực tiếp đóng vai trò hash key.

## What a Senior engineer should notice

`record` cung cấp generated equality, không tự định nghĩa business identity. Mỗi member vẫn mang equality semantics riêng. Khi object được dùng làm `HashSet` key, `Dictionary` key, idempotency key hoặc cache key, equality/hash contract trở thành một phần của correctness chứ không chỉ là implementation detail.
