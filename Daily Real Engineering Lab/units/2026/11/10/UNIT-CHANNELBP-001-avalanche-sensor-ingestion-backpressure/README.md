# UNIT-CHANNELBP-001 — Avalanche sensor: cảnh báo bị mất khi burst

## Mục tiêu
Điều tra mất dữ liệu trong hàng đợi bất đồng bộ và chứng minh bất biến accepted/delivered bằng replay có kiểm soát.

## Bối cảnh thực tế
Gateway nhận bốn cảnh báo tuyết lở liên tiếp, trong khi telemetry sink tạm dừng. Log ghi nhận bốn lần nhận thành công nhưng downstream chỉ có một phần cảnh báo. Một số giả thuyết: consumer xử lý sai, queue capacity, thứ tự scheduling, lỗi ghi.

## Bạn cần làm gì
Reproduce, lưu số lượng và ID, lập ít nhất hai hypothesis, điều tra semantics của hàng đợi, sửa đường đi dữ liệu mà không mở rộng queue vô hạn, rồi verify.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell.

## Chạy nhanh
```powershell
./reproduce.ps1
```
Script reproduce mong đợi chương trình trả exit code khác 0, và script tự xác nhận triệu chứng.

## Cách reproduce vấn đề
Chạy `./run.ps1` và ghi `accepted`, `delivered`, `ids`; chạy lại để so sánh.

## Những gì cần quan sát
Số cảnh báo đã được chấp nhận có bằng số đã giao? Cảnh báo nào thiếu? Có exception từ producer không? Thử đọc code `starter/Program.cs` và ghi hypothesis trước khi thay đổi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/README.md)

## Expected Results
Trước: replay chứng minh có ít nhất một cảnh báo thiếu. Sau: tất cả bốn ID được giao đúng thứ tự; producer không hoàn tất khi downstream vẫn giữ capacity.

## Estimated Time
50 phút.
