# Wrong Fixes

## Thêm một index nữa trên cùng `CreatedUtc`
Không sửa hình dạng predicate; duplicated index còn tăng write/storage cost.

## Dùng `LIKE '2026-09-07%'`
Có thể hoạt động trong vài engine/collation nhưng semantics phụ thuộc representation và vẫn trộn data modeling với string matching.

## Dùng `BETWEEN start AND 23:59:59`
Dễ bỏ sót fractional seconds. Half-open range `[start, nextDay)` rõ ràng hơn.

## Cache kết quả ngay lập tức
Cache có thể che latency nhưng không giải quyết query access path và làm tăng invalidation complexity.
