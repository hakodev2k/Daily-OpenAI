# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Sau khi một recipient đã được thêm vào `HashSet<Recipient>`, bước normalize thay đổi `Email`. Một object mới đại diện cùng logical recipient sau đó không được `Contains` tìm thấy và `Add` trả về `true`, khiến set có hai phần tử tương đương về mặt business.

## Evidence

- `ContainsBeforeSecondAdd=False`
- `SecondAddReturned=True`
- `FinalCount=2`
- Hai recipient cuối cùng có cùng `TenantId` và cùng email sau normalization.

## Root cause

`Recipient.GetHashCode()` phụ thuộc vào `Email`. Object được insert khi `Email` còn là `" User@Example.com "`, nên nó được đặt vào bucket theo hash code của giá trị đó. Sau khi `Email` bị mutate thành `"user@example.com"`, hash code của chính object thay đổi nhưng `HashSet` không tự re-bucket phần tử đã lưu. Lookup cho logical recipient mới đi tới bucket của hash code mới và không thấy phần tử cũ.

Equality/hash invariants của key trong hash-based collection đã bị phá vỡ.

## Why the fix works

Cách đơn giản nhất cho scenario này là normalize dữ liệu trước khi đưa object vào set và không mutate các field tham gia equality trong thời gian object nằm trong set.

Ví dụ:

```csharp
var first = new Recipient
{
    TenantId = "acme",
    Email = NormalizeEmail(" User@Example.com ")
};

recipients.Add(first);

var sameLogicalRecipient = new Recipient
{
    TenantId = "acme",
    Email = NormalizeEmail("user@example.com")
};
```

Một thiết kế mạnh hơn là dùng immutable value key:

```csharp
public readonly record struct RecipientKey(string TenantId, string NormalizedEmail);
```

rồi lưu `HashSet<RecipientKey>` thay vì mutable domain object.

## How to verify

Sau fix:

- `ContainsBeforeSecondAdd=True`
- `SecondAddReturned=False`
- `FinalCount=1`
- `verify.ps1` kết thúc với `PASS`.

## Alternative fixes

- Remove item trước khi mutate rồi add lại sau mutation. Cách này có thể đúng về mặt collection invariant nhưng dễ sai ở production nếu mutation nằm ở nhiều nơi.
- Dùng custom immutable key derived từ business identity.
- Dùng dictionary keyed bởi normalized immutable identifier nếu cần map thêm dữ liệu.

## Wrong or misleading fixes

### Gọi `Distinct()` ở cuối pipeline

Có thể che duplicate ở output nhưng không sửa invariant bị phá trong collection. Những lookup khác vẫn có thể sai.

### Chuyển sang `List<T>` rồi dùng `Any`

Có thể tránh hash invariant nhưng đổi lookup từ gần O(1) sang O(n) và né root cause thay vì thiết kế identity ổn định.

### Chỉ sửa `Equals` mà không sửa `GetHashCode`

Hash-based collections yêu cầu equality và hash code nhất quán. Hai object bằng nhau phải có hash code tương thích.

### Rebuild `HashSet` sau mọi mutation

Có thể phục hồi bucket layout nhưng làm lifecycle khó kiểm soát và không giải quyết thiết kế mutable key.

## Production implications

Mutable keys có thể gây deduplication failure, cache lookup miss, dictionary entry "biến mất", duplicate side effects và behavior khó tái hiện nếu mutation chỉ xảy ra ở một nhánh pipeline.

## Trade-offs

- Normalize-at-boundary đơn giản và hiệu quả khi canonicalization rule rõ ràng.
- Immutable value key làm invariant rõ hơn nhưng thêm một abstraction nhỏ.
- Mutable domain object vẫn có thể tồn tại, nhưng không nên trực tiếp làm hash key nếu các field identity của nó còn thay đổi.

## What a Senior engineer should notice

Senior engineer nên đặt câu hỏi về business identity, canonicalization boundary và lifetime của key thay vì chỉ sửa symptom ở `HashSet`. Invariant quan trọng là: trong thời gian một key nằm trong hash-based collection, các giá trị quyết định equality/hash phải ổn định.
