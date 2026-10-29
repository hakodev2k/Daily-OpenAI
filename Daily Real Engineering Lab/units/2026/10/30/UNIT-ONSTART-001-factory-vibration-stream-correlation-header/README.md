# UNIT-ONSTART-001 — Mất correlation header trên luồng dữ liệu realtime

## Mục tiêu
Điều tra sự khác biệt giữa response thông thường và response streaming trong ASP.NET Core; thu thập bằng chứng HTTP và sửa lỗi mà không thay đổi payload hay status.

## Bối cảnh thực tế
Dashboard giám sát rung động máy móc có một endpoint health và một luồng Server-Sent Events (SSE). Sau khi triển khai middleware hỗ trợ tracing, đội vận hành thấy request đến luồng realtime thiếu `X-Trace-Id`, khiến việc ghép log theo request khó khăn. Health check vẫn trả header bình thường. Cả hai route đều trả status thành công.

## Bạn cần làm gì
1. Chạy starter và tái hiện hai response khác nhau.
2. Ghi ít nhất hai giả thuyết, phân biệt vấn đề ở proxy, endpoint và response pipeline.
3. Kiểm tra code và bằng chứng, đề xuất thay đổi tối thiểu.
4. Sửa **`starter/FactoryDashboard.cs`**, giữ nguyên API contract.
5. Chạy `verify.ps1` và so sánh với reference sau khi tự xử lý.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7+.
- .NET 8 SDK (theo `global.json`), không cần database, Docker, Azure hay NuGet package ngoài framework.

## Chạy nhanh
Từ thư mục unit:
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 nghĩa là **đã quan sát đúng lỗi gốc**, không phải đã sửa. `verify.ps1` sẽ thất bại trước khi bạn sửa starter.

## Những gì cần quan sát
- HTTP status, `Content-Type`, `X-Trace-Id` của cả hai route.
- Payload SSE có giữ đúng định dạng không.
- Khác biệt giữa request hoàn tất mà không ghi body và request ghi/flush body.
- Phân biệt header thiếu với status lỗi hoặc payload lỗi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [solution/README.md](solution/README.md)

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
60–75 phút (L3 / D4).
