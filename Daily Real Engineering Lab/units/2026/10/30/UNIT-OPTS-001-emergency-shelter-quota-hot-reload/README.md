# UNIT-OPTS-001 — Giới hạn giường tại điểm trú ẩn không cập nhật khi vận hành

## Mục tiêu
Điều tra một thay đổi cấu hình runtime mà dashboard ghi nhận nhưng quyết định cấp giường của service không nhất quán; thu thập bằng chứng, đưa hypothesis, sửa và kiểm chứng contract.

## Bối cảnh thực tế
Trong đợt sơ tán do mưa lớn, một điểm trú ẩn giới hạn số giường được giữ trong một yêu cầu. Ban đầu giới hạn là 4. Điều phối viên giảm xuống 2 để tránh phân bổ quá mức, sau đó tăng lên 5. Hệ thống quản trị hiển thị cấu hình mới, nhưng một worker đang chạy lâu vẫn xử lý yêu cầu không như mong đợi. Không được bắt buộc restart mỗi khi thay đổi chính sách.

## Bạn cần làm gì
1. Chạy starter gốc và lưu các dòng check, configuredMax, expected, actual.
2. Đề xuất ít nhất ba giả thuyết: nguồn config, tín hiệu reload, hay lifecycle của service.
3. Kiểm tra cả trường hợp giảm và tăng quota; không chỉ trường hợp đầu tiên.
4. Sửa duy nhất starter/Program.cs, giữ đúng validation đầu vào.
5. Chạy verify.ps1 trên starter đã sửa và giải thích trade-off của live configuration.

## Yêu cầu môi trường
Windows PowerShell 5.1+ hoặc PowerShell 7, .NET 8 SDK. Không cần database, Docker, Azure, mạng ngoài hay NuGet bên thứ ba. Project dùng ASP.NET Core shared framework của .NET 8.

## Chạy nhanh
Từ thư mục unit:

```powershell
.\setup.ps1
.\reproduce.ps1
.\run.ps1
```

## Cách reproduce vấn đề
Chạy `reproduce.ps1` **trước khi sửa**; script chỉ thành công khi starter tái hiện được mismatch dự kiến. `run.ps1` hiển thị chi tiết từng case và có thể trả exit code 23 trên starter gốc.

## Những gì cần quan sát
- Cùng một instance service phục vụ cả trước và sau hai lần thay đổi chính sách.
- So sánh `configuredMax` và `actual` ở mỗi case.
- Những case nào sai sau khi cấu hình thay đổi? Case nào vẫn đúng?
- Bằng chứng nào chứng minh cấu hình đã reload, và bằng chứng nào cho thấy quyết định vẫn cũ?

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng vào workspace/my-investigation.md.
3. Thử fix trong starter/.
4. Verify bằng verify.ps1 trên code đã chỉnh.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md).

## Reference Solution
**Spoiler — chỉ đọc sau khi tự thử:** [solution/README.md](solution/README.md).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md).

## Estimated Time
45 phút · L2 / D4. Validation mode: static-review; source chưa được thực thi trong quá trình tạo lab.
