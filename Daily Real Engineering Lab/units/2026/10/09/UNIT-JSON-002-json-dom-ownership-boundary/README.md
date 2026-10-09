# UNIT-JSON-002 — JSON DOM ownership across a delayed dispatch boundary

## Mục tiêu
Refactor một luồng nhận webhook sang xử lý bất đồng bộ mà không thay đổi wire contract; phân tích object lifetime, bằng chứng lỗi và regression.

## Bối cảnh thực tế
Dịch vụ nhận webhook từ đối tác, tạo work item rồi chuyển sang dispatcher. Log capture báo thành công nhưng dispatcher lỗi với payload hợp lệ. Team muốn giữ nguyên output contract `partner|eventId|amount` và không thêm service trả phí.

## Bạn cần làm gì
1. Chạy baseline và ghi nhận failure.
2. Nêu ít nhất 2 hypothesis; xác định điểm chuyển ownership giữa hai stage.
3. Sửa duy nhất `starter/` để cả ba fixtures đi qua dispatcher.
4. Chạy verify và so sánh kết quả; giải thích trade-off về allocation và schema validation.

## Yêu cầu môi trường
.NET SDK 8.x hoặc SDK mới hơn có thể build `net8.0`; PowerShell 5.1+ hoặc PowerShell 7+. Không cần NuGet package ngoài hay Docker.

## Chạy nhanh
Từ thư mục unit:
```powershell
.\scripts\setup.ps1
.\scripts\reproduce.ps1
.\scripts\run.ps1
.\scripts\verify.ps1
```
`run.ps1` và `verify.ps1` được kỳ vọng thất bại trước khi bạn sửa `starter/`. Chạy `dotnet run --project starter/JsonLab.csproj` nếu không có PowerShell.

## Cách reproduce vấn đề
`reproduce.ps1` chạy bản baseline độc lập, không bị ảnh hưởng bởi chỉnh sửa trong `starter/`. Ghi lại exit code, thứ tự các marker CAPTURE/DISPATCH và loại exception. Đọc thêm log mô phỏng tại `evidence/incident.log`.

## Những gì cần quan sát
- Capture hoàn tất với dữ liệu đầu vào hợp lệ.
- Dispatcher thất bại ở bước tiếp theo, trước khi trả kết quả.
- Endpoint nhận webhook trong log vận hành vẫn có thể phản hồi nhanh; thời gian nhận request không đủ chứng minh xử lý downstream thành công.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/Program.cs`.
4. Verify qua `scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ mở [solution/README.md](solution/README.md) sau khi đã tự điều tra và thử sửa.

## Expected Results
Baseline: `CHECK_FAILED ... ObjectDisposedException`; `reproduce.ps1` báo `REPRODUCTION_CONFIRMED`.
Starter chưa sửa: `verify.ps1` thất bại.
Starter sau fix hợp lệ: cả ba cases thành công và in `VERIFICATION_PASSED`. Không cần so sánh latency.

## Estimated Time
45–60 phút · L4 Refactoring / Modernization · D3 Investigation.
