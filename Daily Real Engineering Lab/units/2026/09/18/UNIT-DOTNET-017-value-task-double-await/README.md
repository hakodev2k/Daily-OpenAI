# UNIT-DOTNET-017 — ValueTask Consumption Contract in a Hot Path

## Mục tiêu
Điều tra một read-through service đã được tối ưu allocation nhưng bắt đầu phát sinh exception ở một nhánh xử lý hiếm gặp.

## Bối cảnh thực tế
Inventory service trả kết quả nhanh từ cache ở phần lớn request. Sau một optimization gần đây, telemetry cho thấy request thông thường vẫn thành công nhưng nhánh audit đôi lúc thất bại sau khi kết quả inventory đã được lấy thành công.

## Bạn cần làm gì
1. Chạy starter và reproduce.
2. Ghi lại thứ tự các async operation và nơi exception xuất hiện.
3. Đưa ra ít nhất hai hypothesis trước khi sửa.
4. Sửa code trong `starter/` mà không thay đổi functional contract.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell

## Chạy nhanh
```powershell
./run.ps1
```

## Cách reproduce vấn đề
```powershell
./reproduce.ps1
```

## Những gì cần quan sát
- Operation nào hoàn tất trước khi exception xuất hiện.
- Exception type/message.
- Số lần async result được consume trong request path.
- Sự khác nhau giữa normal path và audit path.

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
[Reference Solution — spoiler](solution/README.md)

## Expected Results
Before: normal path hoàn tất nhưng audit path có deterministic failure sau lần lấy result đầu tiên.

After: cả normal path và audit path đều hoàn tất, cùng một underlying operation không bị thực thi lại, và verification pass.

Nếu không reproduce được, xác nhận đang chạy .NET 8 SDK và chạy script từ thư mục unit.

## Estimated Time
40 phút.