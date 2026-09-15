# UNIT-DI-001 — Keyed service selection drifts from tenant policy

## Mục tiêu
Điều tra một lỗi dependency-injection selection trong ASP.NET Core khi cùng một interface có nhiều implementation và lựa chọn phụ thuộc tenant.

## Bối cảnh thực tế
Một notification API hỗ trợ nhiều tenant. Mỗi tenant được cấu hình dùng một delivery provider khác nhau. Hệ thống chạy bình thường với tenant mặc định, nhưng một tenant mới đôi khi nhận kết quả từ provider không đúng policy dù request vẫn trả `200`.

## Bạn cần làm gì
1. Chạy starter và reproduce mismatch.
2. Ghi hypothesis trước khi sửa.
3. Xác định boundary chịu trách nhiệm chọn implementation.
4. Sửa code trong `starter/` mà không hard-code tenant vào controller.
5. Chạy `verify.ps1`.
6. Sau đó mới đọc reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh
```powershell
./reproduce.ps1
```

## Cách reproduce vấn đề
Script chạy executable với hai tenant có policy khác nhau và kiểm tra output provider.

## Những gì cần quan sát
- Tenant nào nhận provider không đúng expectation?
- Registration có đủ implementation cần thiết không?
- Selection xảy ra ở đâu trong request flow?
- Việc thêm implementation mới có làm thay đổi hành vi cũ không?

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
[Reference Solution](solution/README.md) — spoiler, chỉ xem sau khi đã thử.

## Expected Results
**Before:** ít nhất một tenant được xử lý bởi provider khác với policy đã khai báo.

**After:** mỗi tenant nhận đúng provider; tenant không có policy rõ ràng bị từ chối thay vì âm thầm dùng một implementation ngẫu nhiên/mặc định.

## Estimated Time
35–50 phút.