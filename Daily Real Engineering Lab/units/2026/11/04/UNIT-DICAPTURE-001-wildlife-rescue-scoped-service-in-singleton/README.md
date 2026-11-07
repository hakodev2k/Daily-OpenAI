# UNIT-DICAPTURE-001 — Thông báo ca trực cứu hộ gửi sai người nhận

## Mục tiêu
Điều tra và refactor lifetime của dependency trong một background worker, không làm thay đổi giao diện gọi Send.

## Bối cảnh thực tế
Một trạm cứu hộ động vật gửi thông báo theo ca trực. Ca A nhận thông báo đúng; khi chuyển sang ca B, thông báo vẫn ghi người nhận thuộc ca A. Hệ thống không ném exception.

## Bạn cần làm gì
Tái hiện hai ca liên tiếp; ghi evidence và ít nhất hai hypothesis; sửa code trong `starter/Program.cs`; kiểm tra không còn rò rỉ ngữ cảnh giữa các ca; đánh giá lifetime validation và cleanup.

## Yêu cầu môi trường
PowerShell 7+ và .NET SDK 8.0; không cần database, Azure hay NuGet bên ngoài.

## Chạy nhanh
Tại thư mục unit: `pwsh ./run.ps1`

## Cách reproduce vấn đề
`pwsh ./reproduce.ps1` chạy starter và xác nhận thông báo ca B sai. Script trả exit code 0 khi đã tái hiện thành công.

## Những gì cần quan sát
Đối chiếu recipient của hai dòng output, thời điểm chuyển ca, và lifecycle của object chứa ngữ cảnh. Không suy luận chỉ từ exit code của ứng dụng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Chạy `pwsh ./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md)

## Expected Results
Trước sửa: dòng thứ hai vẫn có recipient=shift-A. Sau sửa: recipient lần lượt shift-A và shift-B; hai thông báo vẫn có animal tương ứng.

## Estimated Time
65 phút (L4 / D4).
