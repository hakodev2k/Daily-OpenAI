# UNIT-LINQ-002 — Báo cáo cân nhập kho trộn hai snapshot của live feed

## Mục tiêu
Điều tra một báo cáo có các chỉ số tự mâu thuẫn trong lúc dữ liệu nhập kho tiếp tục đến; thu thập bằng chứng, sửa implementation và chứng minh tính nhất quán.

## Bối cảnh thực tế
Hợp tác xã thu mua ngũ cốc phát hành báo cáo theo đợt cân. Dashboard hiển thị **2 phiếu cân** nhưng **700 kg**, trong khi danh sách hai phiếu đã xác nhận ở đầu đợt chỉ có **300 kg**. Một phiếu mới đến trong lúc báo cáo được tổng hợp. Bộ phận thanh toán không thể đối soát số liệu.

## Bạn cần làm gì
1. Chạy fixture nguyên bản, ghi lại số phiếu, tổng kg, số lần mở nguồn và các giả thuyết.
2. Đọc starter, xác định điều kiện khiến báo cáo không phản ánh cùng một thời điểm quan sát.
3. Chỉnh sửa **chỉ** `starter/ReportBuilder.cs` để các chỉ số nhất quán và vẫn xử lý được nguồn rỗng.
4. Chạy verification trên starter đã sửa, sau đó mới xem reference solution.
5. Đánh giá trade-off giữa một lượt duyệt, materialization và transaction snapshot khi dùng nguồn dữ liệu thật.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell. Không cần database, Docker, Azure hay NuGet bên ngoài.

## Chạy nhanh
```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```
Hoặc `dotnet run --project starter/WeighbridgeLab.csproj -- reproduce`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` **trước khi sửa starter**. Script trả exit code 0 nếu nhận diện đúng triệu chứng. Chạy `./verify.ps1` để thấy acceptance checks thất bại trên starter ban đầu.

## Những gì cần quan sát
Đối chiếu `ReceiptCount`, `TotalWeightKg`, `AverageWeightKg` và `SnapshotOpens`. Phân biệt lỗi công thức, làm tròn số, phiếu trùng và ranh giới snapshot. Kiểm tra bằng chứng ở `evidence/incident.md`.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ đọc sau khi tự sửa:** [Giải thích](solution/README.md) · [Code](solution/ReportBuilder.cs)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút, L2 / D4. [Ghi chép điều tra](workspace/my-investigation.md).
