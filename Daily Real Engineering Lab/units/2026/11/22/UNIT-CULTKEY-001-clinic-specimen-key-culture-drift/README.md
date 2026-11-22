# UNIT-CULTKEY-001 — Mã mẫu xét nghiệm tra cứu khác nhau giữa các máy chủ

## Mục tiêu
Điều tra sự không nhất quán của định danh nghiệp vụ khi ứng dụng chạy trên nhiều cấu hình hệ thống; đưa ra hợp đồng so sánh và kiểm chứng hồi quy.

## Bối cảnh thực tế
Hai node của hệ thống tiếp nhận mẫu xét nghiệm cùng xử lý một mã specimen, nhưng một node không tìm thấy mẫu dù bản ghi đã được nhập. Không có database hay API ngoài trong bản mô phỏng.

## Bạn cần làm gì
Reproduce, ghi bằng chứng, đề xuất ít nhất hai giả thuyết, sửa mã trong `starter/Program.cs`, chạy verify và so sánh lời giải sau cùng.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần NuGet bên ngoài.

## Chạy nhanh
Tại thư mục unit: `./scripts/reproduce.ps1`. Sau khi sửa: `./scripts/verify.ps1`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` từ unit root. Script chạy starter với hai cấu hình và xác nhận ít nhất một sai lệch xảy ra.

## Những gì cần quan sát
Ghi lại `culture`, `found`, `value` và tập `keys` trong output; so sánh với kết quả nghiệp vụ mong muốn. Đừng chỉ kiểm tra exit code.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md).

## Expected Results
Trước sửa: reproduce thành công vì phát hiện sai lệch. Sau sửa: verify thành công trên cả hai cấu hình, không làm sai kết quả mã khác.

## Estimated Time
35–45 phút.
