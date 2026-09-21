# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Container process chạy và không restart, nhưng readiness không thành công nên Pod không được đưa vào endpoint phục vụ traffic.

## 2. Evidence
`ASPNETCORE_URLS` và `containerPort` đều mô tả listener 8080. Readiness probe lại gửi HTTP request tới port 8081.

## 3. Root cause
Probe target không khớp application listener contract. Kubernetes không đánh dấu Pod Ready nếu probe không kết nối được tới endpoint đã cấu hình, dù process vẫn đang chạy.

## 4. Why the fix works
Đổi readiness probe sang port 8080 khiến probe kiểm tra đúng listener của application. Khi `/health/ready` trả thành công trong deployment thật, Pod có thể trở thành Ready và được Service chọn làm endpoint.

## 5. How to verify
Sửa `starter/deployment.yaml`, sau đó chạy `./verify.ps1`. Kết quả phải có `READY_CONTRACT_OK` và `PASS`.

## 6. Alternative fixes
Có thể dùng named port như `port: http` để giảm drift giữa `containerPort` và probe. Nếu application chủ đích expose health endpoint trên listener riêng, cấu hình listener thứ hai và probe tương ứng cũng hợp lệ.

## 7. Wrong or misleading fixes
Tăng `initialDelaySeconds` không giải quyết endpoint sai. Restart Pod hoặc tăng replica chỉ nhân bản cùng cấu hình lỗi. Đổi application listener chỉ để khớp probe có thể phá Service/container contract hiện có nếu không có lý do kiến trúc.

## 8. Production implications
Một Pod Alive nhưng NotReady có thể khiến rollout treo, giảm capacity hoặc tạo outage khi deployment cũ bị scale down. Health probe configuration là một phần của runtime contract, không chỉ là YAML phụ trợ.

## 9. Trade-offs
Numeric port rõ ràng nhưng dễ drift. Named port giảm duplication nhưng vẫn cần đảm bảo tên trỏ đúng listener. Tách management listener có thể tăng isolation nhưng làm deployment và network policy phức tạp hơn.

## 10. What a Senior engineer should notice
Phải phân biệt process liveness, application reachability và traffic readiness. Khi rollout không nhận traffic nhưng process ổn định, hãy kiểm tra contract giữa orchestrator và application trước khi giả định lỗi business code.
