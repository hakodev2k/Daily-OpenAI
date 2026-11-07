# UNIT-REDIS-006 — Máy laser cutter bị giữ chỗ sau khi tiến trình đặt lịch dừng đột ngột

## Mục tiêu

Điều tra một sự cố tính nhất quán giữa trạng thái giữ chỗ và vòng đời của ứng dụng; tái hiện bằng crash injection, thu thập bằng chứng, đề xuất sửa đổi và chứng minh các bất biến của hợp đồng đặt lịch.

## Bối cảnh thực tế

Một makerspace cho phép giữ máy laser cutter trong 2 phút để người dùng hoàn tất đặt lịch. Sau một lần tiến trình API dừng đột ngột, một máy bị báo “đang được giữ” quá thời hạn. Các yêu cầu bình thường trước đó hoạt động ổn định; log API không có thông tin đầy đủ về trạng thái sau sự cố.

## Bạn cần làm gì

1. Chạy tình huống crash có kiểm soát và ghi lại các dấu hiệu.
2. So sánh với trường hợp không có crash, xác định bất biến thời hạn giữ chỗ bị phá vỡ.
3. Nêu ít nhất hai giả thuyết và bằng chứng phân biệt chúng.
4. Sửa mã trong `starter/ReservationLeaseService.cs`, không thay đổi simulator, probe hoặc các script kiểm chứng.
5. Chạy kiểm chứng và đánh giá ảnh hưởng khi chuyển từ simulator sang Redis thật.

## Yêu cầu môi trường

- Windows PowerShell 5.1+ hoặc PowerShell 7.
- .NET SDK 8.x; không cần Docker, NuGet ngoài framework hoặc tài khoản cloud.
- `starter/SimulatedRedis.cs` mô phỏng hành vi key, expiry và ranh giới crash; không phải một Redis server thật.

## Chạy nhanh

Tại thư mục unit:

```powershell
dotnet --version
powershell -ExecutionPolicy Bypass -File .\reproduce.ps1
```

## Cách reproduce vấn đề

Chạy `reproduce.ps1` trên starter nguyên bản. Probe tiêm một lần dừng tiến trình ngay sau một bước ghi trạng thái, khởi động lại client và kiểm tra khả năng đặt lịch sau 3 phút thời gian mô phỏng. Thời gian mô phỏng được điều khiển trực tiếp, không cần chờ thật.

## Những gì cần quan sát

Ghi `CRASH_INJECTED`, `HOLD_EXISTS_AFTER_RESTART`, `FINITE_TTL_AFTER_RESTART`, `RETRY_AFTER_3_MINUTES`, `NORMAL_CONTRACT_OK`. Tập trung vào trạng thái sau khi client khởi động lại, không chỉ vào HTTP response của request ban đầu.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi tự điều tra và thử sửa.

## Expected Results

- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time

35–55 phút. Không cần triển khai Redis thật để hoàn thành bài thực hành cốt lõi.
