# Reference Solution — chỉ mở sau khi tự điều tra

## Symptoms
Gateway trả 404 dù `auth=true`. Request URI thực tế là `https://partner.test/assignments?priority=urgent`, thiếu tenant prefix.

## Evidence
Hai vùng đều ghi URI sai trong JSON; expected path chứa `/api/tenants/{region}/`. Header không bị mất.

## Root cause
Root-relative request `/assignments?priority=urgent` thay thế base path khi HttpClient resolve URI theo RFC 3986.

## Why the fix works
Trong `starter/Program.cs` đổi duy nhất `var path = "/assignments?priority=urgent";` thành `var path = "assignments?priority=urgent";`. BaseAddress kết thúc bằng slash nên relative path được resolve bên trong directory tenant.

## How to verify
Chạy `./verify.ps1`; cả hai dòng phải `pass=true`, status 202. `./reproduce.ps1` sẽ thất bại khi bug đã được sửa.

## Alternative fixes
Xây absolute URI bằng Uri constructor sau khi validate tenant và base configuration. Hoặc chuẩn hóa base URL thành directory rồi resolve relative URI. Cần contract tests cho nhiều tenants.

## Wrong / tempting fixes
Retry 404 không sửa request-target; thay API key không giải thích được path; bỏ gateway route validation che lỗi; hard-code một tenant phá multi-region.

## Production implications
Không log secret; redact sensitive query; validate allowed hosts để tránh SSRF; test cấu hình theo environment.

## Trade-offs
BaseAddress + relative path gọn nhưng cần hợp đồng trailing slash. Absolute URI explicit hơn nhưng phải validate từng thành phần.

## Senior engineer should notice
404 có nhiều giả thuyết. Thu thập request URI và auth evidence trước khi kết luận. Kiểm thử cả tenant prefix và query, không chỉ status.
