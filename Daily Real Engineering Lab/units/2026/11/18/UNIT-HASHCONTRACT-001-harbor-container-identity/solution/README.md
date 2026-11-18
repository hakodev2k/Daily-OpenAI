# Reference Solution — UNIT-HASHCONTRACT-001

## Root cause
`Equals` dùng `StringComparer.OrdinalIgnoreCase`, còn `GetHashCode` dùng `string` hash mặc định phân biệt chữ hoa/thường. Hai object bằng nhau có thể rơi vào bucket khác nhau. Contract bắt buộc: `Equals(x,y) == true` thì hash phải bằng nhau.

## Sửa
Thay `GetHashCode` bằng:

```csharp
public int GetHashCode(Container obj) => HashCode.Combine(
    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.PortCode),
    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ContainerNumber));
```

Tham khảo [Program.cs](Program.cs). Copy thay đổi tương ứng vào `starter/Program.cs`, sau đó chạy `./scripts/verify.ps1`.

## Vì sao các fix khác sai
- Gọi `Distinct()` trước khi tạo `HashSet`: dùng comparer khác, không giải quyết contract.
- Chỉ `ToUpperInvariant()` trong `Equals`: hash vẫn không nhất quán.
- Đổi `HashSet` thành `List`: che lỗi, đánh đổi lookup O(1) trung bình thành O(n) và bỏ qua ràng buộc identity.

## Trade-offs
`OrdinalIgnoreCase` phù hợp mã định danh kỹ thuật ASCII-like; nếu đối tác quy định phân biệt case, cần sửa cả hợp đồng identity chứ không áp dụng ignore-case tùy tiện. Hash collision vẫn hợp lệ; điều kiện bắt buộc chỉ là hai đối tượng bằng nhau có cùng hash. Đo số lượng và lookup với nhiều biến thể, không chỉ một fixture.
