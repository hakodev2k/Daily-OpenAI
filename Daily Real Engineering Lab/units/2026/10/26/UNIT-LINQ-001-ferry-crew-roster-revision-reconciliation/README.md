# UNIT-LINQ-001 — Bảng phân ca phà hiển thị bản phân công cũ sau đồng bộ

## Mục tiêu
Điều tra một pipeline đồng bộ và chứng minh output không phụ thuộc thứ tự bản tin.

## Bối cảnh thực tế
Hệ thống điều hành phà nhận các bản tin phân công thuyền viên từ nhiều bến. Mỗi bản tin có `CrewId`, `Revision`, `VesselId`, `EventId`. Sau khi kết nối mạng chập chờn, dashboard đôi khi hiển thị phân công cũ dù dữ liệu đầu vào không đổi. Upstream có thể gửi replay và thay đổi thứ tự batch.

Contract nghiệp vụ: mỗi crew có một phân công ở revision cao nhất; replay cùng revision và vessel hợp lệ; cùng revision nhưng khác vessel phải báo conflict.

## Bạn cần làm gì
1. Reproduce, ghi lại hai kết quả.
2. Đưa ít nhất hai hypothesis và evidence.
3. Chỉ sửa `starter/ShiftReconciler.cs`.
4. Verify thứ tự, revision, replay và conflict.
5. Sau khi tự sửa mới xem reference.

## Yêu cầu môi trường
.NET 8 SDK và runtime, PowerShell (Windows PowerShell hoặc PowerShell 7). Không cần Docker, database, Azure hay NuGet ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
```
Hoặc `dotnet run --project starter/ShiftSyncLab.csproj -- reproduce`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` khi starter chưa sửa. Exit code 0 nghĩa là đã tái hiện triệu chứng, không phải fix thành công.

## Những gì cần quan sát
So sánh revision và vessel của `F-101` trong hai thứ tự feed; so sánh số lượng crew và kiểm tra exception. Ghi lại evidence trước khi sửa.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md) — chỉ mở sau khi đã tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút. [Workspace ghi evidence](workspace/my-investigation.md).
