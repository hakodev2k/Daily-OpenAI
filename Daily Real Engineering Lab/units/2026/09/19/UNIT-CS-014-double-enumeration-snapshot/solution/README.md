# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Summary và export trong cùng một reconciliation operation có thể mô tả hai tập item khác nhau.

## 2. Evidence
Log cho thấy summary được tính trước khi source thay đổi, còn export được tạo sau thay đổi. Cả hai consumer đều dùng cùng biến sequence.

## 3. Root cause
LINQ query trên `IEnumerable<T>` là deferred. Mỗi terminal enumeration chạy lại pipeline trên trạng thái hiện tại của source. Vì sequence được enumerate hai lần ở hai thời điểm khác nhau, operation không có snapshot consistency.

## 4. Why the fix works
Materialize một lần tại boundary nơi operation cần một dataset ổn định, rồi cho các consumer đọc cùng snapshot.

Ví dụ:
```csharp
var availableSnapshot = source.Where(x => x.Quantity > 0).ToArray();
var summaryCount = availableSnapshot.Length;
source.Add(new Item("D-400", 2));
var exported = availableSnapshot.Select(x => x.Sku).ToArray();
```

## 5. How to verify
Chạy `./verify.ps1`; output phải có `VERIFY_PASS`. Source vẫn có thể thay đổi sau snapshot nhưng hai output của operation phải nhất quán.

## 6. Alternative fixes
Có thể thiết kế repository trả immutable snapshot, hoặc thực hiện cả hai phép tính trong một database/query transaction phù hợp nếu consistency requirement nằm ở persistence layer.

## 7. Wrong or misleading fixes
Di chuyển mutation chỉ để test pass che giấu contract. Gọi `Count()` thêm lần nữa làm hai giá trị tình cờ khớp nhưng vẫn quan sát source ở thời điểm khác. Lock toàn hệ thống có thể quá nặng nếu chỉ cần operation-local snapshot.

## 8. Production implications
Deferred enumeration qua mutable collections, database queries hoặc streaming sources có thể tạo inconsistency khó thấy, đồng thời gây thêm I/O nếu query bị thực thi lại.

## 9. Trade-offs
Materialization dùng thêm memory và có thể không phù hợp dataset rất lớn. Khi đó cần xác định transaction/snapshot boundary hoặc streaming contract khác thay vì materialize vô hạn.

## 10. What a Senior engineer should notice
Câu hỏi chính không phải “LINQ có nhanh không” mà là sequence đại diện cho live query hay snapshot, ai sở hữu consistency boundary, và consumer có được phép quan sát các thời điểm khác nhau hay không.