# UNIT-COOKIE-001 — Hồ sơ hội viên thủy cung bị lẫn giữa hai phiên partner

## Mục tiêu

Điều tra một lỗi cô lập phiên trong tích hợp HTTP, thu thập bằng chứng theo từng request và chứng minh dữ liệu của hai tenant không thể bị trả nhầm.

## Bối cảnh thực tế

Cổng gia hạn vé năm của một thủy cung dùng dịch vụ partner để lấy hồ sơ hội viên. Đội vận hành phát hiện sau khi xử lý hồ sơ của đơn vị Coral, một truy vấn từ đơn vị Kelp chưa đăng nhập lại nhận dữ liệu Coral. Khi Kelp đăng nhập, phiên Coral đôi lúc trả dữ liệu Kelp. Không có dịch vụ thật hoặc thông tin khách hàng thật trong lab này.

## Bạn cần làm gì

Chạy mô phỏng trên loopback, ghi lại thứ tự request/response, đề xuất ít nhất hai giả thuyết, xác định ranh giới sở hữu trạng thái phiên và sửa phần code tại `starter/PartnerGateway.cs`. Không thay đổi contract của harness.

## Yêu cầu môi trường

- Windows PowerShell 5.1+ hoặc PowerShell 7.
- .NET 8 SDK và runtime; không cần Docker, database, NuGet bên thứ ba hay tài khoản Azure.
- Kestrel chỉ lắng nghe `127.0.0.1` với cổng tạm.

## Chạy nhanh

Tại thư mục unit:

```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

Sau khi sửa `starter/PartnerGateway.cs`:

```powershell
./verify.ps1
```

Nếu chính sách PowerShell chặn script, có thể dùng `pwsh -File ./reproduce.ps1` hoặc chạy trực tiếp `dotnet run --project ./harness/PartnerCookieHarness.csproj -c Release -- reproduce`.

## Cách reproduce vấn đề

`reproduce.ps1` tạo một partner HTTP giả lập, thực hiện tuần tự: mở phiên Coral, đọc Coral, đọc Kelp trước khi mở phiên, mở phiên Kelp, đọc Kelp và đọc lại Coral. Script thành công khi phát hiện chính xác triệu chứng cần điều tra, **không** có nghĩa hệ thống đã được sửa.

## Những gì cần quan sát

- Mã HTTP và nội dung trả về của từng request, đặc biệt lần đọc Kelp chưa mở phiên.
- Sự thay đổi kết quả Coral sau khi Kelp bắt đầu phiên.
- Dữ liệu nào thuộc về request và dữ liệu nào có thể tồn tại lâu hơn request.
- `run.ps1` in toàn bộ bằng chứng, `reproduce.ps1` xác nhận lỗi, `verify.ps1` kiểm tra cả tính đúng và tính cô lập sau khi sửa.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix chỉ ở `starter/`.
4. Verify với `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** [Lời giải tham khảo](solution/README.md). Chỉ mở sau khi tự điều tra.

## Expected Results

[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md).

## Estimated Time

55 phút. Lab đã được static-review; mã nguồn chưa được chạy trong môi trường tạo bài.
