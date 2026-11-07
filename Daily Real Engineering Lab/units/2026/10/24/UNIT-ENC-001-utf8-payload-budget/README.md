# UNIT-ENC-001 — SMS gateway chấp nhận nội bộ nhưng bị đối tác từ chối

## Mục tiêu
Tự reproduce sai lệch giữa bước kiểm tra payload trong dịch vụ .NET và phản hồi từ SMS provider; thu thập evidence, xác định contract bị vi phạm, sửa policy mà không thay đổi gateway simulator.

## Bối cảnh thực tế
Hệ thống thông báo khách hàng dùng SMS provider với giới hạn 16 đơn vị kích thước payload (giới hạn nhỏ để dễ thử nghiệm). Thông báo tiếng Anh thường thành công, nhưng một số nội dung tiếng Việt và emoji được dịch vụ chấp nhận rồi bị provider trả HTTP 413. Việc retry không giúp ích. Dịch vụ phải từ chối payload không hợp lệ **trước** khi gọi provider, đồng thời vẫn chấp nhận payload hợp lệ đúng ngưỡng.

## Bạn cần làm gì
1. Chạy `./reproduce.ps1`; ghi lại ít nhất ba case, gồm ASCII, tiếng Việt và emoji.
2. Phân tích policy, cách simulator xác định giới hạn, và dữ liệu log. Viết ít nhất hai hypothesis vào workspace.
3. Chỉ chỉnh sửa `starter/PayloadPolicy.cs`. Không sửa `Program.cs`, runner hoặc expected results để làm test pass.
4. Chạy `./verify.ps1`, đảm bảo tất cả case thành công, bao gồm boundary và invalid arguments.
5. So sánh với reference solution sau khi hoàn thành.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 7+ hoặc Windows PowerShell 5.1. Không cần Docker, Azure, database hoặc package NuGet bên ngoài. `global.json` chấp nhận SDK 8.0 feature band mới hơn.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
```
Hoặc: `dotnet run --project starter/EncodingLab.Starter.csproj -- run`.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 có nghĩa là đã **chứng minh lỗi ở starter**, không có nghĩa lỗi đã được sửa. Sau khi chỉnh sửa policy:
```powershell
./verify.ps1
```
`verify.ps1` luôn chạy code trong **starter/** do bạn chỉnh sửa, không chạy reference solution.

## Những gì cần quan sát
- Local policy cho phép hay từ chối từng message?
- Provider simulator trả 202 hay 413, và case nào tạo ra sự khác biệt?
- Case nào nằm đúng ngưỡng nhưng vẫn hợp lệ?
- Thay đổi nào loại bỏ 413 mà không chặn nhầm các message hợp lệ?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: chỉ xem [Reference Solution](solution/README.md) sau khi đã tự điều tra và verify.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Ghi evidence tại [workspace](workspace/my-investigation.md).

## Estimated Time
40–50 phút.
