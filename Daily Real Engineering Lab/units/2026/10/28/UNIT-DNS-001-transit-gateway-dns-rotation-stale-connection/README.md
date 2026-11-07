# UNIT-DNS-001 — Gateway xe buýt vẫn gọi datacenter cũ sau khi đổi DNS

## Mục tiêu
Điều tra hành vi HTTP client khi endpoint phía sau tên miền thay đổi nhưng traffic vẫn liên tục. Thu thập bằng chứng để phân biệt routing, HTTP response cache và connection lifecycle.

## Bối cảnh thực tế
Dịch vụ dự báo giờ đến xe buýt sử dụng một upstream gateway. Trong đợt chuyển lưu lượng EAST → WEST, dashboard routing báo đã chuyển thành công, nhưng ứng dụng .NET vẫn nhận dữ liệu từ EAST. EAST chưa ngừng phục vụ nên lỗi không xuất hiện dưới dạng HTTP 500.

## Bạn cần làm gì
Chạy fixture, ghi lại kết quả ba request và số lần mở kết nối. Đưa ra ít nhất ba giả thuyết; xác định điểm cần can thiệp, sửa starter để sau cutover nhận WEST, và đánh giá rủi ro khi chỉnh chính sách connection.

## Yêu cầu môi trường
.NET SDK 8.x; Windows PowerShell 5.1+ hoặc PowerShell 7. Không cần DNS thực, cloud, Docker, NuGet hay quyền admin. Fixture dùng hai TCP server trên loopback, route snapshot thay cho DNS để tái hiện đúng ranh giới giữa route lookup và pooled connection.

## Chạy nhanh
Từ thư mục unit:
`dotnet run --project starter/TransitGatewayLab.csproj -- --inspect`

PowerShell: `./run.ps1`

## Cách reproduce vấn đề
Trước khi sửa starter:
`./reproduce.ps1` hoặc `dotnet run --project starter/TransitGatewayLab.csproj -- --reproduce`.

## Những gì cần quan sát
So sánh Before cutover, After cutover, Next request và ConnectionAttempts. Lưu ý server EAST vẫn trả 200; không có lỗi mạng. Hãy phân biệt số request HTTP với số kết nối TCP thực sự được tạo.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace](workspace/my-investigation.md).
3. Thử fix trong `starter/`.
4. Verify bằng `./verify.ps1` (chạy starter đã sửa, không chạy solution).
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 01](hints/hint-01.md) · [Hint 02](hints/hint-02.md) · [Hint 03](hints/hint-03.md)

## Reference Solution
**Spoiler:** [Lời giải và trade-offs](solution/README.md), [mã tham khảo](solution/Program.cs). Chỉ xem sau khi tự điều tra.

## Expected Results
[Trước](expected-results/before.md) · [Sau](expected-results/after.md). Nếu không reproduce được, chạy lại từ process mới, không chạy đồng thời hai phiên; kiểm tra .NET SDK và local firewall.

## Estimated Time
55 phút (10 reproduce, 15 hypotheses/evidence, 20 fix/verify, 10 compare).
