# UNIT-MCLK-001 — Thời lượng bảo trì quạt thông gió đường hầm bị âm

## Mục tiêu
Điều tra duration SLA bất thường, bảo toàn UTC audit timestamps, refactor phép đo và thiết lập regression test xác định.

## Bối cảnh thực tế
Một đội vận hành đường hầm cao tốc ghi thời gian kiểm tra quạt thông gió. Dashboard có bản ghi duration âm hoặc kéo dài bất thường, dù thao tác kiểm tra rất ngắn. UTC start/end phải được giữ để audit.

## Bạn cần làm gì
1. Chạy ba fixture và ghi lại symptoms.
2. Đưa ra hai giả thuyết dựa trên bằng chứng.
3. Chỉ sửa 'starter/JobProbe.cs'; không sửa simulator hoặc assertions.
4. Bảo toàn audit UTC instants và khôi phục duration SLA chính xác.
5. Verify rồi so sánh reference và trade-offs.

## Yêu cầu môi trường
PowerShell 5.1+ hoặc 7, .NET SDK 8.0.x, không cần Docker/Azure/NuGet ngoài.

## Chạy nhanh
~~~powershell
cd 'Daily Real Engineering Lab/units/2026/10/31/UNIT-MCLK-001-tunnel-fan-negative-maintenance-duration'
./setup.ps1
./run.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
'reproduce.ps1' chạy starter gốc và trả thành công chỉ khi chứng minh triệu chứng ban đầu. Nó KHÔNG chứng minh fix. Sau khi sửa starter, chạy 'verify.ps1' để kiểm tra regression.

## Những gì cần quan sát
So sánh DurationMs và UTC start/end của stable, backward-step và forward-step; đọc 'evidence/incident.md'. Ghi hypotheses vào workspace trước khi mở hints.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md).

## Expected Results
[Trước và sau](expected-results/before-after.md). Simulator không phụ thuộc tốc độ máy.

## Estimated Time
50–65 phút.
