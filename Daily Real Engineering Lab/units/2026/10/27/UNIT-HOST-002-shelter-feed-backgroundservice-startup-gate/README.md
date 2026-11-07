# UNIT-HOST-002 — Shelter Feed: host startup bị giữ lại bởi warmup

## Mục tiêu
Điều tra vì sao một dịch vụ ASP.NET Core/Generic Host chậm khởi động khi thêm worker đồng bộ dữ liệu sức chứa nơi trú ẩn. Phân biệt **host startup**, **worker readiness** và **completion of background work** bằng bằng chứng thực nghiệm.

## Bối cảnh thực tế
Trong hệ thống điều phối nơi trú ẩn khẩn cấp, API cần sẵn sàng phục vụ truy vấn cơ bản trong khi một background worker chuẩn bị snapshot dữ liệu từ các điểm tiếp nhận. Sau khi thêm warmup, thời điểm host báo startup hoàn tất bị phụ thuộc vào một bước xử lý của worker. Ở môi trường triển khai, điều này kéo dài rollout và làm probe chờ đợi.

Lab cung cấp một cổng kiểm soát deterministic thay cho dịch vụ bên ngoài. Không cần Azure, database hay network.

## Bạn cần làm gì
1. Chạy starter và chứng minh host startup bị giữ lại khi warmup chưa hoàn tất.
2. Thu thập thứ tự sự kiện: worker entered, host StartAsync completed, gate released, warmup completed.
3. Viết ít nhất hai hypothesis; phân biệt host startup với readiness.
4. Chỉnh **chỉ** `starter/ShelterFeedWarmupService.cs` để host có thể hoàn tất startup mà không bỏ qua warmup.
5. Chạy verification, kiểm tra worker vẫn thực sự chạy và shutdown có kiểm soát.
6. So sánh với reference solution sau khi tự thử.

## Yêu cầu môi trường
- .NET 8 SDK (target `net8.0`, `global.json` pin feature band).
- PowerShell 7+ hoặc Windows PowerShell 5.1; không cần package NuGet ngoài shared framework.

## Chạy nhanh
Từ thư mục unit:

```powershell
dotnet --version
dotnet build ./starter/StartupLab.csproj -c Release
./reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Exit code 0 nghĩa là **đã chứng minh được triệu chứng của starter**, không có nghĩa hệ thống đã được sửa.

## Những gì cần quan sát
- Warmup bắt đầu trước khi startup hoàn tất hay không?
- Host StartAsync có hoàn tất trong khi warmup gate vẫn đóng?
- Khi mở gate, cả host startup và warmup có tiến tới trạng thái kết thúc hợp lệ?
- Điều gì sẽ xảy ra nếu hệ thống sử dụng startup probe làm tín hiệu phục vụ traffic?

Ghi bằng chứng vào [workspace/my-investigation.md](workspace/my-investigation.md) và đọc [investigation guide](docs/investigation-guide.md) khi cần.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution và phân tích root cause](solution/README.md). Không mở trước khi thử nghiệm.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45–60 phút. L2 Investigation / D4 Production Troubleshooting.
