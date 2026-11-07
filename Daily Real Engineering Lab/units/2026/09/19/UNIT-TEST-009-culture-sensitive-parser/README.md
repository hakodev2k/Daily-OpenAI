# UNIT-TEST-009 — Parser Passes Locally but Fails Under Another Culture

## Mục tiêu
Điều tra một lỗi parsing phụ thuộc môi trường và biến nó thành behavior deterministic.

## Bối cảnh thực tế
Một worker import giá từ partner xử lý cùng một file trên máy developer và build agent. Dữ liệu đầu vào giống hệt nhau nhưng một số giá trị lại được hiểu khác khi process chạy với regional settings khác.

## Bạn cần làm gì
Reproduce hiện tượng, ghi hypothesis, xác định contract của dữ liệu đầu vào, sửa starter và chứng minh cùng input tạo cùng kết quả dưới nhiều culture.

## Yêu cầu môi trường
- .NET SDK 8.0.x
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy cùng parser dưới hai culture cố định và kiểm tra kết quả business.

## Những gì cần quan sát
- Input không đổi.
- Culture của process thay đổi.
- Giá trị parse hoặc kết quả validation có thể thay đổi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Spoiler](solution/README.md)

## Expected Results
Trước fix, ít nhất một culture tạo kết quả khác contract. Sau fix, `verify.ps1` báo `VERIFY_PASS` cho cả hai culture.

## Estimated Time
30 phút.
