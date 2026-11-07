# UNIT-SCHED-001 — Đối soát cửa sổ giờ cho ad impressions

## Mục tiêu
Tự reproduce một lỗi vận hành trong worker .NET, phân biệt **lịch đánh thức** và **nghĩa vụ xử lý từng cửa sổ nghiệp vụ**, thu thập bằng chứng rồi refactor để dữ liệu không bị bỏ sót khi worker chậm.

## Bối cảnh thực tế
Một nền tảng quảng cáo tính số lượt hiển thị theo từng giờ để xuất hóa đơn. Worker thường hoàn thành đúng hạn; sau một đợt downstream chậm, dashboard báo số giờ đã chốt thấp hơn số giờ đã trôi qua. Không có exception và CPU bình thường. Mỗi giờ phải được chốt một lần theo thứ tự, kể cả sau một khoảng gián đoạn.

## Bạn cần làm gì
1. Chạy hệ thống starter, ghi lại các mốc thời gian và số cửa sổ được chốt.
2. Đề xuất ít nhất hai hypothesis dựa trên evidence.
3. Chỉ sửa **`starter/WindowScheduler.cs`** để worker xử lý đủ cửa sổ đã đến hạn mà không chốt nhầm hoặc vượt qua cửa sổ thất bại.
4. Chạy verification trên **starter đã sửa**, rồi mới đọc reference solution.

## Yêu cầu môi trường
.NET SDK 8+ (mục tiêu `net8.0`), PowerShell 7+ hoặc Windows PowerShell 5.1; không cần database, Docker hay Azure.

## Chạy nhanh
Từ thư mục unit:

```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```

Có thể dùng trực tiếp `dotnet run --project ./starter/AdWindowLab.csproj -- reproduce` và `dotnet run --project ./starter/AdWindowLab.csproj -- verify`.

## Cách reproduce vấn đề
Chạy `./reproduce.ps1`. Chương trình mô phỏng một đoạn worker bị chậm, sau đó dùng lịch nghiệp vụ 5 giờ đã đến hạn để đối chiếu số cửa sổ thực sự được ghi nhận. Script thành công khi **phát hiện** sai lệch trong starter; đây không phải kiểm tra bản sửa.

## Những gì cần quan sát
- Số khoảng thời gian nghiệp vụ đã trôi qua so với số bản ghi hoàn thành.
- `LastCompletedWindow` sau một lần worker được đánh thức.
- Số lần xử lý được gọi, thứ tự các cửa sổ và trạng thái khi downstream lỗi.
- Không dựa vào thời gian chạy chính xác trên mọi máy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix ở starter.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham khảo](solution/README.md) — chỉ mở sau khi đã thử tự sửa.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
60–75 phút (L4 / D4).