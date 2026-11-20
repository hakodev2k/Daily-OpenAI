# UNIT-CHANNELCLOSE-001 — Pipeline tiếp nhận cứu trợ mất hồ sơ

## Mục tiêu
Điều tra và khắc phục một sai lệch giữa hồ sơ đã được tiếp nhận và hồ sơ xuất hiện trong báo cáo batch. Rèn luyện đọc timeline, lập giả thuyết và kiểm chứng invariant.

## Bối cảnh thực tế
Hai cổng tiếp nhận gửi dữ liệu vào pipeline xử lý hồ sơ sơ tán. Cuối đợt, báo cáo được đánh dấu hoàn tất nhưng thiếu một hồ sơ đã được cổng tiếp nhận xác nhận.

## Bạn cần làm gì
Chạy fixture, ghi lại thứ tự event và ít nhất hai giả thuyết; tìm nguyên nhân, sửa `starter/Program.cs`, kiểm tra cả correctness và shutdown semantics. Không thay assertion.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell; không cần dịch vụ ngoài hoặc NuGet package.

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Từ thư mục unit chạy `./reproduce.ps1`, rồi chạy `dotnet run --project ./starter/ShelterIntake.csproj -- --check` để quan sát exit code của invariant.

## Những gì cần quan sát
Ghi `accepted`, `processed`, `missing` và thứ tự các event producer/writer/reader. So sánh thời điểm tuyên bố batch hoàn tất với thời điểm từng nguồn thực sự có thể gửi dữ liệu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Chạy `./verify.ps1`.
5. Sau đó mới xem solution.

## Hints
[01](hints/hint-01.md), [02](hints/hint-02.md), [03](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Giải thích](solution/README.md), [source tham chiếu](solution/Program.cs).

## Expected Results
Xem [before/after](expected-results.md): trước thiếu một hồ sơ; sau đầy đủ và reader kết thúc sạch.

## Estimated Time
45 phút.
