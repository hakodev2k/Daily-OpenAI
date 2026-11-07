# UNIT-TIME-001 — Watchdog kết thúc batch sai thời điểm

## Mục tiêu
Điều tra một incident về giới hạn thời gian xử lý batch, thu thập bằng chứng, sửa code và kiểm tra regression.

## Bối cảnh thực tế
Worker fulfillment giới hạn mỗi batch trong 10 giây. Sau thay đổi môi trường host, một số batch bị dừng sớm, trong khi batch khác chạy quá ngân sách. Downstream vẫn khỏe, CPU bình thường. Đội vận hành cần một regression test offline, không chỉnh system clock thật.

## Bạn cần làm gì
1. Reproduce triệu chứng; ghi nhận expected/actual và exit code.
2. Đề xuất ít nhất ba giả thuyết, cách loại trừ bằng evidence.
3. Chỉ chỉnh `starter/LeaseWatchdog.cs`; giữ nguyên harness và simulator.
4. Verify, sau đó đánh giá process restart và multi-node lease.
5. Chỉ cuối cùng mới xem reference solution.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 5.1+ hoặc 7+. Không cần NuGet bên ngoài, Docker, Azure hay database.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```
Nếu PowerShell không chạy được, dùng `dotnet run --project starter/ClockLab.csproj -- reproduce`.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit **0** nghĩa là triệu chứng starter được tái hiện, **không** phải fix đã đúng. Exit **1** nghĩa là chưa tái hiện đủ triệu chứng hoặc control case thất bại.

## Những gì cần quan sát
Đối chiếu case bình thường, điều chỉnh tiến/lùi và rearm; ghi lại thời lượng logic đã trôi qua. Đọc [incident evidence](evidence/incident-timeline.md) và ghi giả thuyết trước khi mở hint.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Sửa code learner-editable.
4. Verify bằng `./verify.ps1` (exit 0 = tất cả behavior assertions PASS).
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ mở sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
60–75 phút (L3 / D4). Static-reviewed; mã chưa được thực thi trong môi trường tạo lab.
