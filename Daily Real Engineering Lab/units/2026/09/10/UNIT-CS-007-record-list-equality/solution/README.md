# Reference Solution

> Inspect only after reproducing the issue and attempting your own fix.

## Symptoms
Hai batch có cùng `TenantId` và cùng sequence product IDs nhưng equality trả `False`, khiến `HashSet` giữ cả hai.

## Evidence
Starter cho thấy dữ liệu sequence giống nhau trong khi hai `List<int>` là hai object riêng biệt.

## Root cause
C# `record` sinh equality theo equality semantics của từng member. `List<T>` không có structural sequence equality; member collection được so theo object identity. Vì vậy hai record chứa hai list khác instance không bằng nhau dù nội dung giống nhau.

## Why the fix works
Reference solution biến batch thành value object bất biến hơn, snapshot collection thành array riêng và cài `IEquatable<ProductBatch>` để so `TenantId` bằng `Ordinal` và product IDs bằng `SequenceEqual`. `GetHashCode` dùng đúng cùng tập dữ liệu và cùng thứ tự.

## How to verify
Copy cách tiếp cận tương đương vào `starter/`, sau đó chạy `./verify.ps1`.

## Alternative fixes
- Dùng một collection/value type có structural equality phù hợp với contract nghiệp vụ.
- Dùng custom `IEqualityComparer<ProductBatch>` tại boundary deduplication nếu không muốn equality toàn cục của type thay đổi.

## Wrong / tempting fixes
- Chỉ override `Equals` nhưng không đồng bộ `GetHashCode`: phá contract của hash-based collections.
- Dùng `ProductIds.ToString()`: không biểu diễn sequence value.
- Sort list trước khi so khi business yêu cầu thứ tự có ý nghĩa: thay đổi semantics.
- Giữ collection mutable rồi sửa sau khi object đã vào `HashSet`: hash có thể thay đổi và làm lookup sai.

## Production implications
Value objects dùng làm key/set member phải có equality và immutability contract rõ. Equality sai có thể gây duplicate processing, billing, audit hoặc idempotency failures.

## Trade-offs
Custom equality tăng code cần duy trì. Custom comparer giữ domain type đơn giản hơn nhưng semantics chỉ tồn tại tại nơi comparer được dùng. Chọn boundary phù hợp với business invariant.

## Senior engineer should notice
Không chỉ hỏi “record có value equality không”; phải kiểm tra equality semantics của toàn bộ object graph và tính ổn định của hash key sau insertion.
