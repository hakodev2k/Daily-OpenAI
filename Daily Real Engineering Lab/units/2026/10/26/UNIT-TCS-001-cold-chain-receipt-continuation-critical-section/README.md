# UNIT-TCS-001 — Tín hiệu xác nhận pallet làm chậm vòng tiếp nhận dù không có lỗi

## Mục tiêu
Điều tra sự chênh lệch giữa thời gian xử lý một acknowledgement và thời gian giữ vùng đồng bộ của một service .NET; cải thiện contract điều phối mà không thay đổi kết quả nghiệp vụ.

## Bối cảnh thực tế
Một kho lạnh nhận acknowledgement từ thiết bị quét pallet. Service ghép từng acknowledgement với một yêu cầu đang chờ và ghi audit vào adapter cũ. Dưới tải cao, luồng nhận dữ liệu có lúc bị giữ lại lâu bất thường mặc dù acknowledgement hợp lệ và không có lỗi mạng. Đội vận hành muốn giảm thời gian xử lý tại ingestion boundary mà vẫn đảm bảo audit chỉ chạy một lần.

## Bạn cần làm gì
1. Chạy baseline, ghi lại `producerElapsedMs`, `auditInsideGate`, `auditCount`.
2. Viết tối thiểu hai hypothesis (ví dụ: nghẽn I/O, contention, scheduling), rồi chọn evidence phân biệt.
3. Chỉ chỉnh sửa `starter/ReceiptCoordinator.cs`; giữ nguyên public API.
4. Chạy verification nhiều lần và chứng minh audit không chạy trong critical section của producer, acknowledgement vẫn đúng, audit vẫn chạy đúng một lần.
5. So sánh reference solution và đánh giá trade-off.

## Yêu cầu môi trường
.NET 8 SDK, PowerShell 5.1+ hoặc PowerShell 7; không cần NuGet ngoài, Docker, Azure, database hay credential.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
```
Để chạy demo: `./run.ps1`. Chỉ sau khi tự sửa: `./verify.ps1`.

## Cách reproduce vấn đề
Trên starter **chưa sửa**, chạy `./reproduce.ps1`. Exit code **0** nghĩa là triệu chứng được tái hiện thành công, **không phải** đã sửa xong. Có thể dùng `dotnet run --project starter/ColdChainReceiptLab.csproj -- reproduce`.

## Những gì cần quan sát
Quan sát cờ `auditInsideGate`, thời gian xử lý acknowledgement, số audit và mã trạng thái. Không dựa vào một ngưỡng milliseconds cố định vì tốc độ máy khác nhau. Xem [before](expected-results/before.md) để biết contract quan sát.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [solution/README.md](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). [Investigation guide](docs/investigation-guide.md).

## Estimated Time
60–80 phút (L4 / D4). [Ghi evidence tại đây](workspace/my-investigation.md).
