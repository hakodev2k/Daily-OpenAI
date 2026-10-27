# UNIT-CFG-001 — Airport Baggage SLA: cấu hình triển khai không khớp hành vi

## Mục tiêu

Điều tra sự sai lệch giữa deployment configuration và giá trị chính sách thực sự được một .NET worker sử dụng. Thu thập bằng chứng, loại trừ các giả thuyết và thiết kế regression checks cho cả override lẫn fallback.

## Bối cảnh thực tế

Dịch vụ giám sát hành lý nối chuyến của sân bay chạy bình thường trong môi trường test. Trong đợt vận hành cao điểm, đội vận hành yêu cầu cảnh báo sau 5 phút và chuyển vào queue priority, nhưng dashboard vẫn báo ngưỡng 30 phút và queue standard. Deployment báo đã áp dụng cấu hình thành công; không có exception.

## Bạn cần làm gì

1. Chạy starter và reproduce sự sai lệch.
2. Ghi ít nhất ba giả thuyết, evidence tương ứng và phép thử để loại trừ.
3. Chỉnh sửa code trong thư mục starter/ để đạt contract: giá trị môi trường được áp dụng khi có, còn thiếu override thì dùng giá trị mặc định.
4. Chạy verify.ps1 trên chính starter đã sửa; kiểm tra thêm tình huống override thay đổi.
5. Sau khi hoàn thành, đối chiếu solution/README.md.

## Yêu cầu môi trường

- .NET SDK 8.x và PowerShell 7+ (hoặc Windows PowerShell 5.1).
- Không cần Docker, Azure, tài khoản cloud hay NuGet bên ngoài; sử dụng ASP.NET Core shared framework có sẵn trong SDK.
- Các giá trị là synthetic, không dùng credentials thật.

## Chạy nhanh

~~~powershell
cd "Daily Real Engineering Lab/units/2026/10/27/UNIT-CFG-001-airport-baggage-sla-override-drift"
./run.ps1
./reproduce.ps1
~~~

Trên Windows PowerShell có thể dùng: .\reproduce.ps1. Nếu execution policy chặn script, mở một PowerShell process theo policy của tổ chức hoặc chạy từng lệnh được mô tả trong script; không thay đổi policy toàn máy.

## Cách reproduce vấn đề

Chạy ./reproduce.ps1 từ thư mục lab. Script tự đặt hai biến môi trường cho process con, build starter, chạy contract assertion và xác nhận lỗi có chủ đích. Script hoàn nguyên biến môi trường khi kết thúc.

## Những gì cần quan sát

- EXPECTED_ALERT_MINUTES=5, EFFECTIVE_ALERT_MINUTES=30.
- EXPECTED_ESCALATION_QUEUE=priority, EFFECTIVE_ESCALATION_QUEUE=standard.
- CONTRACT_FAIL, exit code 2 trong bước reproduce.
- Không có lỗi parse JSON hay kết nối mạng; hãy kiểm tra evidence và thứ tự các bước khởi tạo hệ thống trước khi kết luận.

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

**Spoiler:** Chỉ xem sau khi đã tự điều tra và chạy verify. [Reference Solution](solution/README.md).

## Expected Results

Trước khi sửa, contract assertion thất bại trong kịch bản deployment override. Sau khi sửa, verify.ps1 phải pass cho hai giá trị override khác nhau và cho tình huống không có override. Chi tiết: [before](expected-results/before.md) và [after](expected-results/after.md).

## Estimated Time

45–60 phút. Maturity L2, difficulty D4. [Investigation workspace](workspace/my-investigation.md).
