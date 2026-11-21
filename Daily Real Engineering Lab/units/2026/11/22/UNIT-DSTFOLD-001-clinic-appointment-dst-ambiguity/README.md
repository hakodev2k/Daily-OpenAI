# UNIT-DSTFOLD-001 — Lịch hẹn phòng khám bị trùng giờ khi chuyển DST

## Mục tiêu
Điều tra hợp đồng chuyển giờ địa phương thành UTC và chứng minh lịch hẹn không bị xác nhận ở thời điểm không xác định duy nhất.

## Bối cảnh thực tế
API đặt lịch khám từ xa nhận ngày, giờ theo phòng khám tại New York. Trong đợt chuyển mùa, một số lịch có cùng giờ hiển thị nhưng bệnh nhân nhận thông báo tại những thời điểm khác nhau.

## Bạn cần làm gì
Chạy bộ replay ba yêu cầu đặt lịch, ghi lại status và UTC, lập ít nhất hai giả thuyết, sau đó sửa logic trong `starter/Program.cs` mà không thay đổi dữ liệu kiểm thử hoặc yêu cầu nghiệp vụ.

## Yêu cầu môi trường
.NET SDK 8, PowerShell, timezone data hỗ trợ America/New_York; không cần Azure, Docker hay database.

## Chạy nhanh
Từ thư mục unit: `./reproduce.ps1` (PowerShell). Để kiểm tra bản sửa: `./verify.ps1`.

## Cách reproduce vấn đề
Chạy `pwsh -File ./reproduce.ps1` hoặc `powershell -File ./reproduce.ps1`; script chấp nhận test thất bại dự kiến và báo lỗi nếu starter bất ngờ vượt qua toàn bộ hợp đồng.

## Những gì cần quan sát
So sánh từng input local time, status, UTC, và số lượng contract failures. Ghi lại trường hợp khác biệt và tác động với lịch hẹn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify bằng verify.ps1.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham chiếu](solution/README.md).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
50 phút.
