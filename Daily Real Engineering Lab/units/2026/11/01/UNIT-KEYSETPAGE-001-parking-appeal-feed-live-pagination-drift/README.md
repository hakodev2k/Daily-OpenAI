# UNIT-KEYSETPAGE-001 — Hồ sơ khiếu nại bãi đỗ xe bị lặp và bỏ sót khi tải trang

## Mục tiêu
Điều tra một incident mà danh sách hồ sơ trên màn hình review trả về các trang riêng lẻ có vẻ hợp lệ nhưng một phiên duyệt nhiều trang lại có hồ sơ lặp hoặc bị bỏ sót. Tái hiện, thu thập bằng chứng, sửa contract phân trang và kiểm chứng các thay đổi đồng thời.

## Bối cảnh thực tế
Cổng xử lý khiếu nại đỗ xe của thành phố có màn hình **Hồ sơ mới nhất**. Nhân viên tải mỗi lần ba hồ sơ, trong khi công dân vẫn có thể tạo hồ sơ mới và quản trị viên có thể đóng hồ sơ cũ. Dashboard báo không có lỗi HTTP, nhưng audit cuối ca cho thấy các hồ sơ không được review đầy đủ.

## Bạn cần làm gì
1. Chạy starter, tái hiện phiên duyệt bị sai và ghi ID của hai trang.
2. Đọc [timeline incident](evidence/incident-timeline.md), nêu ít nhất ba hypothesis và bằng chứng phản biện.
3. Chỉnh sửa duy nhất `starter/Program.cs` để các trang không lặp/bỏ sót các hồ sơ đã tồn tại khi phiên duyệt bắt đầu, ngay cả khi feed có insert/delete trước vị trí đang duyệt.
4. Giữ API công khai `AppealFeed.Fetch`, `Appeal`, `PageCursor`, `PageResult`; không chỉnh `checks/`.
5. Chạy verify; ghi lại trade-off về snapshot nhất quán và thay đổi khóa sắp xếp.

## Yêu cầu môi trường
.NET 8 SDK và PowerShell 7+ (Windows PowerShell 5.1 cũng phù hợp). Không cần database, Docker, Azure hoặc NuGet ngoài SDK. Đây là local contract simulator của một API có SQL-backed feed.

## Chạy nhanh
Tại thư mục unit:
```powershell
./scripts/setup.ps1
./scripts/run.ps1
./scripts/reproduce.ps1
./scripts/verify.ps1
```
Nếu PowerShell chặn script, chạy các lệnh `dotnet` bên trong script ở terminal tin cậy; không cần thay đổi execution policy toàn hệ thống.

## Cách reproduce vấn đề
Chạy `./scripts/reproduce.ps1` trên starter chưa sửa. Exit code 0 nghĩa là **đã tái hiện triệu chứng lỗi**, không có nghĩa là hệ thống đúng. `./scripts/verify.ps1` chạy bộ contract riêng trên mã learner chỉnh sửa; trước khi sửa phải FAIL.

## Những gì cần quan sát
- Trang đầu có ba hồ sơ đúng thứ tự.
- Một hồ sơ mới xuất hiện giữa hai lần gọi API.
- Trang kế tiếp có thể lặp một ID đã thấy, khiến một hồ sơ cũ chưa xuất hiện trong hai trang.
- CPU, HTTP status và số bản ghi trả về đều bình thường; kết quả tổng thể của phiên duyệt mới là điều cần kiểm chứng.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./scripts/verify.ps1`.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ xem sau khi tự điều tra và sửa:** [Reference Solution](solution/README.md).

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md).

## Estimated Time
60–75 phút (L3 Production Incident / D4).
