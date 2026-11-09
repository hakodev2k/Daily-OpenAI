# UNIT-EFTRACK-001 — Hồ sơ bảo tồn bị đảo trạng thái sau khi cập nhật hàng loạt

## Mục tiêu
Điều tra một sai lệch dữ liệu xảy ra sau hai thao tác hợp lệ trong cùng request; phân biệt giá trị lưu trong database với dữ liệu mà application đang giữ.

## Bối cảnh thực tế
Bảo tàng cập nhật mức ưu tiên bảo tồn của hiện vật qua quy tắc hàng loạt. Audit ghi nhận thao tác cập nhật thành công, nhưng sau khi nhân viên chỉnh sửa ghi chú, mức ưu tiên đôi khi trở về giá trị cũ.

## Bạn cần làm gì
Reproduce, ghi lại ba mốc dữ liệu, nêu ít nhất hai giả thuyết, thay đổi code trong `starter/Program.cs`, rồi chạy verify. Giữ nguyên thao tác ghi chú và yêu cầu cập nhật hàng loạt.

## Yêu cầu môi trường
.NET SDK 8, PowerShell, NuGet restore được Microsoft.EntityFrameworkCore.Sqlite 8.0.11. Không cần dịch vụ cloud.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```

## Cách reproduce vấn đề
Chạy reproduce trước khi chỉnh sửa. Đối chiếu trạng thái ngay sau thao tác hàng loạt và sau khi request hoàn tất.

## Những gì cần quan sát
Ghi giá trị trong database, giá trị application đang nắm giữ, SQL/log theo từng mốc và kết quả lần ghi cuối. Phân biệt giả thuyết về transaction, tracking và thứ tự thao tác.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Phân tích và giải pháp](solution/README.md)

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md)

## Estimated Time
50 phút.
