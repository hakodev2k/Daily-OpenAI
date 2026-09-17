# UNIT-FE-001 — UI báo thành công trước khi thao tác bất đồng bộ hoàn tất

## Mục tiêu
Điều tra control flow của Promise chain trong JavaScript khi một bước bất đồng bộ không được nối đúng vào completion boundary của thao tác UI.

## Bối cảnh thực tế
Một màn hình quản trị gửi thay đổi rồi hiển thị trạng thái thành công. Trong failure simulation, UI đôi khi báo thành công trước, sau đó mới xuất hiện lỗi từ bước xử lý tiếp theo.

## Bạn cần làm gì
1. Chạy `node starter/app.js` và reproduce thứ tự event.
2. Ghi hypothesis trước khi sửa.
3. Sửa `starter/app.js` để completion state phản ánh toàn bộ operation.
4. Chạy `./verify.ps1`.
5. Sau đó mới xem reference solution.

## Yêu cầu môi trường
- Node.js 22+
- PowerShell

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy deterministic failure path và kiểm tra event order.

## Những gì cần quan sát
- Thứ tự `SAVE`, `FOLLOW_UP`, `SUCCESS`, `ERROR`.
- Promise nào đại diện cho toàn bộ user operation.
- Error boundary hiện tại có bao phủ bước xử lý cuối hay không.

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
[Reference Solution — chỉ xem sau khi đã thử](solution/README.md)

## Expected Results
Before: success event có thể xuất hiện trước failure của operation.

After: operation chỉ success khi toàn bộ chain hoàn tất; failure được xử lý tại đúng boundary.

## Estimated Time
25–40 phút.