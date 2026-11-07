# Reference Solution — chỉ đọc sau khi tự điều tra

## 1. Symptoms
Starter exit 2 và báo các callback trái policy được chấp nhận. Không có HTTP request thật.

## 2. Evidence
URL \`https://hooks.damtelemetry.example.relay.invalid/ingest\` có host thực là \`hooks.damtelemetry.example.relay.invalid\`. URL \`https://hooks.damtelemetry.example@relay.invalid/ingest\` có userinfo \`hooks.damtelemetry.example\`, host \`relay.invalid\`. Các URL cổng 444 và route \`/ingest-legacy\` cũng ngoài policy.

## 3. Root cause
Starter so sánh prefix của chuỗi URL, không kiểm tra authority/route đã được parse. Chuỗi bắt đầu giống domain đối tác không bảo đảm host thực là domain đó.

## 4. Why the fix works
[CallbackPolicy.cs](CallbackPolicy.cs) parse absolute URI rồi kiểm tra HTTPS, \`IdnHost\` chính xác, port 443, không userinfo/fragment, và path chỉ \`/ingest\` hoặc \`/ingest/v2\`. Query được cho phép theo contract. So sánh host không phân biệt hoa thường, path phải chính xác.

## 5. How to verify
\`./verify.ps1\` biên dịch trực tiếp \`starter/CallbackPolicy.cs\` và chạy 15 fixture độc lập, không dùng solution. Có thể lưu bản sửa riêng rồi đối chiếu với reference.

## 6. Alternative fixes
Dùng allowlist host/route từ cấu hình đã validate tại startup; hoặc không nhận arbitrary URL mà chỉ nhận destination ID do server quản lý. Mỗi lựa chọn khác nhau về tính linh hoạt và bề mặt tin cậy.

## 7. Wrong / tempting fixes
Chỉ thêm \`/\` vào string prefix có thể chặn vài host giả nhưng không diễn đạt đầy đủ route policy. Chỉ cấm \`@\` không xử lý host suffix, port hoặc path. Chỉ test URL hợp lệ không phát hiện false-accept. Các biện pháp này có thể hữu ích trong ràng buộc khác, nhưng không đủ cho contract này.

## 8. Production implications
Lab chỉ kiểm tra URI và policy, không thực hiện network. Nếu production thực sự truy cập URL do người dùng cung cấp, cần thêm kiểm soát DNS/IP sau resolution, redirect, egress, timeout, DNS rebinding và audit. URI validation riêng lẻ không đủ phòng mọi SSRF.

## 9. Trade-offs
Exact host/path dễ audit và giảm false-accept nhưng thay đổi endpoint cần rollout policy. Wildcard host thuận tiện nhưng mở rộng trust boundary. Query được phép ở đây; query chứa secret hoặc thay đổi routing cần thêm rule.

## 10. Senior engineering observations
Phân biệt string identity với network authority; viết contract bằng positive và negative cases; verification độc lập với starter; xác định rõ giới hạn của validation và biện pháp bảo vệ ở tầng network.
