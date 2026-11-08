# UNIT-FWDTRUST-001 — Trail permit admin admission

## Mục tiêu
Điều tra ranh giới tin cậy giữa reverse proxy và ứng dụng ASP.NET Core khi kiểm tra nguồn truy cập quản trị.

## Bối cảnh thực tế
Cổng cấp phép đường mòn chỉ cho phép văn phòng nội bộ vào endpoint quản trị. Một yêu cầu từ môi trường thử nghiệm bên ngoài bất ngờ được chấp nhận, dù cấu hình mạng không thay đổi.

## Bạn cần làm gì
Reproduce, ghi lại peer IP, request headers và response; lập giả thuyết; đề xuất chính sách xác thực nguồn có thể kiểm chứng khi chạy sau reverse proxy.

## Yêu cầu môi trường
.NET SDK 8.x, PowerShell 7+, local only.

## Chạy nhanh
Terminal 1: `dotnet run --project starter/TrailPermit.csproj`. Terminal 2: `pwsh ./scripts/reproduce.ps1`.

## Cách reproduce vấn đề
Gửi cùng request có và không có thông tin nguồn do client cung cấp. Xem `/diagnostics/peer` để đối chiếu.

## Những gì cần quan sát
Status 403/200, IP kết nối thực và giá trị header. Tách quan sát thực tế khỏi giả định về cấu hình proxy.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify theo policy tin cậy và negative tests.
5. Chỉ sau đó xem solution.

## Hints
[1](hints/hint-01.md) · [2](hints/hint-02.md) · [3](hints/hint-03.md)

## Reference Solution
**Spoiler:** [solution](solution/README.md)

## Expected Results
Starter cho thấy header từ client có thể thay đổi quyết định; thiết kế hoàn chỉnh phải từ chối header không đến từ trusted proxy.

## Estimated Time
55 phút.
