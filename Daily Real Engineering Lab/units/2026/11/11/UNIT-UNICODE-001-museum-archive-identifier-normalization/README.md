# UNIT-UNICODE-001 — Mã hiện vật trông giống nhau nhưng lookup không khớp

## Mục tiêu
Điều tra lỗi identity dữ liệu text trong dịch vụ nhập hiện vật .NET bằng evidence ở mức ký tự và byte; bảo vệ contract lookup.

## Bối cảnh thực tế
Bảo tàng nhận accession ID từ hai nhà cung cấp. Nhân viên thấy hai ID giống nhau trên màn hình nhưng registry lưu hai bản ghi và lookup có kết quả không nhất quán.

## Bạn cần làm gì
Chạy starter, thu thập output, ghi hai giả thuyết, sửa starter và kiểm chứng bằng contract test.

## Yêu cầu môi trường
.NET SDK 8 và PowerShell. Không cần database hay network.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 ở reproduce nghĩa là contract đã thất bại như dự kiến.

## Những gì cần quan sát
Số lượng bản ghi, chiều dài ID, byte UTF-8, kết quả lookup và việc có gộp nhầm ID khác biệt hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md).

## Expected Results
Trước: hai ID tương đương được lưu riêng; sau: một identity logic, vẫn phân biệt ID thật sự khác nhau.

## Estimated Time
40 phút.
