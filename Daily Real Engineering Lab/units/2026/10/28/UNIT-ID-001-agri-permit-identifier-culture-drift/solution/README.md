# Reference Solution — SPOILER

Chỉ đọc sau khi reproduce và thử sửa `starter/`.

## 1. Symptoms

Cùng ba revision nhưng worker ở `en-US` có 2 permit, worker ở `tr-TR` có 3; truy vấn mã `INSP-204` ở worker thứ hai trả trạng thái cũ.

## 2. Evidence

`--reproduce` cố định thứ tự và nội dung feed; thay đổi duy nhất là culture của worker. `INSP-205` vẫn độc lập, còn hai biến thể `INSP-204` / `insp-204` không được hợp nhất đồng nhất.

## 3. Root cause

`StringComparer.CurrentCultureIgnoreCase` sử dụng quy tắc chữ hoa/chữ thường của locale hiện hành. Trong tiếng Thổ Nhĩ Kỳ, `I` và `i` không phải cặp tương đương case-insensitive như trong tiếng Anh. Dictionary coi hai chuỗi là hai key, khiến revision sau không thay thế revision trước.

## 4. Why the fix works

Hợp đồng đối tác xác định permit ID là ASCII technical identifier, so sánh không phân biệt case nhưng không phụ thuộc locale. `StringComparer.OrdinalIgnoreCase` cung cấp equality/hash nhất quán giữa các worker. Giữ nguyên last-revision-wins và feed order.

File [PermitRegistry.cs](PermitRegistry.cs) chứa bản sửa. Copy file đó vào `starter/PermitRegistry.cs` **sau khi tự thử**, hoặc áp dụng đúng một thay đổi tương đương.

## 5. How to verify

~~~powershell
./verify.ps1
~~~

Phải thấy `PASS` ở ba locale. Verification chạy trực tiếp code trong `starter/`, không chạy reference solution.

## 6. Alternative fixes

Có thể canonicalize ID tại ingestion boundary với ASCII-only uppercasing và sử dụng `StringComparer.Ordinal` cho key đã chuẩn hóa; cần đảm bảo mọi producer/consumer tuân theo cùng contract, và quyết định rõ có cho phép non-ASCII hay không. Với bài này, ordinal-ignore-case trực tiếp là đơn giản hơn.

## 7. Wrong / tempting fixes

- Chuyển toàn bộ process sang `en-US`: che lỗi và làm thay đổi behavior không liên quan của worker.
- Dùng `ToLower()` / `ToUpper()` không truyền culture: vẫn có thể phụ thuộc locale.
- Bỏ revision trùng hoặc đổi thứ tự feed: làm mất dữ liệu, không giải quyết identity.
- Dùng `InvariantCultureIgnoreCase` một cách mặc định: có thể phù hợp cho một số text contract, nhưng technical identifiers cần quy tắc ordinal đã được định nghĩa rõ.

## 8. Production implications

Nếu các instance dùng locale khác nhau, deduplication và audit có thể diverge. Nên chuẩn hóa contract tại boundary, thêm cross-culture regression tests và kiểm tra tính tương thích với các key đã lưu trong DB/cache. Nếu dữ liệu cũ đã bị tách thành nhiều key, cần migration/reconciliation có audit.

## 9. Trade-offs

`OrdinalIgnoreCase` phù hợp với technical ASCII identifiers nhưng không phải phép so sánh ngôn ngữ tự nhiên. Nếu business yêu cầu Unicode identifiers, cần định nghĩa normalization, case folding và collision policy trước khi triển khai.

## 10. Senior engineer should notice

Sự khác nhau giữa **business identity** và **display text**; cùng một comparer phải dùng xuyên suốt ingestion, lookup, cache, database uniqueness và service boundaries. Test chỉ trên một locale không đủ chứng minh tính đúng của distributed identity contract.
