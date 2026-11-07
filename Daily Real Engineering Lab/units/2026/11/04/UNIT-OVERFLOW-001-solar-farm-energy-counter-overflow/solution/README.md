# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Tổng Wh trong kịch bản peak âm; kịch bản small vẫn đúng.

## Evidence
Peak có tổng chính xác 3,300,000,000 Wh; vượt 2,147,483,647. Boundary có 2,147,483,648 Wh. Oracle `Sum(v => (long)v)` tính với accumulator 64-bit.

## Root cause
Accumulator `int` cộng trong `unchecked`, gây wraparound trước khi giá trị được chuyển thành `long` khi return. Kiểu return lớn không cứu được phép cộng đã xảy ra.

## Why the fix works
Đổi accumulator thành `long` trước phép cộng; dùng `checked` để phát hiện vượt giới hạn 64-bit thay vì tạo số liệu giả.

## How to verify
Chỉnh `starter/Program.cs`, chạy `./verify.ps1`. Cả small, peak, boundary phải đúng và process exit 0.

## Alternative fixes
`values.Sum(v => (long)v)` là cách ngắn gọn; `decimal` thích hợp khi mô hình dữ liệu đòi số thập phân hoặc giới hạn tổng vượt long. Cân nhắc streaming thay vì materialize nếu input lớn.

## Wrong / tempting fixes
Đổi duy nhất return type thành `long` không thay đổi phép cộng. Ép kiểu `(long)total` sau vòng lặp quá muộn. `unchecked` chỉ che lỗi. Bỏ dữ liệu lớn làm sai báo cáo.

## Production implications
Kiểm tra schema DB, DTO, JSON client (đặc biệt JavaScript safe integer), cảnh báo tính toàn vẹn và reconciliation với nguồn gốc. Phân biệt Wh nguyên với kWh có phần thập phân.

## Trade-offs
`long` đơn giản, nhanh và đủ cho miền dữ liệu có giới hạn rõ; `decimal` có phạm vi và chi phí khác. `checked` làm lỗi rõ ràng nhưng cần chính sách xử lý/alert.

## Senior engineer should notice
Kiểm tra overflow tại từng bước biến đổi, không chỉ tại kiểu dữ liệu đầu ra. Test phải chứa ranh giới và dữ liệu production-scale; định nghĩa hợp đồng đo lường/đơn vị và chính sách lỗi.
