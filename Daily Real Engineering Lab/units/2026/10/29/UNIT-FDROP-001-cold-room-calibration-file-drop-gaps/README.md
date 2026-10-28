# UNIT-FDROP-001 — Hồ sơ hiệu chuẩn đã có trên đĩa nhưng biến mất khỏi sổ kiểm định

## Mục tiêu
Điều tra một sai lệch giữa thư mục nhận hồ sơ và sổ kiểm định của hệ thống cold-room; xây dựng bằng chứng, sửa luồng xử lý và xác nhận không bỏ sót hoặc xử lý trùng.

## Bối cảnh thực tế
Một dịch vụ .NET nhận chứng nhận hiệu chuẩn thiết bị qua thư mục drop-off do hệ thống hiện trường đồng bộ. Ca đêm ghi nhận ba tệp JSON hợp lệ đã chuyển tới máy chủ, nhưng báo cáo cuối ca chỉ có hai chứng nhận. Không có exception và hai chứng nhận còn lại vẫn được ghi đúng.

Fixture mô phỏng luồng notification từ hệ thống tệp theo cách có thể lặp lại; không cần kết nối thiết bị, cloud hay tạo FileSystemWatcher thực.

## Bạn cần làm gì
1. Chạy baseline và ghi lại ID có trên đĩa so với ID trong sổ tiếp nhận.
2. Đưa ít nhất hai hypothesis, chỉ ra bằng chứng loại trừ.
3. Chỉnh **starter/CertificateIntake.cs** để việc đối soát cuối ca đúng và chạy lại không phát sinh bản ghi trùng.
4. Chạy verification; ghi trade-off cho môi trường nhiều worker.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ (hoặc Windows PowerShell 5.1), không cần NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet --version
./run.ps1
./reproduce.ps1
./verify.ps1
```
Trước khi sửa, `reproduce.ps1` phải xác nhận triệu chứng và `verify.ps1` phải thất bại. Sau khi sửa, `verify.ps1` phải thành công.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Harness tạo thư mục tạm, ba hồ sơ hoàn chỉnh và một chuỗi callback xác định; kết quả được in ra console.

## Những gì cần quan sát
So sánh các ID `CAL-101`, `CAL-102`, `CAL-103` trong thư mục với danh sách `ACCEPTED`. Kiểm tra số bản ghi khi một callback xuất hiện hai lần. Không dùng thông báo “không có exception” làm bằng chứng đủ về tính đầy đủ dữ liệu.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng script độc lập.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) sau khi đã tự thử sửa.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
50 phút (reproduce 10, điều tra 15, sửa 15, kiểm chứng và trade-off 10).
