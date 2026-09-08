# UNIT-DOTNET-003 — Periodic job overlap under slow execution

## Mục tiêu
Điều tra một background job chạy định kỳ có nhiều execution chồng lấn khi một lần xử lý kéo dài hơn chu kỳ cấu hình.

## Bối cảnh thực tế
Một worker đồng bộ trạng thái đơn hàng chạy định kỳ. Khi downstream phản hồi chậm, log cho thấy nhiều execution có thể cùng hoạt động trong một thời điểm.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi ít nhất 2 hypotheses.
3. Dùng timestamp và `active=` trong log làm evidence.
4. Sửa code trong `starter/` để mỗi vòng chỉ bắt đầu sau khi vòng trước kết thúc.
5. Chạy `verify.ps1`.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 5.1+ hoặc PowerShell 7+

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Timestamp của `START` và `END`.
- Giá trị `active=`.
- Có thời điểm `active` lớn hơn 1 hay không.
- Job có tiếp tục chạy nhiều vòng hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
> Reference Solution — inspect only after reproducing the issue and attempting your own fix.
- [Reference Solution](solution/README.md)

## Expected Results
**Before:** có thời điểm nhiều hơn một execution đang active.

**After:** `active` không vượt quá 1 và job vẫn chạy nhiều vòng tuần tự.

## Estimated Time
35–50 phút.
