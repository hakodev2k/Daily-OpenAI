# UNIT-IO-001 — Smart Meter Frames: Intermittent Ingestion Failures

## Mục tiêu
Điều tra một sự cố binary framing trong .NET: cùng dữ liệu hợp lệ nhưng ingestion có lúc thành công, có lúc bị từ chối. Thu thập bằng chứng và sửa đường đọc frame mà không phá vỡ kiểm tra dữ liệu đầu vào.

## Bối cảnh thực tế
Dịch vụ nhận bản tin từ smart meter qua một luồng dữ liệu. Mỗi bản tin có header 4 byte biểu diễn độ dài payload (big-endian), sau đó là UTF-8 payload. Ở môi trường thử nghiệm, nhiều bản tin hợp lệ vẫn bị reject khi dữ liệu đến theo các đợt nhỏ; replay nguyên bản tin thường lại thành công.

## Bạn cần làm gì
1. Reproduce và ghi lại sự khác biệt giữa các kích thước chunk.
2. Đặt ít nhất hai hypothesis, chỉ ra evidence ủng hộ/bác bỏ.
3. Chỉnh sửa **starter/Program.cs**, không chỉnh simulator hay test oracle để che lỗi.
4. Chạy verify, bảo đảm frame hợp lệ, frame thiếu byte, length vượt giới hạn và cancellation đều được xử lý đúng.

## Yêu cầu môi trường
- .NET SDK 8.x (có `global.json`; không cần Docker, Azure hay NuGet ngoài SDK).
- PowerShell 7+ hoặc PowerShell trên Windows; có thể chạy lệnh `dotnet` trực tiếp ở macOS/Linux.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet --version
./scripts/run.ps1
./scripts/reproduce.ps1
```
Lệnh tương đương không cần PowerShell:
```bash
dotnet run --project starter/TelemetryIngest.csproj -- demo
dotnet run --project starter/TelemetryIngest.csproj -- reproduce
```

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Script chỉ **xác nhận lỗi gốc** (exit 0 khi tái hiện thành công); nó không phải kiểm thử bản sửa. So sánh kết quả `chunk=4096` và `chunk=2` từ `run.ps1`.

## Những gì cần quan sát
- Một frame có cùng nội dung nhưng kết quả phụ thuộc cách stream cung cấp dữ liệu.
- Dòng `REPRODUCED` có nghĩa là lỗi gốc đã xuất hiện.
- Trước khi sửa, `./scripts/verify.ps1` phải thất bại; sau khi sửa, tất cả ca kiểm thử phải PASS.
- Ghi lại exception type, số byte dự kiến và số byte thực nhận ở mỗi bước.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [Chỉ mở sau khi tự điều tra và thử sửa](solution/README.md).

## Expected Results
- [Before](expected-results/before.md)
- [After](expected-results/after.md)

## Estimated Time
35 phút. Ghi kết quả trong [workspace/my-investigation.md](workspace/my-investigation.md).
