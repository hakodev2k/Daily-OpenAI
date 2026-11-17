# UNIT-GETORADD-001 — Hai vé được cấp, chỉ một vé được lưu

## Mục tiêu
Điều tra tính đúng đắn của thao tác cấp mã vé khi nhiều request đồng thời sử dụng cache.

## Bối cảnh thực tế
Cổng bán vé lễ hội nhận hai yêu cầu cùng mã sự kiện. Dashboard báo một mã được lưu nhưng hệ thống cấp mã ghi nhận hai lần.

## Bạn cần làm gì
Chạy starter, ghi lại số lần cấp mã, phân biệt kết quả lưu và side effect, đưa giả thuyết, sửa tại `starter/Program.cs`, rồi xác nhận.

## Yêu cầu môi trường
.NET SDK 8 hoặc mới hơn; PowerShell 7 hoặc Windows PowerShell; không cần cloud hay database.

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Lệnh phải quan sát được lỗi nghiệp vụ và trả mã lỗi khác 0 trước khi sửa.

## Những gì cần quan sát
Ghi lại `Stored`, `Returned`, `Allocations`, exit code. Vì sao hai request cùng key vẫn tạo ra hai sự kiện cấp mã?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ xem [solution](solution/README.md) sau khi tự verify.

## Expected Results
Ban đầu: hai side effects, một giá trị trong cache. Sau khi sửa: một side effect, hai caller nhận cùng giá trị.

## Estimated Time
45 phút.
