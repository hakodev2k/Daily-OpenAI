# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Hostname không đổi nhưng process tiếp tục gửi request tới endpoint đã được thay thế sau DNS rotation; restart làm behavior biến mất.

## 2. Evidence
Resolver đã trỏ sang endpoint mới, trong khi pooled request path vẫn giữ endpoint được chọn lúc connection cũ được thiết lập.

## 3. Root cause
DNS resolution không đồng nghĩa với việc mọi HTTP request tạo connection mới. Một pooled connection có thể tiếp tục được tái sử dụng sau khi DNS record thay đổi. Trong simulator, `MaxConnectionAge = int.MaxValue` khiến endpoint đã resolve được giữ vô thời hạn.

## 4. Why the fix works
Đặt lifecycle hữu hạn cho pooled connection buộc request path định kỳ thiết lập connection mới và resolve endpoint lại. Với contract của lab, đặt `MaxConnectionAge = 2` hoặc nhỏ hơn trước khi gửi request đầu tiên làm request thứ hai sau rotation chuyển sang endpoint mới.

Trong .NET production, `SocketsHttpHandler.PooledConnectionLifetime` là cơ chế phù hợp khi cần giới hạn tuổi connection để DNS changes được quan sát mà vẫn giữ lợi ích connection pooling.

## 5. How to verify
Chạy `./verify.ps1`. Script kiểm tra endpoint mới được quan sát trong bound của scenario và learner-editable starter là code path được validate.

## 6. Alternative fixes
`IHttpClientFactory` có thể quản lý handler lifecycle phù hợp với application architecture. Nếu service discovery/proxy layer cung cấp stable connection endpoint và tự cân bằng phía sau, application có thể không cần rotation ngắn. Giá trị lifetime phải dựa trên DNS/service-discovery behavior và connection cost.

## 7. Wrong or misleading fixes
Tạo `HttpClient` mới cho mọi request có thể làm symptom biến mất nhưng đánh đổi connection reuse và có thể gây resource pressure. Restart process chỉ reset state chứ không sửa lifecycle policy. Giảm DNS TTL một mình không đảm bảo connection đang pooled sẽ bị đóng đúng lúc.

## 8. Production implications
Endpoint rotation thường xuất hiện trong deployments, failover và infrastructure changes. Một process giữ connection quá lâu có thể tiếp tục nói chuyện với instance đang drain hoặc retired.

## 9. Trade-offs
Lifetime quá ngắn tăng DNS lookups, TCP/TLS handshakes và connection churn. Lifetime quá dài làm endpoint changes được quan sát chậm. Chọn giá trị theo DNS TTL, deployment drain window, failure behavior và latency/cost của connection establishment.

## 10. What a Senior engineer should notice
Phải phân biệt lifecycle của `HttpClient`, handler, DNS resolution và individual pooled connections. Mục tiêu không phải 'dispose HttpClient thường xuyên' mà là thiết kế connection lifecycle phù hợp với service-discovery contract.