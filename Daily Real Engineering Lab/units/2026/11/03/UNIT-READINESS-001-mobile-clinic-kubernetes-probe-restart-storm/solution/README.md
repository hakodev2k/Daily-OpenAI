# Reference Solution — SPOILER

## Symptoms & evidence
Ở phút 1–2, database không sẵn sàng; starter tăng restart và lost-in-flight dù process vẫn sống. Kết quả là một lỗi dependency làm phát sinh thêm lỗi vòng đời pod.

## Root cause
Chính sách hiện tại đồng nhất liveness và readiness, gắn cả hai với khả năng truy cập database. Kubernetes liveness failure có thể kích hoạt restart, trong khi readiness failure chỉ rút pod khỏi endpoint nhận traffic.

## Why fix works
Giữ liveness cho sức khỏe của process (ví dụ deadlock không thể hồi phục); readiness đánh giá khả năng nhận request. Trong mô phỏng, dependency outage khiến readiness=false, liveness=true. Khi database phục hồi, readiness trở lại true mà không cần restart.

## Verify
`pwsh ./scripts/verify.ps1` phải trả exit code 0, summary `restarts=0 rejected=2 accepted=2 lost=0`. Chạy `dotnet run --project solution/ProbeLab.csproj` chỉ để so sánh reference, không thay cho kiểm thử starter.

## Alternative fixes
Readiness có thể phản ánh trạng thái circuit breaker hoặc local queue capacity thay vì ping database mọi request. Startup probe dành cho khởi động chậm; liveness nên hạn chế kiểm tra dependency ngoài process.

## Wrong fixes
Tăng replica hoặc giảm liveness period chỉ che triệu chứng; tăng timeout vô hạn có thể giữ pod không healthy; tắt cả readiness khiến pod tiếp tục nhận request khi dependency không thể phục vụ.

## Production implications & trade-offs
Readiness gắn dependency quá chặt có thể làm toàn bộ service không còn endpoint khi database outage; đôi khi ứng dụng vẫn phục vụ cached/read-only traffic. Probe cần timeout ngắn, chi phí thấp, hysteresis hợp lý và monitoring riêng cho dependency. Liveness không nên gọi trực tiếp dependency ngoài process. Cân nhắc graceful termination, PodDisruptionBudget và backoff để tránh thundering herd.

## Senior engineer should notice
Mô phỏng là mô hình chính sách, không phải mô phỏng chính xác kubelet: liveness failure thực tế phụ thuộc thresholds, periods và termination grace. Bảo đảm semantics bằng integration tests trên cluster trước rollout.
