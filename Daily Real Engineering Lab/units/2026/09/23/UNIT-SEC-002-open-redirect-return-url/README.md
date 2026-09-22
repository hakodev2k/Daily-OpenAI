# UNIT-SEC-002 — Return URL Trust Boundary

## Mục tiêu
Điều tra một luồng đăng nhập ASP.NET Core có hành vi điều hướng bất thường sau khi authentication thành công, thu thập evidence, xác định trust boundary bị vi phạm và sửa mà không làm hỏng navigation hợp lệ trong ứng dụng.

## Bối cảnh thực tế
Employee Self-Service Portal cho phép người dùng mở một trang nội bộ, bị chuyển tới `/login`, rồi quay lại trang ban đầu sau khi đăng nhập. QA phát hiện một số request có thể khiến trình duyệt rời portal ngay sau khi đăng nhập thành công. Authentication vẫn đúng và không có credential bị bỏ qua.

## Bạn cần làm gì
1. Chạy starter và reproduce symptom.
2. Ghi lại request, response status và `Location` header.
3. Đưa ra ít nhất 2 hypothesis trước khi sửa code.
4. Sửa code trong `starter/` để giữ được return navigation hợp lệ nhưng không cho request input đưa user ra ngoài trust boundary của portal.
5. Chạy `verify.ps1`.
6. Sau đó mới đọc reference solution.

## Yêu cầu môi trường
- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell

## Chạy nhanh
```powershell
cd "Daily Real Engineering Lab/units/2026/09/23/UNIT-SEC-002-open-redirect-return-url"
./run.ps1
```

## Cách reproduce vấn đề
Ở terminal khác:
```powershell
./reproduce.ps1
```

Script sẽ kiểm tra cả một destination nội bộ và một destination nằm ngoài portal. Hãy tập trung vào response `Location` sau POST login.

## Những gì cần quan sát
- Authentication request có thành công không?
- Hai input điều hướng tạo `Location` header khác nhau như thế nào?
- Destination nào thuộc navigation contract của portal?
- Dữ liệu nào đang đi từ request input tới response navigation?

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
[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results
Trước khi sửa, reproduction phải chứng minh có input khiến response điều hướng ra ngoài portal. Sau khi sửa, navigation nội bộ hợp lệ vẫn hoạt động và external destination bị thay bằng fallback an toàn.

## Estimated Time
Khoảng 50 phút.