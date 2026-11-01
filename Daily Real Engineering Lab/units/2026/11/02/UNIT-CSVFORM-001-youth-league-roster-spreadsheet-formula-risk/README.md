# UNIT-CSVFORM-001 — Danh sách giải đấu trẻ: CSV hợp lệ nhưng ô bảng tính bị cảnh báo

## Mục tiêu
Điều tra ranh giới tin cậy khi xuất dữ liệu đăng ký từ .NET sang spreadsheet, dựa trên bằng chứng ở mức ô và regression contract.

## Bối cảnh thực tế
API của giải bóng đá trẻ xuất 8 đăng ký cho ban tổ chức. CSV mở được, HTTP 200, số dòng đúng; tuy nhiên hệ thống kiểm tra an toàn dữ liệu báo một số ô có nguy cơ được spreadsheet diễn giải ngoài ý muốn. Các field do người dùng bên ngoài cung cấp.

## Bạn cần làm gì
1. Reproduce và ghi số ô rủi ro.
2. Viết ba hypothesis trong workspace/my-investigation.md.
3. Sửa starter/RosterCsv.cs; không thay Program.cs hoặc contract để làm test xanh.
4. Chạy verify.ps1 trên starter đã sửa; sau đó mới xem reference solution.

## Yêu cầu môi trường
.NET 8 SDK, Windows PowerShell 5.1+ hoặc PowerShell 7. Không cần Excel, Docker, cloud hay package ngoài.

## Chạy nhanh
Mở PowerShell tại thư mục unit:
    ./setup.ps1
    ./reproduce.ps1
    ./verify.ps1

## Cách reproduce vấn đề
Chạy ./reproduce.ps1 với starter gốc; ghi lại Registrations, ParsedRows, RiskyCells và exit code. Không mở hoặc thực thi bất kỳ nội dung công thức nào.

## Những gì cần quan sát
- Cấu trúc CSV và số dòng có đúng không?
- Các field có dấu phẩy, ngoặc kép, xuống dòng có round-trip được không?
- Vì sao HTTP 200 và CSV parser PASS không chứng minh an toàn khi mở bằng spreadsheet?
- Bằng chứng nào giúp phân biệt lỗi cấu trúc CSV với lỗi tại ranh giới ứng dụng?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) / [After](expected-results/after.md).

## Estimated Time
60–75 phút. Đây là mô phỏng rủi ro cell, không thực thi công thức và không bảo đảm mô phỏng chính xác mọi phiên bản Excel/LibreOffice.
