# UNIT-EF-009 — Tracking identity collision

## Mục tiêu
Điều tra một lỗi EF Core xuất hiện trong luồng cập nhật dữ liệu khi cùng một business operation vừa đọc entity hiện tại vừa nhận model thay đổi từ request.

## Bối cảnh thực tế
Một API quản trị sản phẩm đọc dữ liệu hiện tại để kiểm tra business rule, sau đó áp dụng dữ liệu cập nhật. Request hợp lệ nhưng update thất bại ở runtime. Một số developer đề xuất tách thêm Repository hoặc gọi `SaveChanges` sớm hơn, nhưng chưa có evidence cho thấy các thay đổi đó giải quyết đúng vấn đề.

## Bạn cần làm gì
1. Chạy starter và reproduce lỗi.
2. Ghi lại entity instances nào đang tồn tại trong operation.
3. Quan sát trạng thái mà `DbContext` đang quản lý trước thời điểm lỗi.
4. Đưa ra hypothesis trước khi sửa.
5. Sửa `starter/` nhưng giữ nguyên business contract.
6. Chạy `verify.ps1` để xác nhận update thành công và dữ liệu không bị tạo thêm ngoài ý muốn.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script build và chạy một console app nhỏ dùng EF Core InMemory provider để mô phỏng request update.

## Những gì cần quan sát
- Exception type và message.
- Số lượng entity instances đại diện cho cùng database row trong operation.
- `ChangeTracker` entries ngay trước thao tác thất bại.
- Việc đọc dữ liệu trước update có ảnh hưởng thế nào đến state của context.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
Xem `hints.md` theo thứ tự.

## Reference Solution
`solution/README.md` — chỉ xem sau khi đã thử fix.

## Expected Results
Before: update operation thất bại ổn định và evidence cho thấy context đang quản lý state không phù hợp với cách update hiện tại.

After: operation update đúng record, giữ nguyên số lượng records và verification pass.

## Estimated Time
35–50 phút.