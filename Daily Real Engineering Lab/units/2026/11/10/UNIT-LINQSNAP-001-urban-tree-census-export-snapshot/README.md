# UNIT-LINQSNAP-001 — Báo cáo kiểm kê cây xanh thay đổi giữa hai lần đọc

## Mục tiêu
Tự reproduce một sai lệch tính nhất quán trong pipeline C# LINQ, thu thập evidence và xác định ranh giới dữ liệu phù hợp cho báo cáo.

## Bối cảnh thực tế
Đội cây xanh đô thị tổng hợp dữ liệu khảo sát thành bản tóm tắt và danh sách ID gửi kiểm toán. Trong lúc dựng báo cáo, một bản ghi mới được đồng bộ từ thiết bị hiện trường. Báo cáo đôi khi ghi tổng số không khớp số dòng xuất ra dù mỗi bước đều chạy thành công.

## Bạn cần làm gì
Chạy chương trình ở chế độ `reproduce`, ghi lại hai lần đọc; đưa ra ít nhất hai giả thuyết; chỉnh `starter/Program.cs` để một báo cáo có cùng tập bản ghi cho cả tổng và danh sách, nhưng báo cáo mới ở lần chạy tiếp theo vẫn phản ánh dữ liệu mới.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET SDK 8.x; không cần database, Docker hoặc NuGet bên ngoài.

## Chạy nhanh
Từ thư mục unit: `./scripts/setup.ps1`, `./scripts/run.ps1`.

## Cách reproduce vấn đề
Trước khi sửa code, chạy `./scripts/reproduce.ps1`. Script phải xác nhận triệu chứng xuất hiện và trả exit code 0 nếu reproduce thành công. Chạy `./scripts/verify.ps1` để thấy hợp đồng nhất quán hiện chưa đạt.

## Những gì cần quan sát
Ghi lại `Count`, danh sách `Ids`, số bản ghi nguồn ở đầu/cuối quy trình và vị trí thay đổi dữ liệu. Không suy luận root cause chỉ từ tên phương thức.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis vào [workspace](workspace/my-investigation.md).
3. Thử fix trên `starter/Program.cs`.
4. Verify bằng script.
5. Chỉ sau đó xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler:** [Reference Solution](solution/README.md) chỉ đọc sau khi tự thử.

## Expected Results
Trước: cùng báo cáo có `Count != Ids.Length`. Sau: hai trường nhất quán và báo cáo lần sau vẫn nhận dữ liệu cập nhật.

## Estimated Time
40 phút.
