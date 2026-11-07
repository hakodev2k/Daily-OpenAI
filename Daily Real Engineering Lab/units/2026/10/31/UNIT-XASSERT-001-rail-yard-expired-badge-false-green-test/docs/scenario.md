# Incident brief — Rail maintenance yard access

Quy tắc nghiệp vụ tại cổng nhà thầu:
- `Revoked=true`: từ chối.
- `ExpiresAtUtc <= nowUtc`: từ chối (đúng thời điểm hết hạn cũng phải từ chối).
- Còn hạn và chưa bị thu hồi: chấp nhận.

Timeline mô phỏng:
- 06:50 UTC: CI test suite xanh.
- 06:55 UTC: kỹ thuật viên kiểm tra độc lập, thẻ hết hạn một phút vẫn được cho phép.
- 07:00 UTC: đội backend cần xác định **policy sai**, **test sai**, hay cả hai.

Nguồn chứng cứ: `dotnet test` exit code, `AcceptanceProbe` output, nội dung `starter/RailYard.Access.Tests` và `starter/RailYard.Access`. Không có database, network hay clock thực; timestamp cố định giúp tái hiện.
