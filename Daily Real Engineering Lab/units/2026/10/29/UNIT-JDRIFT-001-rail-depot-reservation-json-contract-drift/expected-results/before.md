# Before — starter nguyên bản

- `./reproduce.ps1` trả exit code **0** nếu triệu chứng dự kiến đã xuất hiện.
- Payload v1 hợp lệ trả `reservedUnits=7`.
- Payload R-101 có cách đặt tên field khác vẫn được chấp nhận nhưng object trả `reservedUnits=0`.
- Payload R-102 thiếu field số lượng cũng được chấp nhận với `reservedUnits=0`.
- `./verify.ps1` phải có ít nhất một test **FAIL** trên starter chưa sửa. Nếu mọi test PASS trước khi sửa, kiểm tra lại bạn có vô tình thay đổi `starter/` không.

Không yêu cầu đo latency hay timing chính xác.
