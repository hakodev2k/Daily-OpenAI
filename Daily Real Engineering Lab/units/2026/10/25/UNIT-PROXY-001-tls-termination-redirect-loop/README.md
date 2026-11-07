# UNIT-PROXY-001 — HTTPS Redirect Loop Sau Khi Triển Khai Qua Reverse Proxy

## Mục tiêu
Điều tra sự khác biệt giữa HTTP request mà client gửi tới edge và request mà ASP.NET Core thực sự nhận được. Xác định vì sao endpoint hoạt động trên máy phát triển nhưng thất bại khi được gọi qua một TLS-terminating reverse proxy; sửa lỗi mà vẫn giữ chính sách HTTPS cho truy cập trực tiếp.

## Bối cảnh thực tế
Partner Portal gọi API xác nhận vận đơn qua một ingress HTTPS. Sau đợt phát hành mới, health check của edge báo nhiều HTTP 307, client báo `ERR_TOO_MANY_REDIRECTS`, trong khi Kestrel vẫn nhận request và CPU thấp. Cùng endpoint gọi bằng HTTP nội bộ cũng không trả dữ liệu như mong đợi. Đội vận hành nghi ngờ certificate, edge routing, cache và ứng dụng.

Bài lab dùng một proxy contract giả lập qua HTTP headers; không cần DNS, chứng chỉ TLS, Docker hay tài khoản cloud.

## Bạn cần làm gì
1. Chạy starter, tái hiện hai request: qua proxy giả lập và trực tiếp.
2. Ghi status code, `Location`, scheme nhìn thấy ở ứng dụng, và tối thiểu ba hypothesis.
3. Chỉ thay đổi code trong `starter/`; giữ các điều kiện an toàn của production ingress.
4. Chạy `verify.ps1` để chứng minh request hợp lệ qua proxy trả 200, request HTTP trực tiếp vẫn bị chuyển hướng.
5. So sánh giải pháp và trade-off sau khi đã tự điều tra.

## Yêu cầu môi trường
- Windows PowerShell 5.1+ hoặc PowerShell 7.
- .NET SDK 8.0+ có khả năng build `net8.0` (không dùng NuGet bên thứ ba).
- TCP port 5077 rảnh, hoặc truyền `-Port` khác.

## Chạy nhanh
Từ thư mục unit:

```powershell
powershell -ExecutionPolicy Bypass -File .\setup.ps1
powershell -ExecutionPolicy Bypass -File .\reproduce.ps1
# Chỉnh sửa starter/Program.cs sau khi ghi evidence
powershell -ExecutionPolicy Bypass -File .\verify.ps1
```

Để quan sát trực tiếp, chạy `powershell -ExecutionPolicy Bypass -File .\run.ps1` ở terminal riêng.

## Cách reproduce vấn đề
`reproduce.ps1` khởi động chính `starter/`, phát hai request và kết thúc server. Request A mô phỏng client HTTPS tại edge được chuyển tới Kestrel qua HTTP. Request B là HTTP nội bộ không qua edge. Probe cố tình tắt tự động follow redirect để giữ nguyên bằng chứng.

Nếu port 5077 bận, chạy `reproduce.ps1 -Port 5088` và `verify.ps1 -Port 5088`.

## Những gì cần quan sát
- Status code và `Location` của hai đường request có giống nhau không?
- Edge nhận nhiều redirect liên tiếp nhưng backend có exception hay không?
- Tại sao việc thêm retry ở client không cải thiện kết quả?
- Sau sửa, response body phải báo scheme của request A đúng như hợp đồng proxy, đồng thời request B vẫn bị redirect.

Đối chiếu dữ liệu tình huống tại [evidence/incident-timeline.md](evidence/incident-timeline.md). Đây là dữ liệu mô phỏng, không phải log đã chạy trong phiên tạo lab.

## Quy tắc làm lab
1. Reproduce trước.
2. Ghi hypothesis trong [workspace/my-investigation.md](workspace/my-investigation.md).
3. Thử fix trong starter.
4. Verify trên chính starter.
5. Chỉ sau đó mới xem solution.

## Hints
[Hint 1](hints/hint-01.md) · [Hint 2](hints/hint-02.md) · [Hint 3](hints/hint-03.md)

## Reference Solution
⚠️ Spoiler: [solution/README.md](solution/README.md). Kiểm tra độc lập với `verify.ps1 -Reference` chỉ sau khi hoàn tất phần tự làm.

## Expected Results
[Trước sửa](expected-results/before.md) · [Sau sửa](expected-results/after.md).

## Estimated Time
45–60 phút. Difficulty D4, maturity L3 (production incident investigation).
