# UNIT-CACHEAGE-001 — Bản tin sơ tán không cập nhật

## Mục tiêu
Điều tra freshness của cache bằng evidence có clock mô phỏng.

## Bối cảnh thực tế
Một dịch vụ thông báo sơ tán thay đổi chỉ dẫn tại phút 12 nhưng dashboard vẫn hiển thị bản cũ ở phút 16. CPU bình thường và hệ thống không có exception.

## Bạn cần làm gì
Tái hiện triệu chứng, lập giả thuyết, chỉnh `starter/Program.cs` để thông tin không cũ quá 10 phút kể từ lần lấy nguồn, đồng thời loại entry không được truy cập trong 5 phút.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell. Không cần dịch vụ bên ngoài.

## Chạy nhanh
`./reproduce.ps1`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`, đọc timeline t=00,04,08,12,16. Exit 0 ở đây nghĩa là đã chứng minh triệu chứng.

## Những gì cần quan sát
Ghi displayed/source/fetches ở mỗi mốc, thời điểm sai lệch và giả thuyết giải thích vì sao số lần fetch thấp.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong workspace.
3. Thử fix trên starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md)

## Expected Results
Trước: dữ liệu cũ ở phút 16. Sau: dữ liệu mới ở phút 16, không regress số lần fetch.

## Estimated Time
55 phút.
