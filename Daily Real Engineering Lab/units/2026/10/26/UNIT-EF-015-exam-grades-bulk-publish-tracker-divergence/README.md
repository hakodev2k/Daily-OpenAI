# UNIT-EF-015 — Điểm thi đã công bố bất ngờ quay về Draft

## Mục tiêu
Điều tra một sai lệch dữ liệu sau hai bước cập nhật trong cùng request scope; đưa ra fix nhỏ nhất, kiểm chứng trạng thái persisted và giải thích trade-offs.

## Bối cảnh thực tế
Một hệ thống cấp chứng chỉ nghề công bố điểm thi theo batch, sau đó bộ phận QA bổ sung ghi chú cho một hồ sơ. Log batch báo một điểm đã công bố, nhưng trang tra cứu cuối request vẫn hiển thị Draft. Không có exception.

## Bạn cần làm gì
1. Reproduce bằng lệnh bên dưới; ghi lại trạng thái lưu thực tế.
2. Nêu ít nhất hai giả thuyết và cách phân biệt chúng.
3. Sửa logic trong **starter/GradesService.cs** (không sửa harness).
4. Chạy verify để bảo đảm trạng thái công bố, điểm số và ghi chú cùng đúng.
5. So sánh với reference solution sau khi hoàn thành.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell; NuGet để restore Microsoft.EntityFrameworkCore.Sqlite 8.0.20. Không cần SQL Server, Docker hay Azure.

## Chạy nhanh
~~~powershell
./setup.ps1
./reproduce.ps1
~~~

## Cách reproduce vấn đề
Chạy **./reproduce.ps1** trên starter chưa sửa. Script phải kết thúc với exit code 0 khi tái hiện được symptom. Sau khi sửa, chạy **./verify.ps1**; trước khi sửa verify phải thất bại.

## Những gì cần quan sát
So sánh log số hàng được batch xử lý với giá trị **Status**, **ReviewerNote**, **Score** đọc lại từ database. Ghi bằng chứng về thứ tự các thao tác và state của entity tại các mốc quan trọng; không suy đoán từ thông báo thành công của batch.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix trong starter.
4. Verify correctness và regression.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** chỉ đọc sau khi tự điều tra: [Reference Solution](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md). Lệnh verify phải trả exit code 0 sau fix.

## Estimated Time
65 phút. Ghi chép tại [workspace/my-investigation.md](workspace/my-investigation.md).
