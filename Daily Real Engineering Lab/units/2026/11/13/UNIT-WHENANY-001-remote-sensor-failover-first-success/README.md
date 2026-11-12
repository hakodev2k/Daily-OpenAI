# UNIT-WHENANY-001 — Điều tra failover gateway cảm biến

## Mục tiêu
Phân tích Task completion, lỗi từng gateway và hành vi lựa chọn nguồn dự phòng.

## Bối cảnh thực tế
Trạm đo chất lượng không khí có hai gateway; nguồn chính đôi khi phản hồi lỗi nhanh, nguồn phụ có thể cung cấp dữ liệu hợp lệ chậm hơn. Hệ thống lại báo outage.

## Bạn cần làm gì
Reproduce, thu thập exit code/timeline, ghi hai hypothesis, sửa starter và kiểm tra cả trường hợp tất cả gateway lỗi.

## Yêu cầu môi trường
.NET SDK 8, PowerShell 7+, không cần package ngoài.

## Chạy nhanh
`pwsh ./scripts/run.ps1`

## Cách reproduce vấn đề
`pwsh ./scripts/reproduce.ps1` trước khi sửa starter.

## Những gì cần quan sát
Nguồn dự phòng còn khả dụng nhưng kết quả chung thất bại. Ghi thời gian và trạng thái từng nguồn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong workspace.
3. Thử fix.
4. Verify.
5. Sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
[Cảnh báo spoiler](solution/README.md)

## Expected Results
Trường hợp dự phòng hoạt động: `DELIVERED:backup`; tất cả nguồn lỗi: `ALL_FAILED_HANDLED:2`.

## Estimated Time
40 phút.
