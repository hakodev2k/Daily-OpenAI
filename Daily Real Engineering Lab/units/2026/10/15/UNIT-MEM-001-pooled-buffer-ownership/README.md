# UNIT-MEM-001 — Telemetry bị thay đổi sau khi được nhận

## Mục tiêu
Điều tra lỗi dữ liệu biến đổi giữa lúc ingest và lúc xử lý batch; xây dựng bằng chứng, sửa tại boundary phù hợp và chứng minh không regression.

## Bối cảnh thực tế
Gateway nhận sự kiện cảm biến theo burst và đưa vào hàng đợi in-memory để xử lý sau. Dashboard đôi khi ghi nhận hai thiết bị khác nhau có payload giống nhau dù log ingress cho thấy dữ liệu ban đầu khác biệt. Không có database, broker hay mạng bên ngoài trong bản local.

## Bạn cần làm gì
1. Chạy starter, tái hiện payload của một sự kiện bị đổi sau khi sự kiện khác đến.
2. Ghi ít nhất hai giả thuyết, nêu bằng chứng loại trừ.
3. Sửa **starter/**, không chỉnh bộ test hoặc **solution/**.
4. Chạy verification; giải thích ownership/lifetime và chi phí của phương án.
5. Sau đó mới mở reference solution.

## Yêu cầu môi trường
.NET SDK 8.0.x, PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần NuGet package bên ngoài, Docker hay Azure.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```
Nếu PowerShell không cho chạy script, dùng `dotnet run --project starter/TelemetryLab.csproj -- reproduce` tại thư mục unit.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code **0** nghĩa là triệu chứng của starter đã được chứng minh; **không** có nghĩa code đã đúng. Ghi lại giá trị hai event và thứ tự nhận.

## Những gì cần quan sát
So sánh event khi nhận với event lúc drain; lưu ý thứ tự, số lượng và dữ liệu UTF-8. Xác định lúc nào payload của sự kiện cũ thay đổi. `run.ps1` chỉ quan sát, không kiểm tra đúng/sai.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trên `starter/Program.cs`.
4. Verify bằng `./verify.ps1` (exit 0 mới là PASS).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) — chỉ đọc sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45–60 phút (L2 / D3). Đây là static-reviewed lab; mã chưa được thực thi trong môi trường tạo lab.
