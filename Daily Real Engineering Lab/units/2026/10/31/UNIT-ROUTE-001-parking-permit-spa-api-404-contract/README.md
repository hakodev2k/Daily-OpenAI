# UNIT-ROUTE-001 — API trả HTML 200 khi route không tồn tại

## Mục tiêu
Tự tái hiện sai lệch giữa HTTP status, Content-Type và hợp đồng JSON khi SPA cùng chạy với ASP.NET Core API. Phân biệt route hợp lệ, route API không tồn tại và điều hướng SPA.

## Bối cảnh thực tế
Cổng gia hạn giấy phép đỗ xe của thành phố phục vụ giao diện web và API trên cùng host. Một bản cập nhật frontend gọi nhầm endpoint; monitoring ghi nhận HTTP 200 nhưng ứng dụng lại báo lỗi đọc JSON. Người dùng vẫn vào trang quản lý giấy phép bình thường.

## Bạn cần làm gì
1. Chạy ứng dụng mô phỏng và ghi lại status, Content-Type, body cho từng URL.
2. Đưa ra ít nhất hai giả thuyết; kiểm tra sự khác biệt giữa đường dẫn API và SPA.
3. Chỉnh sửa **chỉ** `starter/PermitRoutes.cs`; giữ nguyên API đã tồn tại và điều hướng SPA.
4. Chạy kiểm tra hồi quy trên chính starter đã sửa; sau đó mới so sánh reference solution.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8.
- Không cần Docker, database, Azure hoặc NuGet bên thứ ba.

## Chạy nhanh
```powershell
.\scripts\setup.ps1
.\scripts\run.ps1
.\scripts\reproduce.ps1
```

## Cách reproduce vấn đề
`reproduce.ps1` chạy contract trên **starter gốc**, trả mã 0 khi xác nhận được đúng nhóm triệu chứng cần điều tra (các đường dẫn hợp lệ vẫn hoạt động). Sau khi sửa `starter/PermitRoutes.cs`, chạy `verify.ps1`. Script verify không kiểm tra thay cho mã của bạn bằng reference solution.

## Những gì cần quan sát
- Endpoint API hợp lệ trả status, media type và JSON ra sao?
- Một đường dẫn API sai khác gì một đường dẫn SPA?
- Client nhìn thấy gì khi route API không tồn tại?
- Một URL trông như static asset được xử lý như thế nào?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
- [Hint 01](hints/hint-01.md)
- [Hint 02](hints/hint-02.md)
- [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** Chỉ mở [Reference Solution](solution/README.md) sau khi đã thử sửa.

## Expected Results
- [Trước khi sửa](expected-results/before.md)
- [Sau khi sửa](expected-results/after.md)

## Estimated Time
35–45 phút (L1 / D3). Ghi chứng cứ trong [workspace](workspace/my-investigation.md).
