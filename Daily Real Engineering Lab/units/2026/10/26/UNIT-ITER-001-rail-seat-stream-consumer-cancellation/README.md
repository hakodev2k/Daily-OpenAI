# UNIT-ITER-001 — Subscription hủy nhưng luồng cập nhật ghế vẫn tiếp tục

## Mục tiêu
Điều tra lifecycle của asynchronous stream trong .NET, thu thập bằng chứng, sửa hành vi khi subscriber hủy và chứng minh không regression.

## Bối cảnh thực tế
Dashboard đặt vé tàu liên tỉnh theo dõi cập nhật ghế. Khi người dùng đóng màn hình, client báo đã hủy subscription nhưng backend vẫn phát thêm dữ liệu. Luồng chạy bình thường nếu không có cancellation.

## Bạn cần làm gì
1. Chạy starter và ghi bốn chỉ số từ harness.
2. Viết ít nhất hai giả thuyết, dùng evidence để loại trừ.
3. Sửa mã tại `starter/SeatAvailabilityFeed.cs`, không sửa harness.
4. Chạy verify cho cả trường hợp hủy trước và sau lần đọc đầu tiên.

## Yêu cầu môi trường
.NET SDK 8+ và PowerShell 5.1+/7+; không cần Docker, Azure hay NuGet ngoài SDK.

## Chạy nhanh
Tại thư mục unit:

    ./scripts/setup.ps1
    ./scripts/run.ps1
    ./scripts/reproduce.ps1

Sau khi tự sửa starter:

    ./scripts/verify.ps1

Không có PowerShell:

    dotnet restore scripts/LabChecks/LabChecks.csproj
    dotnet run --project scripts/LabChecks/LabChecks.csproj -c Release -- --mode reproduce
    dotnet run --project scripts/LabChecks/LabChecks.csproj -c Release -- --mode verify

## Cách reproduce vấn đề
`reproduce.ps1` trả exit code 0 nếu starter thể hiện lỗi dự kiến; `verify.ps1` trả exit code 1 khi chưa sửa. Reproduce xác nhận triệu chứng, không xác nhận correctness.

## Những gì cần quan sát
- Luồng bình thường có đủ ba bản cập nhật không?
- Đường hủy nào hoạt động và đường nào không?
- Hủy trước phần tử đầu khác hủy sau phần tử đầu ra sao?
- Sau fix, behavior không hủy và hủy trực tiếp còn đúng không?

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
**Spoiler:** [Reference Solution](solution/README.md) — chỉ xem sau khi tự điều tra.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45–60 phút. Ghi bằng chứng vào [workspace](workspace/my-investigation.md); tham khảo [investigation guide](docs/investigation-guide.md) nếu cần.
