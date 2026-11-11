# UNIT-LINQSNAP-001 — Báo cáo tồn kho thay đổi giữa hai lần đọc

## Mục tiêu
Điều tra báo cáo có tổng chi tiết và footer không khớp; kiểm chứng giả thuyết bằng regression test.

## Bối cảnh thực tế
Kho thực phẩm cộng đồng xuất báo cáo tồn kho khi một phiếu điều chỉnh được ghi nhận. Bản in tự mâu thuẫn dù không có lỗi runtime.

## Bạn cần làm gì
Reproduce, thu thập evidence, nêu hai hypothesis, sửa `starter/InventoryReport.cs`, verify và đánh giá trade-off.

## Yêu cầu môi trường
.NET SDK 8, PowerShell.

## Chạy nhanh
`dotnet run --project starter/InventoryLab.csproj -- reproduce`

## Cách reproduce vấn đề
`./reproduce.ps1`

## Những gì cần quan sát
Các SKU, tổng chi tiết, tổng footer, thời điểm callback điều chỉnh kho. Chỉ ghi triệu chứng, chưa đoán root cause.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Solution](solution/README.md).

## Expected Results
Reproduce xác nhận mismatch; sau sửa `./verify.ps1` xác nhận cùng một bản báo cáo nhất quán.

## Estimated Time
40 phút.
