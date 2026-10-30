# UNIT-LPROBE-001 — Museum ticket kiosk: điều tra vòng lặp restart khi đối tác gián đoạn

## Mục tiêu

Điều tra một incident ASP.NET Core khi hệ thống bên ngoài gián đoạn; phân biệt sức khỏe process với khả năng nhận traffic. Thu thập bằng chứng, đặt giả thuyết, sửa cấu hình và chứng minh không làm mất tín hiệu cảnh báo.

## Bối cảnh thực tế

Dịch vụ xác thực vé bảo tàng gọi API đối tác để đánh giá rủi ro. Khi đối tác gián đoạn, pod Kubernetes liên tục restart dù process ASP.NET Core vẫn phản hồi HTTP. Đội vận hành cần giữ chức năng kiểm soát traffic an toàn và tránh restart không cần thiết.

## Bạn cần làm gì

1. Chạy mô phỏng và ghi status của hai probe tại ba pha.
2. Đề xuất ít nhất ba giả thuyết.
3. Kiểm tra health checks và giải thích hợp đồng của từng probe.
4. Chỉ sửa mã trong thư mục starter/; không sửa contracts/.
5. Chạy kiểm tra độc lập rồi mới mở lời giải.

## Yêu cầu môi trường

- .NET SDK 8 trở lên (TargetFramework net8.0, không có NuGet ngoài).
- PowerShell 5.1+ hoặc PowerShell 7; không cần Docker, Kubernetes, cloud.
- Quyền bind cổng loopback ngẫu nhiên trên 127.0.0.1.

## Chạy nhanh

Tại thư mục unit:

~~~powershell
dotnet --version
./reproduce.ps1
~~~

Hoặc:

~~~powershell
dotnet run --project ./contracts/ProbeContract.csproj -c Release -- --reproduce
~~~

## Cách reproduce vấn đề

reproduce.ps1 khởi chạy ASP.NET Core thực trên loopback với đối tác giả lập. Contract kiểm tra ba pha normal, partner-down, recovered. Ghi lại status và hypothesis trước khi sửa mã.

## Những gì cần quan sát

- Đối tác bình thường: cả hai probe HTTP 200.
- Đối tác lỗi: starter trả HTTP 503 cho cả hai probe, nhưng process vẫn phản hồi.
- Đối tác hồi phục: cả hai trở lại HTTP 200.
- Fixture không chạy Kubernetes; restart loop là hệ quả vận hành cần đánh giá từ cấu hình probe.

Xem [incident timeline](evidence/incident.md) và ghi [investigation](workspace/my-investigation.md).

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify bằng ./verify.ps1 (chạy starter đã sửa, không chạy solution).
5. Chỉ sau đó mới xem solution.

## Hints

[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution

**Spoiler:** chỉ xem [solution](solution/README.md) sau khi tự điều tra và chạy verify.

## Expected Results

Trước sửa: normal live=200 ready=200; partner-down live=503 ready=503; recovered live=200 ready=200. Reproduce thoát 0 khi tái hiện đúng.

Sau sửa: normal live=200 ready=200; partner-down live=200 ready=503; recovered live=200 ready=200. Verify thoát 0 khi toàn bộ hợp đồng được thỏa mãn. Không sửa contract để làm xanh kết quả.

Nếu lỗi bind port, kiểm tra chính sách loopback/firewall; nếu thiếu SDK, cài SDK tương thích. Không so sánh timing tuyệt đối.

## Estimated Time

60 phút. [Tài liệu tham khảo](docs/references.md) dành cho sau lab.
