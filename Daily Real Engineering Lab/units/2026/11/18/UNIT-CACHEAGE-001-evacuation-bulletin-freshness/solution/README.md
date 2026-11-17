# Reference Solution — chỉ đọc sau khi tự sửa

## Symptoms
Tại t=16, displayed vẫn là ZONE-A-OPEN dù source đã đổi.

## Evidence
Đọc mỗi 4 phút, source đổi t=12, fetches=1 ở t=16.

## Root cause
Sliding expiration chỉ kiểm tra khoảng cách từ lượt đọc gần nhất; entry nóng không bao giờ hết hạn. maxAge đã được truyền nhưng không áp dụng.

## Why the fix works
Theo dõi fetchedMinute độc lập với lastAccessMinute; refresh khi idle TTL hoặc absolute max age hết hạn. Đọc cache không gia hạn tuổi dữ liệu.

## How to verify
Sửa starter và chạy `./verify.ps1`. Reference: `dotnet run --project ./solution/Lab.csproj -- verify`. Tại t=12 bản mới được lấy, fetches=2; t=16 bản mới vẫn đúng.

## Alternative fixes
Push invalidation có thể nhanh hơn nhưng cần delivery, ordering và recovery; absolute TTL phù hợp khi freshness SLA cho phép.

## Wrong fixes
Tăng idle TTL làm stale lâu hơn; restart service che triệu chứng; xóa cache mỗi lần đọc gây tải nguồn.

## Production implications
Đo tuổi dữ liệu và version, không chỉ hit ratio; xác định fail-safe khi upstream lỗi, có thể cần SLA ngắn hơn 10 phút.

## Trade-offs
TTL ngắn tăng tải; TTL dài tăng stale window. Cân nhắc tính chất an toàn của bản tin.

## Senior engineer should notice
Kiểm thử tại đúng boundary 10 phút, đồng thời kiểm thử nguồn lỗi và nhiều instance cache.
