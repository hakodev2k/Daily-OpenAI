# UNIT-JSONLIFE-001 — JSON khảo sát biển lỗi sau bước import
## Mục tiêu
Reproduce, điều tra ownership của JSON DOM, sửa và kiểm tra regression.
## Bối cảnh thực tế
Worker tiếp nhận bản ghi từ tàu nghiên cứu. Parse thành công nhưng downstream không đọc được stationId. Network latency bình thường.
## Bạn cần làm gì
Reproduce, ghi 2 hypothesis, tìm evidence từ stack trace, sửa starter và verify.
## Yêu cầu môi trường
.NET SDK 8.0, PowerShell 7; không cần package ngoài.
## Chạy nhanh
`pwsh ./reproduce.ps1`, sau khi sửa `pwsh ./verify.ps1`.
## Cách reproduce vấn đề
Chạy script reproduce từ thư mục unit, ghi lại exception type và vị trí throw.
## Những gì cần quan sát
Phân biệt lỗi parse với lỗi xảy ra khi xử lý downstream; kiểm tra lifetime qua service boundary.
## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.
## Hints
[1](hints/01.md) · [2](hints/02.md) · [3](hints/03.md)
## Reference Solution
**Spoiler:** [Lời giải](reference-solution/README.md)
## Expected Results
Trước fix: downstream ném ObjectDisposedException. Sau fix: hai bản ghi được đọc chính xác.
## Estimated Time
50 phút, L2 / D4. [Workspace](workspace/my-investigation.md).
