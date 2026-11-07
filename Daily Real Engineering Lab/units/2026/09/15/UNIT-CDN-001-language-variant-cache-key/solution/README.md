# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms
Cùng URL trả `200`, nhưng body có thể thuộc locale của request trước đó đã làm ấm edge cache.

## Evidence
Origin function dùng cả `path` và `Accept-Language`; edge identity trong starter chỉ dùng `path`. Request thứ hai vì vậy có thể trở thành hit vào object của representation khác.

## Root cause
Cache key không chứa một request dimension có ảnh hưởng trực tiếp đến representation. Hai representation khác nhau bị gộp thành cùng một cache object.

## Why the fix works
Cache identity phải tương thích với representation contract. Trong simulator, cách tối thiểu là normalize locale rồi đưa locale vào key, ví dụ `path + "|lang=" + locale`. Trong HTTP/CDN thật, cấu hình cache key và response variant policy phải thống nhất; tùy CDN có thể cấu hình header vào cache key hoặc thiết kế URL/variant khác.

## Reference change
Trong `EdgeCache.Get`, thay:
```csharp
var cacheKey = path;
```
bằng một identity có locale đã normalize, ví dụ:
```csharp
var locale = acceptLanguage.StartsWith("vi", StringComparison.OrdinalIgnoreCase) ? "vi" : "en";
var cacheKey = $"{path}|lang={locale}";
```
Sau đó chạy `./verify.ps1` trên chính `starter/` đã sửa.

## How to verify
Cả bốn dòng phải có `match=True`, đồng thời vẫn phải xuất hiện `EDGE-HIT` cho request lặp lại của mỗi locale.

## Alternative fixes
- Dùng locale trong URL (`/vi/...`, `/en/...`) để cache identity tự nhiên khác nhau.
- Cấu hình CDN cache key include header đã normalize nếu nền tảng hỗ trợ và cardinality được kiểm soát.
- Không cache response này nếu biến thể quá phức tạp hoặc correctness quan trọng hơn lợi ích cache.

## Wrong / tempting fixes
- Purge cache: chỉ xóa symptom tạm thời; request làm ấm tiếp theo lại quyết định variant sai.
- TTL rất ngắn: giảm cửa sổ lỗi nhưng không sửa correctness contract.
- Include nguyên chuỗi `Accept-Language` không normalize: có thể tạo cardinality lớn vì nhiều tổ hợp header tương đương về business locale.
- Tắt cache toàn bộ: có thể đúng về correctness nhưng mất lợi ích edge; chỉ hợp lý nếu constraint thực tế cho phép.

## Production implications
Với CDN như Akamai hoặc các edge cache khác, cache-key policy là một phần của application correctness, không chỉ performance tuning. Cần inventory mọi dimension làm representation thay đổi: locale, device class, auth/public boundary, query parameters, content encoding và các feature variant liên quan.

## Trade-offs
Thêm dimension vào key tăng số object và giảm hit ratio. Normalize quá mạnh lại có thể gộp các representation thực sự khác nhau. Thiết kế đúng là tối thiểu hóa cardinality nhưng vẫn giữ identity đủ để bảo toàn correctness.

## Senior engineer should notice
Cache hit ratio cao không chứng minh cache policy đúng. Trước khi tối ưu hit ratio, phải định nghĩa representation identity và kiểm tra cache boundary có cùng contract với origin hay không.
