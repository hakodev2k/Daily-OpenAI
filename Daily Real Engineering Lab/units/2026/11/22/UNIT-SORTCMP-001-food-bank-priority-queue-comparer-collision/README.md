# UNIT-SORTCMP-001 — Hàng đợi cứu trợ làm mất yêu cầu cùng mức ưu tiên

## Mục tiêu
Điều tra tính toàn vẹn của hàng đợi sắp xếp khi các yêu cầu có cùng mức ưu tiên.

## Bối cảnh thực tế
Kho thực phẩm phân bổ các thùng hàng cứu trợ. Một số điểm nhận báo thiếu yêu cầu dù đầu vào đã được xác nhận. Mức ưu tiên của nhiều điểm có thể giống nhau.

## Bạn cần làm gì
Chạy ứng dụng, ghi số yêu cầu đầu vào và đầu ra, thu thập dấu vết enqueue, đưa ra ít nhất hai giả thuyết, sửa trong `starter/Program.cs` và chạy verification.

## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần database hoặc Azure.

## Chạy nhanh
Từ thư mục unit: `pwsh -File scripts/run.ps1`.

## Cách reproduce vấn đề
`pwsh -File scripts/reproduce.ps1` (mong đợi script xác nhận triệu chứng đã tái hiện).

## Những gì cần quan sát
Đối chiếu số yêu cầu đầu vào, số lần enqueue thành công, số yêu cầu được dispatch và danh sách RequestId.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Xem sau khi tự giải](solution/README.md).

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md)

## Estimated Time
50 phút.
