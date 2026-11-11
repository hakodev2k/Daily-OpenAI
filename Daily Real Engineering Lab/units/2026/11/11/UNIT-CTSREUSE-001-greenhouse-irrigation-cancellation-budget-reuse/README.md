# UNIT-CTSREUSE-001 — Greenhouse Irrigation: Cancelled Second Cycle

## Mục tiêu
Điều tra một lỗi vòng đời cancellation trong .NET worker chạy nhiều chu kỳ; phân biệt deadline của từng lần vận hành với lifetime của worker.

## Bối cảnh thực tế
Gateway nhà kính nhận hai đợt tưới liên tiếp. Đợt đầu hoàn tất bình thường, nhưng đợt thứ hai bị hủy ngay cả khi cảm biến và actuator vẫn khỏe. Dashboard ghi `OperationCanceledException`, CPU và mạng không bất thường.

## Bạn cần làm gì
1. Chạy baseline, ghi lại hai kết quả và thời gian.
2. Đề xuất ít nhất hai giả thuyết dựa trên trace.
3. Chỉnh `starter/Program.cs` để mỗi chu kỳ có budget riêng, vẫn tôn trọng shutdown token.
4. Chạy verify; giải thích vì sao kết quả đúng và giới hạn của cách sửa.

## Yêu cầu môi trường
.NET SDK 8.0; PowerShell 7+ trên Windows, Linux hoặc macOS. Không cần Docker, cloud hoặc NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
pwsh ./scripts/run.ps1
pwsh ./scripts/reproduce.ps1
```

## Cách reproduce vấn đề
Chạy `pwsh ./scripts/reproduce.ps1` trên starter nguyên bản. Script chỉ PASS khi tái hiện được chu kỳ thứ hai bị hủy.

## Những gì cần quan sát
So sánh `cycle=1` và `cycle=2`, trạng thái `cancelled`, thời điểm bắt đầu và số hành động thành công. Ghi bằng chứng vào workspace trước khi mở hints.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Solution](solution/Program.cs) — chỉ mở sau khi tự thử.

## Expected Results
Baseline: chu kỳ 1 thành công, chu kỳ 2 bị hủy. Sau sửa: cả hai thành công; nếu shutdown được yêu cầu thì công việc vẫn có thể hủy. [Chi tiết](expected-results/results.md).

## Estimated Time
50 phút (L3 / D4).
