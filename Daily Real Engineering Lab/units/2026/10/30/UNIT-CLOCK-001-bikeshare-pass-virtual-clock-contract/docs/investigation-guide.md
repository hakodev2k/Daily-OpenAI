# Investigation Guide

Không đọc `solution/` trước khi ghi lại bằng chứng.

1. Chạy `reproduce.ps1` và `verify.ps1`. Hai lệnh có tiêu chí thành công khác nhau.
2. Dựng bảng sáu mốc: `atStart`, `beforeExpiry`, `atExpiry`, `beforeStart`, `invalidWindow`, `afterRewind`. Đánh dấu kỳ vọng và thực tế.
3. Nêu ít nhất ba hypothesis: ranh giới `>=`, chuyển đổi UTC, và sự khác biệt giữa đồng hồ của harness với đồng hồ của domain service.
4. Đọc `harness/Program.cs` để hiểu nguồn dữ liệu và cách clock được điều khiển. Đọc `starter/PassGate.cs` nhưng chưa sửa cho đến khi xác định giả thuyết bằng chứng.
5. Sửa nhỏ nhất có thể, không đổi contract `[start, end)`. Verify cả điểm ngay trước và đúng expiry, sau đó thử tua thời gian trở lại.
6. Đánh giá: vì sao absolute expiration nên dựa trên UTC clock được cấp? Khi đo duration/latency, nên dùng nguồn thời gian nào?

## Evidence checklist

- Bảng expected/actual cho sáu mốc.
- Hypothesis bị bác bỏ bằng một quan sát cụ thể.
- Diff nhỏ nhất của bản sửa.
- Kết quả `verify.ps1` sau sửa và trường hợp invalid interval.
- Rủi ro nếu có nhiều nguồn clock trong cùng một request.

## Không thể reproduce?

Kiểm tra .NET 8 SDK, `global.json`, chạy lại `setup.ps1`; không chỉnh giờ hệ điều hành. Harness đặt dữ liệu ở tương lai hai ngày so với lúc bắt đầu chạy, không dùng `Task.Delay`.
