# UNIT-XASSERT-001 — CI xanh nhưng thẻ hết hạn vẫn mở cổng bảo trì đường sắt

## Mục tiêu
Điều tra một sự cố mà test suite báo thành công trong khi một quy tắc truy cập quan trọng không được thực thi. Tách biệt **hành vi production**, **độ tin cậy của test** và **độ nhạy của test với regression**.

## Bối cảnh thực tế
Hệ thống kiểm soát cổng khu bảo trì đường sắt kiểm tra thẻ nhân viên nhà thầu trước khi mở cửa. Sau một đợt cập nhật chính sách, kiểm tra thủ công cho thấy thẻ hết hạn vẫn được chấp nhận, nhưng CI vẫn báo tất cả unit tests xanh. Không có database hay cloud dependency trong bản tái hiện.

## Bạn cần làm gì
1. Reproduce hiện tượng CI xanh nhưng business contract sai.
2. Ghi ít nhất hai giả thuyết và chứng cứ bác bỏ/xác nhận.
3. Sửa cả policy và khả năng phát hiện regression của test.
4. Verify bằng test thường, independent acceptance probe và mutation/fault-injection gate.
5. So sánh với reference solution sau khi tự thử.

## Yêu cầu môi trường
- Windows PowerShell 7 hoặc Windows PowerShell 5.1.
- .NET SDK 8.x; `global.json` ưu tiên 8.0.100 và cho phép roll-forward trong feature band.
- NuGet network chỉ cần cho lần restore package xUnit đầu tiên. Không cần dịch vụ trả phí.

## Chạy nhanh
Tại thư mục unit:
```powershell
dotnet --version
./reproduce.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```
Script chạy test suite và một phép kiểm tra độc lập trên cùng starter implementation.

## Những gì cần quan sát
- Kết quả test suite và phép kiểm tra độc lập có nhất quán không?
- Một test có thực sự quan sát được kết quả cuối cùng của tác vụ bất đồng bộ không?
- Cả thẻ hợp lệ, thẻ bị thu hồi và thẻ hết hạn có được kiểm tra riêng biệt không?
- Log/exit code nào chứng minh lỗi được tái hiện?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự điều tra:** [Reference solution](solution/README.md).

## Expected Results
Xem [before / after](expected-results/before-after.md). `reproduce.ps1` xác nhận hiện tượng lỗi, còn `verify.ps1` phải chỉ thành công sau khi sửa learner-editable starter.

## Estimated Time
45–60 phút. Difficulty D4, maturity L2.
