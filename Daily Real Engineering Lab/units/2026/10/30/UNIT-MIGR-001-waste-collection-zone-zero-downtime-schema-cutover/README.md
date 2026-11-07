# UNIT-MIGR-001 — Chuyển đổi vùng thu gom rác không gián đoạn

## Mục tiêu
Thiết kế migration PostgreSQL an toàn khi ASP.NET Core API triển khai rolling, dữ liệu cũ chưa chuẩn hóa và rollback phải khả thi.

## Bối cảnh thực tế
Hệ thống điều phối thu gom rác thay tên vùng text bằng định danh chuẩn. Trong diễn tập, một số tuyến bị mất hoặc sai vùng khi API cũ và mới cùng hoạt động. Dừng tiếp nhận yêu cầu không được phép.

## Bạn cần làm gì
1. Đọc [yêu cầu](scenario/requirements.md), [timeline](evidence/timeline.md) và hai CSV.
2. Phân loại bản ghi và nêu ba giả thuyết có thể giải thích sai lệch.
3. Điền [Decision Record](workspace/my-investigation.md) với ít nhất hai phương án, deployment phases, rollback và monitoring.
4. Thực hiện sáu tình huống [acceptance replay](verification/acceptance-cases.md) trên giấy, ghi evidence cho từng case.
5. Đọc [requirement change](extensions/change-request.md), điều chỉnh thiết kế trước khi mở reference.

## Yêu cầu môi trường
Chỉ cần PowerShell hoặc công cụ đọc Markdown/CSV; **đây là architecture tabletop**, không chạy PostgreSQL thật.

## Chạy nhanh
```powershell
Get-Content .\scenario\requirements.md
Import-Csv .\evidence\legacy-sample.csv | Format-Table
Import-Csv .\evidence\zone-map.csv | Format-Table
Get-Content .\verification\acceptance-cases.md
```

## Cách reproduce vấn đề
Lần lượt áp dụng T0–T4 trong evidence lên các writer v1/v2. Đánh dấu thời điểm mỗi contract có thể từ chối, làm mất hoặc hiểu sai dữ liệu. Không giả định deploy diễn ra đồng thời.

## Những gì cần quan sát
Theo dõi sự khác biệt giữa khả năng ghi, dữ liệu đã chuyển đổi, dữ liệu mơ hồ và đường rollback. Nêu bằng chứng cụ thể, không đoán root cause chỉ từ log deploy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix hoặc quyết định.
4. Verify bằng acceptance cases.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution — chỉ đọc sau khi tự ra quyết định](solution/README.md).

## Expected Results
Decision Record có phương án, phase gates, rollback, 5 metrics, phân loại CSV, kết quả A–F và phản ứng trước requirement change. Không có một đáp án kiến trúc duy nhất.

## Estimated Time
85 phút: phân tích 25, thiết kế 35, replay 15, đối chiếu 10.
