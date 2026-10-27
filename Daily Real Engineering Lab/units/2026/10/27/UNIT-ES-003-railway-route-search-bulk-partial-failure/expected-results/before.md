# Before
- `reproduce.ps1` in `REPRODUCED_EXPECTED_FAILURE` khi starter còn nguyên.
- HTTP_STATUS=200; starter báo bốn document thành công.
- Gateway thực tế chỉ index `route-101`, `route-104`; các tuyến còn lại chưa được xử lý đúng.
- `route-102` và `route-103` mỗi tuyến mới được thử một lần.
- Nếu starter đã được sửa, `reproduce.ps1` không còn được kỳ vọng thành công; xem Git history hoặc reset riêng file learner để reproduce lại.
