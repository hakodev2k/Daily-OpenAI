# UNIT-SEO-001 — Product pages bị index thành nhiều URL tương đương

## Mục tiêu
Điều tra một SEO regression trong hệ thống commerce nơi cùng một nội dung sản phẩm có thể được truy cập qua nhiều URL có query parameters khác nhau.

## Bối cảnh thực tế
Sau khi team marketing thêm tracking parameters và bộ lọc UI, Search Console bắt đầu báo nhiều URL gần như trùng nội dung. Người dùng vẫn truy cập website bình thường và response đều `200`, nhưng crawler nhận các tín hiệu URL không nhất quán.

## Bạn cần làm gì
1. Chạy starter để quan sát metadata được tạo cho các request URL mẫu.
2. Ghi lại các URL đại diện cho cùng một resource và các URL thực sự thay đổi nội dung.
3. Xác định invariant mà canonical URL cần bảo toàn.
4. Sửa learner-editable code trong `starter/`.
5. Chạy `verify.ps1` để kiểm tra behavior và regression cases.
6. Chỉ sau khi tự thử mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy các URL mẫu gồm tracking parameters, sorting và variant selector rồi in canonical metadata mà starter sinh ra.

## Những gì cần quan sát
- Hai URL nào đang biểu diễn cùng một nội dung chính.
- Query parameter nào chỉ phục vụ tracking/presentation.
- Query parameter nào có thể đại diện cho resource khác.
- Canonical output có ổn định khi thứ tự query thay đổi hay không.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Sửa starter.
4. Verify.
5. So sánh solution.

## Hints
- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution
[Reference Solution — chỉ xem sau khi đã reproduce và tự thử](solution/README.md)

## Expected Results
**Before:** các URL tương đương có thể sinh canonical khác nhau hoặc canonical giữ lại dimensions không đại diện cho identity của resource.

**After:** canonical URL ổn định cho cùng resource, trong khi dimensions thực sự định danh nội dung không bị gộp nhầm.

## Troubleshooting
Nếu script không chạy, kiểm tra `dotnet --version` và bảo đảm .NET 8 SDK có sẵn.

## Estimated Time
30–45 phút.