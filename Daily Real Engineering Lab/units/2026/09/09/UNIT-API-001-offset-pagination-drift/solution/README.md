# Reference Solution

## Root cause
Offset pagination phụ thuộc vị trí row trong tập dữ liệu hiện tại. Insert ở đầu feed làm offset phía sau dịch chuyển, gây duplicate/missing item giữa các page.

## Fix
Dùng keyset/cursor pagination theo stable unique ordering key. Sau page 1, lấy lastSeenId từ item cuối và query các item có Id < lastSeenId theo Id DESC.

## Trade-offs
Keyset pagination ổn định và scale tốt với index phù hợp nhưng không hỗ trợ jump-to-page-N như offset. Nếu sort key không unique, cursor phải chứa composite key đủ để ordering deterministic.
