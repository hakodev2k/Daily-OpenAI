# UNIT-CSVFRAME-001 — Dòng CSV nhiều dòng làm lệch dữ liệu nhập vật tư y tế

## Mục tiêu
Điều tra lỗi toàn vẹn dữ liệu khi nhập CSV từ đối tác, xác định bằng chứng và xây dựng kiểm thử hợp đồng dữ liệu.

## Bối cảnh thực tế
Bệnh viện huyện nhận bảng kê vật tư gồm SKU, số lượng và ghi chú. Một số phiếu từ nhà cung cấp có ghi chú nhiều dòng hoặc dấu câu đặc biệt. Báo cáo nhập kho cho thấy số dòng thấp hơn bảng kê gốc.

## Bạn cần làm gì
Chạy hệ thống, ghi lại số bản ghi, so sánh với nội dung CSV, đề xuất giả thuyết rồi sửa trong `starter/Program.cs`.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7 hoặc Windows PowerShell 5.1. Không cần database, mạng hay package ngoài.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit, chạy `./reproduce.ps1`; script yêu cầu starter hiện tại không đạt contract. Sau khi sửa, chạy `./verify.ps1`.

## Những gì cần quan sát
Số dòng, SKU, quantity, note; so sánh byte CRLF và dấu ngoặc kép trong fixture. Đừng chỉ nhìn console vì một bản ghi bị bỏ qua có thể không gây exception.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trên starter.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) và [reference source](solution/Program.cs).

## Expected Results
Trước sửa: `--check` FAIL. Sau sửa: 2 dòng, đúng cả ghi chú nhiều dòng, escaped quote, dấu phẩy trong field; malformed input bị từ chối.

## Estimated Time
45 phút.