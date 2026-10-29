# UNIT-LEASE-001 — Preview ảnh đường bộ thay đổi sau khi tạo

## Mục tiêu
Thực hành phân tích ranh giới dữ liệu giữa một service tạo preview và consumer xử lý bất đồng bộ; refactor an toàn mà vẫn giữ hợp đồng API và có regression checks.

## Bối cảnh thực tế
Nền tảng tiếp nhận ảnh hư hỏng mặt đường chuẩn bị các tile preview cho hàng đợi kiểm duyệt. Một preview được ghi nhận đúng lúc tạo, nhưng khi công việc kế tiếp chạy thì payload của yêu cầu trước đôi lúc khác. Log đầu vào và số lượng job đều bình thường. Sự cố ảnh hưởng đối chiếu ảnh với ticket hiện trường.

## Bạn cần làm gì
Chạy fixture, ghi lại ba thời điểm quan sát, đề xuất ít nhất hai hypothesis (nguồn đầu vào, xử lý buffer, consumer), sửa duy nhất starter/PreviewService.cs để dữ liệu của job đã tạo không thay đổi bởi các job khác. Giữ nguyên chữ ký public và không sửa test harness.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7 hoặc Windows PowerShell. Không cần cloud, Docker, database hay NuGet ngoài SDK. SDK được cấu hình trong global.json.

## Chạy nhanh
~~~powershell
./setup.ps1
./run.ps1
./reproduce.ps1
./verify.ps1
~~~
Nếu chưa bật execution policy cho script, chạy các lệnh dotnet tương đương: dotnet run --project ./starter/PreviewLeaseLab.csproj -- reproduce và -- verify, từ thư mục unit.

## Cách reproduce vấn đề
Trên starter chưa chỉnh sửa, chạy reproduce.ps1. Exit code 0 có nghĩa triệu chứng đã được tái hiện; exit code khác 0 nghĩa fixture không tái hiện được. Sau khi sửa, verify.ps1 phải trả exit code 0; verify chạy trên starter do bạn chỉnh, không chạy trên solution.

## Những gì cần quan sát
So sánh payload của job A ngay sau khi tạo và sau khi tạo job B. Ghi nhận dữ liệu B, sự ổn định của A qua nhiều request, và tính độc lập với mảng đầu vào. Không dùng thời gian chạy làm bằng chứng chính.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis và bằng chứng.
3. Thử fix trong starter.
4. Verify, kiểm tra regression.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler — chỉ mở sau khi tự thử:** [Phân tích và trade-offs](solution/README.md) · [Code tham chiếu](solution/PreviewService.cs).

## Expected Results
[Before](expected-results/before.md) · [After](expected-results/after.md) · [Investigation workspace](workspace/my-investigation.md)

## Estimated Time
60–75 phút · L4 / D4. Đây là fixture deterministic để khảo sát một contract về bộ nhớ; chưa chạy .NET trong phiên tạo lab.
