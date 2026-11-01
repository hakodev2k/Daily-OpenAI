# UNIT-CANCELREG-001 — Dừng worker đèn đường băng nhưng tín hiệu shutdown không được xác nhận

## Mục tiêu
Tách việc xác nhận tín hiệu shutdown khỏi việc hoàn tất cleanup; điều tra một lỗi concurrency có thể reproduce cục bộ.

## Bối cảnh thực tế
Worker theo dõi đèn đường băng cần dừng trước đợt bảo trì. Khi downstream chưa cho phép flush dữ liệu cuối ca, dashboard thấy yêu cầu shutdown bị treo dù dữ liệu vẫn được flush sau đó.

## Bạn cần làm gì
Chạy starter, thu thập timeline, đưa ra ít nhất hai giả thuyết, sửa `starter/ShutdownCoordinator.cs` để tín hiệu dừng được xác nhận trước khi downstream sẵn sàng, nhưng cleanup vẫn được chờ và không bị nhân đôi.

## Yêu cầu môi trường
.NET SDK 8 (xem `global.json`), PowerShell; không cần Docker, Azure hay NuGet bên ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chủ động giữ downstream gate rồi kiểm tra trạng thái lời gọi shutdown. Exit code 0 nghĩa là **đã reproduce triệu chứng**, không có nghĩa đã fix.

## Những gì cần quan sát
Ghi thời điểm callback bắt đầu, thời điểm lời gọi shutdown trả về, trạng thái gate, số lần flush trước/sau khi gate mở. Phân biệt tín hiệu đã nhận với cleanup đã xong.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Sửa starter; không chỉnh test harness để làm xanh.
4. Chạy `./verify.ps1` (kiểm tra chính starter đã sửa).
5. Chỉ sau đó xem reference solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải](solution/README.md) — chỉ đọc sau khi tự thử.

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút. [Investigation guide](docs/investigation-guide.md).
