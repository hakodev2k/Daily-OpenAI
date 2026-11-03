# UNIT-DECROUND-001 — Bike-share settlement

## Mục tiêu
Điều tra chênh lệch giữa phí chuyến xe nội bộ và bảng kê đối tác, thiết lập regression test cho ranh giới tiền tệ.

## Bối cảnh thực tế
Đối tác yêu cầu quyết toán **theo từng chuyến**, 2 chữ số thập phân. Phí thô R-101=1.005, R-102=2.015, R-103=3.125, R-104=4.234. Bảng kê chính thức lần lượt 1.01, 2.02, 3.13, 4.23; tổng 10.39. Một số chuyến bị lệch, một số vẫn khớp.

## Bạn cần làm gì
Chạy starter, thu thập bảng chênh lệch, ghi ít nhất hai giả thuyết vào workspace, tìm quy tắc đối soát, sửa `starter/Program.cs` rồi verify.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+; không cần DB, secrets hay cloud.

## Chạy nhanh
```powershell
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` thành công nếu starter phát hiện mismatch (exit 1 của app). `verify.ps1` kiểm tra mã starter đã sửa và chỉ thành công khi khớp đối tác.

## Những gì cần quan sát
Đối chiếu từng chuyến và tổng, phân biệt lỗi làm tròn từng dòng với lỗi cộng tổng. Thu thập stdout/stderr.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference solution](solution/README.md).

## Expected Results
Trước: `--verify` exit 1 và một số dòng lệch. Sau: bốn dòng và tổng 10.39 đều khớp, exit 0. Nếu không chạy được, kiểm tra `dotnet --info`.

## Estimated Time
40 phút.
