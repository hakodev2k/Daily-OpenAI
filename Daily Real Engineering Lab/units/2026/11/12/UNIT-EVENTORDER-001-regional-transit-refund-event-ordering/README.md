# UNIT-EVENTORDER-001 — Hoàn tiền vé tàu bị đảo trạng thái

## Mục tiêu
Điều tra projection sự kiện bất đồng bộ, thu thập evidence và thiết kế invariant khi duplicate/reorder.

## Bối cảnh thực tế
Ledger xác nhận vé T-731 đã hoàn tiền nhưng màn hình chăm sóc khách hàng đôi khi lại hiển thị Paid sau retry. Các thông báo đều hợp lệ.

## Bạn cần làm gì
Reproduce; viết hai hypothesis; sửa `starter/Program.cs`; verify; viết ADR về consistency, sequence gap, scale nhiều consumer.

## Yêu cầu môi trường
.NET SDK 8.x; PowerShell 7+ nếu dùng scripts.

## Chạy nhanh
`dotnet run --project starter/TicketReplay.csproj -- --reproduce`

## Cách reproduce vấn đề
Chạy `./reproduce.ps1` tại thư mục lab. Script exit 0 khi symptom xuất hiện.

## Những gì cần quan sát
So sánh status với ledger, sequence và arrival order; xem các lịch phát khác nhau có ảnh hưởng kết quả không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[hints/01.md](hints/01.md), [hints/02.md](hints/02.md), [hints/03.md](hints/03.md)

## Reference Solution
**Spoiler:** [solution/README.md](solution/README.md)

## Expected Results
Reproduce thấy mismatch; verify ban đầu FAIL, sau fix PASS với ba permutation. Đề xuất production phải xét multi-instance atomicity và missing sequence.

## Estimated Time
65 phút.
