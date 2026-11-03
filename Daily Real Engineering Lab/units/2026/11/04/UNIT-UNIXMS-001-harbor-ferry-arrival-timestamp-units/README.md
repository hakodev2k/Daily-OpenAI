# UNIT-UNIXMS-001 — Bảng giờ phà: sai đơn vị Unix timestamp

## Mục tiêu
Tái hiện lỗi ingest thời gian, thu thập bằng chứng, sửa hợp đồng dữ liệu và kiểm chứng boundary.

## Bối cảnh thực tế
Bảng giờ phà không hiển thị chuyến FERRY-17 mặc dù JSON từ đối tác hợp lệ; log ghi nhận lỗi chuyển đổi thời gian.

## Bạn cần làm gì
Điều tra bằng chứng, ghi ít nhất hai hypothesis, sửa `starter/Program.cs`, chạy verify và đánh giá khả năng tương thích API.

## Yêu cầu môi trường
.NET SDK 8, PowerShell; không cần network/service ngoài.

## Chạy nhanh
```powershell
./setup.ps1
./reproduce.ps1
./verify.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` phải in `REJECTED` và exit 0. `verify.ps1` phải fail trên starter ban đầu.

## Những gì cần quan sát
Giá trị JSON, hợp đồng đơn vị của partner, exception và UTC mong đợi; phân biệt đơn vị, timezone và dữ liệu không hợp lệ.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Reference](solution/README.md)

## Expected Results
Sau sửa: `ARRIVAL_UTC=2026-11-03T20:40:00.0000000+00:00` và `VERIFY_PASS`. Verify chạy trên starter đã sửa, không chạy solution.

## Estimated Time
35 phút.
