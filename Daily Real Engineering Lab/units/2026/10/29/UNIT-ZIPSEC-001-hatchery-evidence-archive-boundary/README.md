# UNIT-ZIPSEC-001 — Hatchery Evidence ZIP Escapes Intake Directory

## Mục tiêu
Điều tra một sự cố bảo mật khi worker .NET giải nén ZIP chứng từ, thu thập bằng chứng, tìm nguyên nhân và sửa đúng ranh giới tin cậy.

## Bối cảnh thực tế
Trại giống thủy sản gửi ZIP chứng từ kiểm định. Worker chỉ được ghi vào thư mục intake riêng của lần nhập. Đội vận hành phát hiện một file mới xuất hiện ở thư mục lân cận sau khi xử lý ZIP, dù job báo thành công. Lab dùng thư mục tạm cục bộ, không dùng dữ liệu thật.

## Bạn cần làm gì
Chạy reproduce, ghi ít nhất ba giả thuyết, điều tra các đường dẫn quan sát được, sửa duy nhất starter/ArchiveIntake.cs, chạy verify và phân tích các giới hạn của giải pháp.

## Yêu cầu môi trường
.NET SDK 8.x và PowerShell 7+. Không cần Azure, Docker, database hoặc NuGet bên ngoài.

## Chạy nhanh
    pwsh ./setup.ps1
    pwsh ./reproduce.ps1

Sau khi sửa starter:
    pwsh ./verify.ps1

Có thể chạy trực tiếp:
    dotnet run --project checks/ArchiveChecks.csproj -- reproduce
    dotnet run --project checks/ArchiveChecks.csproj -- verify

## Cách reproduce vấn đề
Script tạo ZIP mô phỏng trong thư mục tạm và xác nhận file bất thường xuất hiện ngoài intake. Exit code 0 của reproduce nghĩa là **đã thấy triệu chứng**, không phải fix đã thành công.

## Những gì cần quan sát
- Đường dẫn tuyệt đối của intake và file bất thường.
- Tên entry của ZIP so với đường dẫn file thực tế.
- Một gói bị từ chối có để lại file hợp lệ đã trích xuất hay không.
- Tại sao các chỉ số CPU và retry trong evidence không đủ giải thích sự cố.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Giải pháp tham chiếu](solution/README.md). Chỉ đọc sau khi tự thử.

## Expected Results
[Trước khi sửa](expected-results/before.md) · [Sau khi sửa](expected-results/after.md)

## Estimated Time
60–75 phút (L3 / D4). Xem [scenario](docs/scenario.md), [evidence](evidence/incident.log) và [workspace](workspace/my-investigation.md).
