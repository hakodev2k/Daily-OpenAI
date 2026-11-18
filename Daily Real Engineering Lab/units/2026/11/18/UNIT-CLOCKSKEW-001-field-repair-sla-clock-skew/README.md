# UNIT-CLOCKSKEW-001 — SLA bảo trì hiện duration âm

## Mục tiêu
Điều tra và sửa phép đo SLA duration của ticket bảo trì thiết bị.

## Bối cảnh thực tế
Sau khi máy chủ đồng bộ thời gian, dashboard hiện một ticket có duration âm dù tác vụ hoàn thành.

## Bạn cần làm gì
Chạy starter, lưu evidence, nêu hai giả thuyết, tự sửa và verify; giữ lại timestamp audit.

## Yêu cầu môi trường
.NET SDK 8+ và PowerShell.

## Chạy nhanh
`dotnet run --project starter/ClockLab.csproj`

## Cách reproduce vấn đề
`./reproduce.ps1`

## Những gì cần quan sát
Ghi StartedAt, FinishedAt, DurationMs, Verdict. Xác định độ tin cậy của số đo, không chỉ dựa vào dấu của duration.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md).

## Expected Results
Trước sửa: `DurationMs=-2000`, `Verdict=FAIL`. Sau sửa: duration 125ms, `Verdict=PASS`.

## Estimated Time
35–45 phút (L2 / D4).
