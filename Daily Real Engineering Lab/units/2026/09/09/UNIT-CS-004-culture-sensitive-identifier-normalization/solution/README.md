# Reference Solution

## Root cause

`ToUpper()` dùng `CurrentCulture`. Với `tr-TR`, ký tự `i` có uppercase là `İ`, nên `file-001` trở thành `FİLE-001` thay vì `FILE-001`. Nếu technical identifier được normalize bằng culture của process, cùng logical key có thể khác nhau giữa môi trường.

## Fix được khuyến nghị

Nếu domain định nghĩa SKU là case-insensitive theo technical/ordinal semantics, dùng comparer biểu đạt trực tiếp invariant đó:

```csharp
var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
```

Sau đó lookup bằng identifier gốc thay vì gọi `ToUpper()` theo culture.

`ToUpperInvariant()` có thể phù hợp trong một số canonicalization protocol đã được định nghĩa rõ, nhưng không nên normalize vô thức khi comparer đã thể hiện đúng comparison semantics.

## Wrong fixes

- ép process về `en-US`
- thay input test để không chứa `i`
- gọi `ToLower()` thay cho `ToUpper()` mà vẫn dùng `CurrentCulture`

Các cách này che triệu chứng thay vì sửa semantics.

## Trade-off

`OrdinalIgnoreCase` phù hợp với protocol keys, IDs, cache keys và nhiều technical identifiers. Text hiển thị cho người dùng lại thường cần culture-aware comparison/formatting. Chọn semantics theo domain, không theo thói quen.
