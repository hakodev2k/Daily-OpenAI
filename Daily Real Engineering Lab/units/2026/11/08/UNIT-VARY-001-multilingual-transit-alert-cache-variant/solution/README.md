# Reference solution — SPOILER
## Root cause
`CacheKeyBuilder.Build` chỉ đưa route vào key. Bản tin tiếng Việt và tiếng Anh bị coi là cùng một object; first writer wins. Đây là lỗi cache-variant identity, không phải lỗi translation hoặc TTL.

## Fix
Thay nội dung `starter/CacheKeyBuilder.cs` bằng [reference](CacheKeyBuilder.cs). Sau đó chạy `./verify.ps1` trên starter.

## Trade-offs
Trong production, cần xác định tập language được hỗ trợ, fallback locale, `Vary: Accept-Language` với HTTP caches, tránh cache poisoning do arbitrary headers, và cache invalidation theo phiên bản nội dung. Không đưa toàn bộ raw `Accept-Language` vào key khi chưa chuẩn hóa.

## Wrong fixes
- Xóa cache mỗi request: che symptom nhưng loại bỏ lợi ích cache.
- Tăng TTL: khiến nhiễm biến thể tồn tại lâu hơn.
- Dùng khóa theo user ID: phân mảnh cache và vẫn không định danh ngôn ngữ khi user đổi locale.
- Chỉ test `vi -> vi`: không kiểm tra cross-variant isolation.
