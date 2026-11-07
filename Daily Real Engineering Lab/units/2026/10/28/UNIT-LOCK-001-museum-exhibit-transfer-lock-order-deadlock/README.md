# UNIT-LOCK-001 — Chuyển hiện vật giữa hai kho bị treo khi xử lý đồng thời

## Mục tiêu

Tái hiện sự cố concurrency trong hệ thống quản lý hiện vật, thu thập bằng chứng, phân tích nguyên nhân và sửa lỗi mà vẫn giữ invariant tồn kho.

## Bối cảnh thực tế

Bảo tàng quản lý hai kho NORTH và SOUTH. Mỗi yêu cầu chuyển hiện vật riêng lẻ thành công. Khi hai nhân viên chuyển hiện vật theo hai chiều đồng thời, đôi lúc cả hai yêu cầu không kết thúc, dashboard đứng yên dù CPU không cao. Fixture mô phỏng một incident backend với hai worker độc lập.

## Bạn cần làm gì

1. Chạy starter, ghi output, exit code và thời gian chờ.
2. Ghi ít nhất hai giả thuyết và bằng chứng để phân biệt.
3. Sửa duy nhất logic trong `starter/GalleryInventory.cs`, không né runner.
4. Verify tính hoàn thành và bảo toàn tổng số hiện vật.
5. Giải thích giới hạn của bản sửa khi triển khai nhiều process/pod.

## Yêu cầu môi trường

.NET SDK 8.0+, .NET 8 runtime và PowerShell 5.1+ hoặc PowerShell 7+. Không cần database, Docker, NuGet ngoài hay Azure.

## Chạy nhanh

Tại thư mục unit:

```powershell
./reproduce.ps1
# Sau khi sửa starter/GalleryInventory.cs:
./verify.ps1
```

`./run.ps1` chạy starter trực tiếp.

## Cách reproduce vấn đề

`reproduce.ps1` build starter và chạy hai yêu cầu chuyển kho ngược chiều trong cùng một cửa sổ concurrency. Script PASS khi quan sát được failure dự kiến của bản gốc; sau khi sửa, reproduce không còn PASS là đúng.

## Những gì cần quan sát

- Hai yêu cầu bắt đầu nhưng không hoàn thành trước deadline fixture.
- Output `DEADLOCK_DETECTED`, exit code `2`.
- Phân biệt chờ tài nguyên với CPU bottleneck, I/O timeout và exception nghiệp vụ.
- Sau sửa: `PASS: both transfers completed; NORTH=10; SOUTH=10; TOTAL=20`.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution

**Spoiler — chỉ mở sau khi tự điều tra:** [Phân tích và trade-offs](solution/README.md) và [mã tham khảo](solution/GalleryInventory.cs).

## Expected Results

[Before](expected-results/before.md) · [After](expected-results/after.md). Ghi kết quả vào [workspace](workspace/my-investigation.md).

## Estimated Time

50–65 phút.
