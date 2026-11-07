# UNIT-DOTNET-016 — Một kết quả async, hai lần quan sát

## Mục tiêu
Điều tra một lỗi async contract trong .NET khi cùng một kết quả bất đồng bộ được quan sát nhiều hơn một lần.

## Bối cảnh thực tế
Một background component đọc dữ liệu từ một nguồn hiệu năng cao. Luồng xử lý chính nhận được kết quả bình thường, nhưng nhánh telemetry đôi khi làm cùng operation thất bại với `InvalidOperationException` dù không có lỗi I/O.

## Bạn cần làm gì
Reproduce lỗi, ghi hypothesis, xác định contract bị vi phạm, sửa code trong `starter/` và chạy verification. Giữ nguyên yêu cầu: operation nguồn chỉ được khởi tạo một lần.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` build và chạy starter với một nguồn async deterministic mô phỏng primitive hiệu năng cao.

## Những gì cần quan sát
- Operation nguồn được tạo bao nhiêu lần.
- Lần quan sát đầu và lần quan sát tiếp theo có cùng behavior hay không.
- Exception xuất hiện ở thời điểm tạo operation hay thời điểm consume kết quả.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Chạy `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[solution/README.md](solution/README.md) — spoiler, chỉ xem sau khi đã thử fix.

## Expected Results
**Before:** lần consume thứ hai làm process báo lỗi contract.  
**After:** cả business processing và telemetry nhận cùng logical result, trong khi source operation vẫn chỉ được khởi tạo một lần.

## Estimated Time
35–50 phút.