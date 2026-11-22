# UNIT-EFTRACK-001 — Library Hold: trạng thái giữ sách bị lệch sau cập nhật hàng loạt

## Mục tiêu
Điều tra tính nhất quán giữa thao tác cập nhật dữ liệu hàng loạt và các đối tượng EF Core đang được theo dõi; đưa ra regression test kiểm chứng cùng một request.

## Bối cảnh thực tế
Hệ thống thư viện duyệt danh sách yêu cầu giữ sách. Một tác vụ xử lý hàng loạt đổi trạng thái trong database, nhưng phản hồi của màn hình kiểm duyệt vẫn hiển thị trạng thái cũ dù database đã đổi.

## Bạn cần làm gì
1. Chạy ứng dụng mẫu, lưu log trước và sau thao tác.
2. Viết ít nhất hai giả thuyết về sự khác biệt giữa trạng thái database và phản hồi.
3. Sửa `starter/Program.cs` sao cho phản hồi cuối cùng phản ánh dữ liệu đã lưu.
4. Đảm bảo không tăng số lần query vô hạn và không phá vỡ logic khi không có bản ghi phù hợp.

## Yêu cầu môi trường
.NET SDK 8; PowerShell 7 hoặc Windows PowerShell; kết nối NuGet khi restore lần đầu. Không cần SQL Server: SQLite in-memory dùng chung một connection.

## Chạy nhanh
```powershell
./scripts/setup.ps1
./scripts/run.ps1
```

## Cách reproduce vấn đề
```powershell
./scripts/reproduce.ps1
```
Quan sát dòng `DATABASE` và `RESPONSE` trong output. Script xác nhận trạng thái sai lệch ở starter ban đầu.

## Những gì cần quan sát
Ghi lại giá trị `tracked`, `database`, số bản ghi bị ảnh hưởng, thứ tự query và trạng thái của đối tượng sau mỗi bước. Phân biệt lỗi ghi dữ liệu với lỗi đọc phản hồi.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp và trade-offs](solution/README.md)

## Expected Results
Starter: `DATABASE=Approved; RESPONSE=Pending`. Sau sửa: `DATABASE=Approved; RESPONSE=Approved`, kiểm tra tự động PASS.

## Estimated Time
40 phút.
