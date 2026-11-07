# UNIT-CLOSURE-001 — Thông báo bàn giao tuần tra bị gửi sai người

## Mục tiêu
Điều tra lỗi định tuyến thông báo trong batch, phân biệt thời điểm tạo công việc và thời điểm công việc thực sự chạy; refactor mà không thay đổi thứ tự hoặc số lượng thông báo.

## Bối cảnh thực tế
Một hệ thống quản lý tuần tra rừng quốc gia chuẩn bị thông báo bàn giao ca cho các kiểm lâm viên. API chỉ xếp công việc vào hàng đợi nội bộ; một worker xử lý sau khi toàn bộ batch được chuẩn bị. Dashboard cho thấy đã xếp đủ bốn công việc nhưng một người nhận nhiều thông báo, các người khác không nhận gì.

## Bạn cần làm gì
1. Chạy baseline để xác nhận triệu chứng.
2. Đọc output và ghi ít nhất hai giả thuyết trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Chỉ sửa `starter/NotificationBatchScheduler.cs`; không sửa fixture, oracle hay scripts.
4. Chạy verify và giải thích tại sao việc enqueue thành công không chứng minh recipient chính xác.
5. So sánh với reference solution sau khi tự thử.

## Yêu cầu môi trường
- .NET SDK 8.0.100 trở lên, có khả năng build `net8.0`; PowerShell 7+ hoặc Windows PowerShell 5.1.
- Không cần database, Docker, Azure, NuGet bên thứ ba hay dữ liệu thật.

## Chạy nhanh
Tại thư mục unit:

```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```

`reproduce.ps1` dùng baseline cố định, độc lập với code bạn sửa. `verify.ps1` dùng chính `starter/NotificationBatchScheduler.cs`.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1`. Fixture xếp bốn thông báo rồi mới cho worker xử lý; output hiển thị hàng đợi có đủ bốn mục, nhưng kết quả giao nhận không khớp với danh sách yêu cầu. Lệnh reproduce **PASS** khi phát hiện đúng triệu chứng baseline.

## Những gì cần quan sát
- Số mục trong hàng đợi trước khi worker chạy.
- Có phát sinh giao nhận sớm trong giai đoạn enqueue không.
- Số lượng, thứ tự và người nhận thực tế sau khi worker chạy.
- Dữ liệu của từng mục tại lúc xếp hàng so với lúc xử lý.

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
**Spoiler — chỉ mở sau khi điều tra:** [solution/README.md](solution/README.md)

## Expected Results
- Trước fix: baseline chứng minh được mismatch, dù enqueue đủ bốn mục.
- Sau fix: bốn thông báo đúng từng recipient, đúng thứ tự, không gửi sớm; verify PASS.
- Xem thêm [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time
45–65 phút (L4 refactoring / D4).
