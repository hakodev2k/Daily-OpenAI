# Reference Solution — xem sau khi tự điều tra

## 1. Symptoms

`case-1042` xuất hiện trong seed/query evidence nhưng repository point read trả 404.

## 2. Evidence

Stored address là `id=case-1042`, `partitionKey=tenant-acme`; request mang business tenant code `ACME`.

## 3. Root cause

Repository dùng trực tiếp business tenant code làm partition-key value. Point read yêu cầu đúng cả `id` và partition key; `ACME` không bằng `tenant-acme`, nên lookup hợp lệ về mặt API nhưng không định vị được item.

## 4. Why the fix works

Canonical partition-key derivation biến tenant code thành cùng representation đã dùng khi lưu item. Khi read và write cùng dùng một contract, point read định vị đúng logical partition và item.

## 5. How to verify

Chạy `../verify.ps1`. Known item phải 200; unknown item vẫn 404.

## 6. Alternative fixes

- Lưu partition key canonical trực tiếp trong request context sau tenant resolution và truyền xuống repository.
- Dùng một value object `TenantPartitionKey` để tránh truyền nhầm raw tenant code.
- Nếu schema hiện tại sai và cần partition strategy mới, thực hiện migration có kế hoạch thay vì sửa read path bằng scan.

## 7. Wrong / misleading fixes

- Retry cùng `id` + partition key sai: chỉ lặp lại lookup sai.
- Query toàn container theo `id`: có thể che contract bug, tăng RU/latency và mất lợi ích point read.
- Hard-code `tenant-acme`: sửa một tenant nhưng phá abstraction.

## 8. Production implications

Partition-key derivation là một phần của persistence contract. Thay đổi normalization/prefix/casing mà không migration có thể khiến dữ liệu cũ trở nên 'mất tích' với point read dù vẫn tồn tại vật lý.

## 9. Trade-offs

Canonicalization helper đơn giản và rẻ, nhưng phải được version/migration-aware nếu production đã có nhiều representation lịch sử.

## 10. What a Senior engineer should notice

`404` ở document database không luôn đồng nghĩa item bị xóa. Với point read, phải kiểm tra đầy đủ logical address và consistency của data contract trước khi mở rộng sang retry, scan hoặc hạ tầng.