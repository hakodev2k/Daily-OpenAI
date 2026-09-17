# UNIT-BLOB-001 — Upload retry nhưng blob thứ hai bị rỗng

## Mục tiêu
Điều tra một lỗi upload attachment chỉ xuất hiện khi request đầu tiên thất bại tạm thời và hệ thống thực hiện retry.

## Bối cảnh thực tế
Một document service nhận file từ upstream rồi lưu vào Azure Blob Storage. Bình thường upload thành công. Khi storage client mô phỏng lỗi transient ở lần đầu, retry báo thành công nhưng nội dung blob cuối cùng có kích thước `0` byte. Log không có exception ở lần retry.

## Bạn cần làm gì
1. Chạy starter và reproduce lỗi.
2. Ghi ít nhất hai hypothesis vào `workspace/my-investigation.md`.
3. Quan sát trạng thái nguồn dữ liệu trước mỗi upload attempt.
4. Sửa code trong `starter/` mà không thay đổi fake storage client.
5. Chạy `verify.ps1` để xác nhận retry vẫn lưu đúng payload.
6. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- .NET 8 SDK
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy chương trình với một storage client deterministic: attempt đầu thất bại sau khi đọc request body, attempt tiếp theo được phép thành công.

## Những gì cần quan sát
- Số attempt.
- Số byte storage client đọc được ở mỗi attempt.
- Payload cuối cùng được lưu.
- Trạng thái của nguồn dữ liệu giữa các attempt.

## Quy tắc làm lab
1. Reproduce trước.
2. Thu thập evidence.
3. Ghi hypothesis.
4. Sửa starter.
5. Verify.
6. Chỉ sau đó mới xem solution.

## Hints
- `hints/hint-01.md`
- `hints/hint-02.md`
- `hints/hint-03.md`

## Reference Solution
`solution/README.md`

## Expected Results
**Before:** attempt đầu đọc đủ payload rồi thất bại; attempt sau báo thành công nhưng blob không còn payload mong đợi.

**After:** transient failure vẫn được retry và blob cuối cùng chứa đúng toàn bộ payload ban đầu.

## Estimated Time
35–50 phút.