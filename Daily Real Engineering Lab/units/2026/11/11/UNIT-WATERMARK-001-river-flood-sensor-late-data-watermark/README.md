# UNIT-WATERMARK-001 — River Flood Forecast: Late Data & Alert Corrections

## Mục tiêu
Thiết kế pipeline cảnh báo mực nước có thể kiểm chứng khi dữ liệu đến muộn, đảo thứ tự và được hiệu chỉnh.

## Bối cảnh thực tế
12 trạm đo ven sông gửi telemetry qua mạng di động không ổn định. Bản tin hạ cấp cảnh báo đã phát lúc 09:10 nhưng dữ liệu bổ sung sau đó cho thấy mực nước vượt ngưỡng trong khoảng đánh giá.

## Bạn cần làm gì
Đọc [bối cảnh](docs/scenario.md), phát lại [dữ liệu](evidence/arrivals.csv), ghi quyết định trong [workspace](workspace/my-decision.md), đối chiếu [acceptance](docs/acceptance.md).

## Yêu cầu môi trường
PowerShell 7+ hoặc trình đọc CSV/Markdown; không cần Azure, Docker hay kết nối mạng.

## Chạy nhanh
```powershell
Import-Csv .\evidence\arrivals.csv | Sort-Object {[datetimeoffset]$_.receivedAt} | Format-Table
Get-Content .\docs\acceptance.md
```

## Cách reproduce vấn đề
Duyệt từng record theo `receivedAt`, áp dụng policy hiện tại ở [scenario](docs/scenario.md), so sánh quyết định lúc 09:10 với dữ liệu đầy đủ sau 09:20.

## Những gì cần quan sát
Thời gian đo và thời gian nhận; thứ tự nhận; bản ghi hiệu chỉnh; độ trễ giữa cảnh báo ban đầu và thông tin hoàn chỉnh.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix bằng thiết kế và policy.
4. Verify với acceptance matrix.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md), [Hint 2](hints/hint-02.md), [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md).

## Expected Results
Một thiết kế có invariant, watermark, xử lý correction, late event, replay, SLO, rollback và trade-off định lượng.

## Estimated Time
80 phút (L6 / D4).