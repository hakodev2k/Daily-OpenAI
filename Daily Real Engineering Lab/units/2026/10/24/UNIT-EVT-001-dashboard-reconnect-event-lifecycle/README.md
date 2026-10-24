# UNIT-EVT-001 — Dashboard nhận notification trùng sau reconnect

## Mục tiêu
Tái hiện và refactor lỗi vòng đời session C# qua các trạng thái kết nối, reconnect, offline và đóng dashboard; xác minh bằng kiểm thử hành vi.

## Bối cảnh thực tế
Dashboard vận hành theo dõi công việc bằng notification nội bộ. Sau khi mất kết nối rồi kết nối lại, một số notification xuất hiện nhiều lần. Đôi khi dashboard đã đóng vẫn ghi nhận notification. Smoke test trong trạng thái kết nối bình thường vẫn xanh.

## Bạn cần làm gì
1. Chạy starter, thu thập expected/actual của năm tình huống.
2. Ghi ít nhất hai hypothesis, bằng chứng ủng hộ và bác bỏ.
3. Chỉ chỉnh class `DashboardSession` trong `starter/Program.cs`; không sửa expected counts hoặc test harness.
4. Chạy verify và chứng minh invariant của vòng đời session.
5. So sánh reference solution sau khi đã tự thử.

## Yêu cầu môi trường
.NET SDK 10.x, PowerShell 5.1+; không cần Docker, database, Azure, NuGet ngoài hay mạng.

## Chạy nhanh
~~~powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
~~~

## Cách reproduce vấn đề
`./scripts/reproduce.ps1` chạy starter và exit 0 khi quan sát đúng triệu chứng ban đầu. Lệnh này **không** kiểm tra fix. Có thể chạy trực tiếp `dotnet run --project ./starter/EventReconnectLab.csproj -- reproduce` nếu PowerShell bị chặn.

## Những gì cần quan sát
So sánh `steady-session`, `repeat-connect`, `reconnect`, `offline-session`, `disposed-session`. Những chuỗi trạng thái nào làm số notification khác kỳ vọng? Tại sao smoke test không phát hiện?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify bằng `./scripts/verify.ps1` (starter chưa sửa phải fail).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
⚠️ Spoiler: chỉ mở [solution/README.md](solution/README.md) sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45–70 phút; L4 Refactoring Workshop, D4 Production Troubleshooting.
