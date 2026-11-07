# Reference Solution

## Symptoms
Backend logic đúng theo language, nhưng shared cache có thể trả response của request trước.

## Evidence
Hai request cùng resource path nhưng khác language tạo ra expected representation khác nhau; starter chỉ có một cache identity cho path.

## Root cause
Cache identity không chứa dimension làm representation thay đổi.

## Why fix works
Variant-aware key tách các representation có semantics khác nhau.

## How verify
Chạy verify.ps1 sau khi sửa starter; cả vi và en phải nhận đúng content trong cùng process.

## Alternative fixes
Chuẩn hóa locale vào URL; dùng cache policy/header phù hợp với CDN; không cache shared khi variation contract không thể biểu diễn an toàn.

## Wrong or misleading fixes
Giảm TTL chỉ làm lỗi ít tồn tại lâu hơn. Purge cache không sửa contract. Scale backend không thay đổi cache identity.

## Production implications
Cache key policy phải khớp origin variation và được test với proxy/CDN thực tế.

## Trade-offs
Thêm dimensions làm tăng cache cardinality và giảm hit ratio; URL-based locale có thể đơn giản hóa vận hành nhưng ảnh hưởng routing/SEO.

## What a Senior engineer should notice
Caching là correctness contract trước khi là performance optimization.