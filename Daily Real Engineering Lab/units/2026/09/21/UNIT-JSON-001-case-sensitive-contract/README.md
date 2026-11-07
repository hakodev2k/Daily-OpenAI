# UNIT-JSON-001 — Case-Sensitive JSON Contract

## Mục tiêu
Điều tra vì sao một JSON document hợp lệ tạo ra model có identifier bằng giá trị mặc định.

## Bối cảnh thực tế
Một ứng dụng nhập order từ hệ thống đối tác. Cách viết hoa/thường của một property trong document khác với contract mà application đang dùng.

## Bạn cần làm gì
Chạy starter, quan sát model sau deserialize, xác định contract boundary bị lệch và sửa trong `starter/` để property được ánh xạ đúng.

## Yêu cầu môi trường
.NET 8 SDK và PowerShell 7+.

## Chạy nhanh
`./run.ps1`

## Cách reproduce vấn đề
`./reproduce.ps1`

## Những gì cần quan sát
Payload, model sau deserialize, serializer options và naming contract.

## Quy tắc làm lab
Chỉ sửa `starter/`. Không thay đổi sample input và không gán identifier thủ công sau deserialize.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler](solution/README.md)

## Expected Results
`./verify.ps1` báo PASS và model nhận đúng identifier.

## Estimated Time
35 phút.
