# UNIT-CDICT-001 — Một flight corridor nhưng cơ quan cấp phép nhận hai yêu cầu

## Mục tiêu
Điều tra một lỗi concurrency trong dịch vụ cấp phép bay drone: hai request hợp lệ cùng khóa nhận cùng mã permit trong cache, nhưng hệ thống bên ngoài lại ghi nhận nhiều lần cấp phép. Reproduce, thu thập evidence, sửa và kiểm tra cả tính đúng đắn lẫn số side effect.

## Bối cảnh thực tế
Dịch vụ .NET của một nhà vận hành drone tra cứu hoặc cấp mới permit cho mỗi flight corridor. Một đợt request đồng thời khiến cơ quan cấp phép ghi nhận hai lần cấp, trong khi dashboard nội bộ chỉ hiển thị một permit. Các request riêng lẻ không có vấn đề.

Fixture sử dụng fake authority có cổng đồng bộ xác định để tái hiện contention, không gọi API thật hay tốn phí.

## Bạn cần làm gì
1. Chạy `./reproduce.ps1` và ghi lại `issued`, `sameCachedPermit`, `issuedAfterOtherKey`.
2. Đưa ít nhất hai hypothesis; phân biệt cache corruption, race và external retry.
3. Sửa **starter/PermitRegistry.cs** để một khóa chỉ gây một lần cấp phép trong quá trình tranh chấp, nhưng khóa khác vẫn được cấp độc lập.
4. Chạy `./verify.ps1` và phân tích cách xử lý khi external authority thất bại.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+ hoặc Windows PowerShell 5.1; không cần NuGet, database, Docker hay cloud.

## Chạy nhanh
Từ thư mục unit:
```powershell
dotnet --version
./reproduce.ps1
./verify.ps1
```
Trước khi sửa, `reproduce.ps1` xác nhận symptom và `verify.ps1` thất bại. Sau khi sửa, `verify.ps1` phải thành công. `./run.ps1` in snapshot quan sát thông thường.

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Harness chạy hai worker có điểm bắt đầu đồng thời và fake external authority. Baseline không phụ thuộc network hay timing ngẫu nhiên của API bên ngoài.

## Những gì cần quan sát
Một permit được trả cho cả hai caller, nhưng số lần gọi cơ quan cấp phép lớn hơn một. So sánh số entry trong cache với số side effect thực tế; kiểm tra cùng khóa được gọi lại và một khóa khác được tạo mới.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó xem reference solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ mở [solution/README.md](solution/README.md) sau khi đã tự điều tra và sửa.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
45 phút (reproduce 10, investigation 15, fix 10, verify và trade-off 10).
