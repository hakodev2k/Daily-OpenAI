# Reference Solution — spoiler

## Symptoms
Hai trạm khác nhau trong cùng ngày có thể nhận cùng một bản dự báo.

## Evidence
Log HIT cho SOUTH hiển thị responseStation=NORTH; upstream chỉ gọi hai lần thay vì ba.

## Root cause
Khóa cache chỉ bao gồm ngày, không có stationId. Hai request khác trạm chia sẻ cùng một entry.

## Why the fix works
Tạo khóa từ stationId chuẩn hóa và ngày; mỗi cặp định danh có namespace riêng. Bản sửa mẫu trong `Program.cs` giữ nguyên hành vi cache hit cho cùng cặp.

## How to verify
Áp dụng thay đổi tương ứng vào `starter/Program.cs`, chạy `./verify.ps1`. Kỳ vọng 3 upstream calls, đúng trạm cho từng response.

## Alternative fixes
Dùng typed composite key (record struct), hoặc per-station cache. Với tenant thực, thêm tenantId vào namespace; xác định timezone của ngày trước khi chuẩn hóa.

## Wrong or misleading fixes
Tắt cache che triệu chứng nhưng tăng tải. Chỉ tăng TTL hoặc xóa cache định kỳ không ngăn collision. Đổi thứ tự request chỉ đổi trạm bị ảnh hưởng.

## Production implications
Đo cache hit theo namespace với metric cardinality được kiểm soát, tránh log dữ liệu nhạy cảm. Phân biệt key collision với stale cache, cân nhắc invalidation và versioning.

## Trade-offs
Composite key tăng cardinality và bộ nhớ; đổi lại giữ tính đúng đắn. Khi dữ liệu thay đổi thường xuyên cần TTL/invalidation riêng.

## Senior engineer should notice
Cache key là một phần của contract về danh tính dữ liệu. Mọi tham số ảnh hưởng đến kết quả (station, tenant, date, locale, units) cần được xem xét khi xây key.
