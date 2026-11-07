# UNIT-OVERFLOW-001 — Sản lượng điện âm trong báo cáo trang trại mặt trời

## Mục tiêu
Điều tra một sai lệch tổng hợp số liệu ở ranh giới dữ liệu lớn; xác định cơ chế bằng evidence và viết regression test.

## Bối cảnh thực tế
Hệ thống nhận chỉ số Wh theo từng inverter. Dashboard cuối ngày có lúc báo tổng sản lượng âm dù mọi bản ghi nguồn đều dương. Tài chính đang đối chiếu số liệu với hệ thống vận hành.

## Bạn cần làm gì
Reproduce, lưu output, đưa ra ít nhất hai giả thuyết, sửa trong `starter/Program.cs`, chạy verify và giải thích tác động của thay đổi kiểu dữ liệu lên API/DB.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ (hoặc chạy `dotnet run` trực tiếp). Không cần database hay cloud.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`; script kiểm tra chương trình trả về tổng không khớp với oracle được tính độc lập. Exit code 0 của reproduce có nghĩa triệu chứng đã xuất hiện.

## Những gì cần quan sát
So sánh các dòng `INPUT`, `ACTUAL` và `EXPECTED`. Đọc dấu và độ lớn của giá trị; thử dữ liệu nhỏ và dữ liệu gần ngưỡng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Trước: dữ liệu lớn cho kết quả sai; sau: cả bộ dữ liệu nhỏ, lớn và biên đều khớp oracle. Xem [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
35–45 phút.
