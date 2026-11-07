# Reference Solution — chỉ xem sau khi đã thử

## Symptoms
Các identifier tương đương theo contract của upstream có thể lookup khác kết quả chỉ vì casing.

## Evidence
Starter cho thấy key nguyên bản resolve, trong khi các biến thể casing không resolve; identifier không tồn tại cũng không resolve.

## Root cause
`Dictionary<string, string>` mặc định dùng equality semantics của `string` phù hợp với exact casing, trong khi business contract của external identifier là case-insensitive ordinal identity.

## Why the fix works
Khởi tạo dictionary với `StringComparer.OrdinalIgnoreCase` đặt identity semantics ngay tại collection boundary. Mọi lookup và duplicate-key check dùng cùng một policy.

## Reference change
```csharp
var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    ["SKU-ALPHA"] = "Product-101",
    ["SKU-BETA"] = "Product-202"
};
```

## How to verify
Chạy `./verify.ps1`. Hai casing variant phải resolve đúng product và `SKU-GAMMA` vẫn phải miss.

## Alternative fixes
Một value object cho identifier với equality semantics rõ ràng phù hợp hơn khi identifier xuất hiện xuyên nhiều layer hoặc có validation/normalization riêng.

## Wrong / tempting fixes
- Gọi `ToLower()` rải rác trước từng lookup làm policy phân tán và dễ bỏ sót.
- Dùng culture-sensitive casing cho machine identifier có thể tạo behavior phụ thuộc culture.
- Match gần đúng hoặc substring sẽ làm rộng identity contract ngoài yêu cầu.

## Production implications
Equality policy ảnh hưởng lookup, uniqueness và deduplication. Policy nên được quyết định tại boundary sở hữu identity contract và được test bằng examples từ upstream.

## Trade-offs
Case-insensitive identity chỉ đúng khi external contract thực sự nói casing không có ý nghĩa. Nếu casing có ý nghĩa, comparer này sẽ gộp hai identity khác nhau.

## Senior insight
Đừng coi string identifier chỉ là text. Nó có domain semantics về equality, normalization và uniqueness; collection phải phản ánh cùng contract đó.