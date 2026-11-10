# UNIT-RECALL-001 — School Meal Allergen Recall

## Mục tiêu
Điều tra rủi ro xuất kho nguyên liệu bị thu hồi trong hệ thống event-driven và thiết kế ranh giới an toàn.

## Bối cảnh thực tế
18 bếp học đường phục vụ 32.000 suất/ngày. Nhà cung cấp thu hồi một lô nguyên liệu nhưng bếp vẫn hiển thị lô đó hợp lệ trong vài phút. Một chuyến xe đã rời kho.

## Bạn cần làm gì
Đọc [scenario](docs/scenario.md), đối chiếu [timeline](evidence/timeline.csv), lập giả thuyết và đề xuất ít nhất hai phương án. Hoàn thành [worksheet](workspace/my-decision.md) và [acceptance matrix](docs/acceptance.md).

## Yêu cầu môi trường
Không cần cloud, Docker hay database. PowerShell hoặc trình xem CSV/Markdown là đủ.

## Chạy nhanh
Từ thư mục unit chạy:
```powershell
Get-Content .\docs\scenario.md
Import-Csv .\evidence\timeline.csv | Format-Table
Get-Content .\docs\acceptance.md
```

## Cách reproduce vấn đề
Đọc timeline theo thứ tự thời gian; đối chiếu quyết định xuất kho với trạng thái ở nguồn có thẩm quyền tại cùng thời điểm.

## Những gì cần quan sát
Khoảng cách thời gian giữa thay đổi trạng thái, event delivery, read model và hành động vật lý; ghi lại các bằng chứng không đủ tin cậy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix bằng kiến trúc và policy.
4. Verify bằng acceptance matrix.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md).

## Expected Results
Thiết kế có safety invariant, xử lý delayed/duplicate/out-of-order events, timeout, retry, và kiểm soát race tại thời điểm dispatch.

## Estimated Time
75 phút (L6/D4).
