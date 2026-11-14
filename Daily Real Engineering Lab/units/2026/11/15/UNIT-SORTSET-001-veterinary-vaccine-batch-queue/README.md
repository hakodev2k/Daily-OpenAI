# UNIT-SORTSET-001 — Lô vaccine biến mất khỏi kế hoạch phân phối

## Mục tiêu
Điều tra sai lệch số lượng giữa dữ liệu nhập và danh sách xuất, bảo toàn định danh lô, và thiết kế regression test cho chính sách sắp xếp.

## Bối cảnh thực tế
Trung tâm thú y phân phối vaccine theo hạn sử dụng. Báo cáo nhập có 4 lô, 280 liều, nhưng danh sách cấp phát và tổng liều đôi khi thấp hơn. Không có exception hay cảnh báo trùng lô.

## Bạn cần làm gì
Chạy chương trình, thu thập danh sách lô và tổng liều; viết ít nhất hai hypothesis; sửa code trong `starter/Program.cs` để không mất lô hợp lệ và giữ thứ tự ưu tiên; bổ sung test cho lô cùng ngày và trùng định danh.

## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7+ hoặc chạy trực tiếp `dotnet run`. Không cần database/network.

## Chạy nhanh
`pwsh ./scripts/setup.ps1` rồi `pwsh ./scripts/run.ps1`.

## Cách reproduce vấn đề
`pwsh ./scripts/reproduce.ps1` phải in `REPRODUCED` trên starter chưa sửa.

## Những gì cần quan sát
Ghi lại INPUT, OUTPUT, COUNT, DOSES; so sánh từng BatchId và thứ tự ExpiryDays. Đối chiếu tổng liều với bản ghi nguồn.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Xem gợi ý tăng dần](hints.md).

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md).

## Expected Results
Sau khi sửa, `pwsh ./scripts/verify.ps1` in `VERIFY_PASS`, exit 0; tất cả lô hợp lệ xuất hiện, thứ tự ngày không giảm, tổng liều khớp nguồn.

## Estimated Time
40–50 phút.
