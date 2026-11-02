# UNIT-HOSTEX-001 — Một cảnh báo lỗi khiến worker quan trắc không xử lý các cảnh báo tiếp theo

## Mục tiêu

Tái hiện sự cố trong một .NET hosted worker, phân biệt lỗi ở một lần gửi với trạng thái sống của toàn bộ host, thu thập bằng chứng từ log và số liệu, sửa đường xử lý và kiểm chứng không mất các cảnh báo độc lập.

## Bối cảnh thực tế

Một dịch vụ nội bộ chuyển cảnh báo chất lượng không khí từ ba trạm đo đến cổng tiếp nhận của đối tác. Trạm đầu tiên được tiếp nhận; trạm thứ hai gặp lỗi từ cổng đối tác; sau đó không thấy kết quả của trạm thứ ba dù dữ liệu đầu vào hợp lệ. Bộ giám sát ghi nhận tiến trình dừng sớm hơn thời gian quan sát dự kiến. Nhóm vận hành cần xác định đây là lỗi dữ liệu, lỗi gateway, hay sự cố vòng đời dịch vụ.

## Bạn cần làm gì

1. Chạy starter và ghi lại `LAB_RESULT`, `hostStoppedEarly`, `attempts`, `published`, `failed`.
2. Đề xuất ít nhất ba hypothesis và tìm bằng chứng phân biệt.
3. Đọc `starter/NoticeWorker.cs`, `FakeGateway.cs` và log host; giữ nguyên `Program.cs`, `FakeGateway.cs`, `DeliveryJournal.cs` và scripts.
4. Chỉ sửa `starter/NoticeWorker.cs` để một lần gửi thất bại có thể được ghi nhận và xử lý theo hợp đồng, đồng thời các cảnh báo độc lập vẫn được tiếp tục.
5. Chạy `verify.ps1`, so sánh với reference solution sau khi đã tự thử.

## Yêu cầu môi trường

.NET SDK 8 trở lên và PowerShell 5.1/7. Project target `net8.0`, chỉ dùng shared framework có sẵn từ ASP.NET Core Web SDK; không cần NuGet bên thứ ba, Azure, Docker hay mạng ngoài.

## Chạy nhanh

Từ thư mục unit:

```powershell
./reproduce.ps1
# Sửa starter/NoticeWorker.cs
./verify.ps1
```

Chạy trực tiếp (Windows/Linux/macOS): `dotnet run --project starter/AirAlertLab.csproj`. Khi chưa sửa, chương trình trả mã 3 cho triệu chứng được dự kiến; sau khi sửa đúng trả mã 0.

## Cách reproduce vấn đề

`reproduce.ps1` chạy chính starter, yêu cầu ứng dụng xuất `LAB_RESULT=EXPECTED_FAILURE` và exit code 3. Mỗi lần chạy sử dụng ba thông báo cố định, gateway giả lập có hành vi cố định và thời gian quan sát 700 ms. Script trả mã 0 khi **đã chứng minh được lỗi gốc**; mã 0 của script không có nghĩa là starter đã được sửa.

## Những gì cần quan sát

- Số lần thử gửi và thứ tự ba mã `AQ-101`, `AQ-102`, `AQ-103`.
- Log ứng dụng so với log vòng đời của generic host.
- Trạng thái `hostStoppedEarly` trong khoảng quan sát.
- Phân biệt cảnh báo gửi thành công, cảnh báo lỗi cần xử lý tiếp và cảnh báo chưa được thử.
- Sau khi sửa: `AQ-101` và `AQ-103` được gửi, `AQ-102` được ghi nhận thất bại, host không dừng sớm.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ đọc sau khi tự điều tra và sửa:** [Reference Solution](solution/README.md).

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

40–60 phút. Ghi điều tra tại [workspace/my-investigation.md](workspace/my-investigation.md).
