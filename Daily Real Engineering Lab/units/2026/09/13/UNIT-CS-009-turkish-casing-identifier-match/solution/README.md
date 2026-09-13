# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms

Cùng logical identifier `INVOICE` / `invoice` được chấp nhận hoặc từ chối tùy culture của process.

## 2. Evidence

Starter cố định `CurrentCulture` thành `tr-TR`. Với culture này, casing của chữ `i/I` có quy tắc ngôn ngữ khác invariant/ordinal identifier semantics. Việc normalize cả hai phía bằng `ToUpper()` không đảm bảo tạo ra cùng chuỗi.

## 3. Root cause

Code dùng culture-sensitive casing để implement equality cho một identifier kỹ thuật. Comparison contract vì thế bị phụ thuộc môi trường runtime.

## 4. Fix tham chiếu

Thay lookup bằng comparison thể hiện trực tiếp contract:

```csharp
bool IsKnown(string candidate) =>
    configuredIds.Any(x => string.Equals(x, candidate, StringComparison.OrdinalIgnoreCase));
```

## 5. Vì sao fix hoạt động

`OrdinalIgnoreCase` thực hiện case-insensitive comparison theo ordinal semantics thay vì biến identifier thành natural-language text phụ thuộc `CurrentCulture`.

## 6. Verify

Chạy `./verify.ps1`. Script chạy chính learner-editable `starter/` và yêu cầu exit code `0`.

## 7. Alternative fixes

Nếu protocol quy định identifier case-sensitive, dùng `StringComparison.Ordinal` mới là đúng. Nếu dữ liệu thực sự là natural-language text dành cho người dùng, culture-aware comparison có thể phù hợp hơn. Contract phải quyết định API.

## 8. Wrong / Tempting Fixes

- Hard-code culture thành `en-US`: chỉ thay một implicit assumption bằng assumption khác.
- Gọi `ToLowerInvariant()` cho mọi loại string trong hệ thống: có thể chạy với case này nhưng biến normalization thành policy toàn cục không có căn cứ.
- Bắt lookup miss rồi thử nhiều casing: che contract và làm behavior khó dự đoán.

## 9. Production implications

Lỗi globalization thường xuất hiện sau deploy sang host/container có culture khác, hoặc khi process culture được cấu hình lại. Identifier, key, protocol token, route-like value và text hiển thị cần comparison policy khác nhau.

## 10. Trade-offs

Ordinal comparison phù hợp khi domain định nghĩa chuỗi như machine identifier. Culture-aware comparison phù hợp khi semantics là ngôn ngữ của người dùng. Không có một comparison mode đúng cho mọi string.

## 11. Senior engineer nên nhận ra

Đừng sửa casing trước rồi mới hỏi equality. Hãy xác định **string semantic contract** trước: case-sensitive hay insensitive, culture-aware hay ordinal, normalization có thuộc protocol hay không; sau đó chọn API thể hiện contract trực tiếp.
