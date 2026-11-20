# Reference — Spoiler
OFFSET phụ thuộc vị trí hiện tại: sau insert 105 ở đầu, 103 dịch từ thứ hai sang thứ ba, nên trang 2 lặp 103. ORDER BY deterministic không ngăn dịch chuyển.

## Keyset solution
Trang đầu SELECT TOP (2) ... ORDER BY CreatedAt DESC,DonationId DESC. Trả cursor gồm lastCreatedAt,lastId,filter hash,version,expiry (ký HMAC nếu client có thể sửa). Trang sau:
```sql
SELECT TOP (@size) DonationId,CreatedAt FROM Donations
WHERE CreatedAt < @lastCreatedAt OR (CreatedAt = @lastCreatedAt AND DonationId < @lastId)
ORDER BY CreatedAt DESC,DonationId DESC;
```
Sau cursor 103 trả [102,101] dù 105 vừa thêm. Dùng index (CreatedAt DESC,DonationId DESC), có filter prefix nếu cần. Verify: timestamp đồng hạng dùng DonationId tie breaker; invalid cursor/filter mismatch trả 400.

## Snapshot alternative
Nếu kiểm toán yêu cầu cùng một tập bất biến qua nhiều HTTP requests, materialize export theo exportId tại cutoff; keyset một mình không đảm bảo snapshot khi update sort key hoặc delete. Transaction snapshot chỉ có hiệu lực trong transaction, không tự xuyên nhiều requests.

## Wrong fixes
Tăng page size chỉ che lỗi; ORDER BY CreatedAt thiếu tie breaker; NOLOCK tăng bất nhất; retry không bảo đảm ổn định.

## Trade-offs
Keyset phù hợp append-only feed và không hỗ trợ nhảy tùy ý tới page N; materialized export tốn storage/job nhưng có audit reproducibility.
