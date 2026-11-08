# UNIT-REGEXTO-001 — Archive tag validation stalls on malformed input

## Mục tiêu
Điều tra timeout trong validation .NET và thực hành hypothesis-driven debugging.

## Bối cảnh thực tế
Hệ thống lưu trữ tư liệu cộng đồng nhận tag do người dùng nhập. Một số batch có tag bất thường khiến worker timeout dù tag bình thường vẫn xử lý được.

## Bạn cần làm gì
Reproduce, ghi hai giả thuyết, thu thập stdout/stderr và exit code, sửa starter, chạy verify.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+, không cần dịch vụ ngoài.

## Chạy nhanh
`pwsh -File ./reproduce.ps1`, sau khi sửa `starter/Program.cs` chạy `pwsh -File ./verify.ps1`.

## Cách reproduce vấn đề
Script reproduce chỉ PASS nếu nhận đúng failure signature ban đầu.

## Những gì cần quan sát
Đối chiếu tag ngắn và tag dài, phân biệt invalid response với timeout; ghi thời gian và exit code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md).

## Expected Results
Reproduce xác nhận lỗi; verify kiểm tra đúng cả tag hợp lệ và malformed tag dài.

## Estimated Time
40 phút.
