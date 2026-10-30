# Reference Solution — chỉ đọc sau khi tự sửa

## Symptoms

Thẻ còn document nhưng đã hết hạn hoặc bị thu hồi vẫn được gateway cho phép. Khi cleanup xóa document, hiện tượng tự biến mất.

## Evidence

Ba contract case fail trong starter; các case document absent hoặc chưa đến hạn vẫn đúng. Timeline cho thấy quyết định thay đổi theo sự tồn tại của document, không theo thời điểm hiệu lực.

## Root cause

AccessPolicy.CanConnect chỉ kiểm tra store.Find(passId) khác null. MongoDB TTL index là cơ chế dọn dữ liệu nền, không phải đồng hồ đảm bảo thu hồi quyền truy cập chính xác theo expiry. RevokedAtUtc cũng bị bỏ qua.

## Why the fix works

Xem [AccessPolicy.cs](AccessPolicy.cs). Quyết định DENY nếu không tìm thấy thẻ, nowUtc >= ExpiresAtUtc, hoặc RevokedAtUtc đã có hiệu lực. Thẻ hợp lệ được phép mà không phụ thuộc lượt cleanup. nowUtc được truyền tường minh để fixture kiểm chứng boundary.

## How to verify

Từ thư mục unit, sao chép file solution/AccessPolicy.cs vào starter/AccessPolicy.cs, rồi chạy:

~~~powershell
./verify.ps1
~~~

Script chạy chính starter/ vừa sửa, không chạy solution riêng. Kết quả mong đợi: bảy PASS, exit code 0. Chạy reproduce.ps1 sau sửa sẽ trả 1 vì triệu chứng gốc không còn; đây là hành vi đúng.

## Alternative fixes

Có thể truy vấn MongoDB với filter gồm id, expiresAtUtc > nowUtc và điều kiện revocation; cách này giảm nguy cơ caller quên kiểm tra. Có thể dùng signed token ngắn hạn nhưng nếu cần thu hồi tức thì vẫn phải có cơ chế kiểm tra trạng thái thu hồi hoặc chấp nhận cửa sổ rủi ro.

## Wrong / tempting fixes

- Tăng tần suất TTL monitor: không biến background deletion thành SLA authorization và còn có chi phí vận hành.
- Xóa document ngay khi thu hồi: có ích cho retention, nhưng không thay thế điều kiện expiry, xử lý lỗi hoặc consistency.
- Cache ALLOW 5 phút: làm chậm việc thu hồi trên nhiều gateway nếu không có invalidation chắc chắn.
- Dựa vào máy gateway có clock không đồng bộ: cần UTC clock discipline và giám sát lệch giờ; fixture không chứng minh clock production.

## Production implications and trade-offs

Truy vấn primary/consistency và policy tại request boundary có chi phí latency, nhưng đáp ứng security invariant tốt hơn cách suy ra quyền từ TTL. Cần kiểm tra SLA 5 giây trên hai replica, cơ chế retry có giới hạn, fail closed khi trạng thái không xác định, log audit không chứa secret và rollout canary.

## What a Senior engineer should notice

Dọn dữ liệu, tính hợp lệ nghiệp vụ và khả năng quan sát là ba trách nhiệm khác nhau. Local simulator chỉ chứng minh policy contract; phải đo production latency, consistency, revocation propagation và lỗi phụ thuộc trước khi kết luận kiến trúc đạt SLA.

Đọc [ADR tham khảo](decision-record.md) để so sánh các lựa chọn.
