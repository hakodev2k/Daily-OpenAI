# Reference Solution — chỉ đọc sau khi reproduce và tự sửa

## Symptoms
Trong 8 giây rollout, request được gửi tới instance trước khi nạp dữ liệu lịch khám xong; 503 xuất hiện ở t=0–3.

## Evidence
Replay có `firstTraffic=0 failures=4`, trong khi `appointmentsReady` chỉ đúng từ t=4. Liveness vẫn true suốt vòng đời.

## Root cause
Một tín hiệu health dùng cho cả liveness và readiness. Process sống không đồng nghĩa service có thể xử lý nghiệp vụ.

## Reference patch
Trong `starter/Program.cs`, đổi `var readiness = processAlive;` thành `var readiness = processAlive && appointmentsReady;`. Không thay `liveness`.

## Why it works
Traffic chỉ được route khi dependency khởi tạo thiết yếu đã sẵn sàng; instance còn sống nhưng chưa ready không bị restart vô ích.

## Verification
Chạy `./scripts/verify.ps1`. Kỳ vọng `firstTraffic=4 failures=0`, `VERIFY_PASS`; các giây 0–3 có `routed=False`, `http=0` (không có request, không phải HTTP status 0 thực tế).

## Alternatives
Dùng ASP.NET Core health checks riêng `/health/live` và `/health/ready`, readiness probe có startup delay phù hợp; startup probe hữu ích cho khởi tạo dài. Trong Kubernetes, kiểm tra `initialDelaySeconds`, `periodSeconds`, `failureThreshold` và `successThreshold` theo dữ liệu thật.

## Wrong fixes / tempting fixes
- Tăng CPU limit: không giải quyết traffic tới service chưa sẵn sàng.
- Đặt liveness false khi cache đang nạp: gây restart loop.
- Sleep cố định 30 giây: có thể chậm vô ích hoặc vẫn thiếu khi cold start lâu.
- Tăng retry từ client: chỉ che lỗi và tạo thêm tải, đôi khi phù hợp như biện pháp bổ sung.

## Production implications and trade-offs
Readiness phải phản ánh các điều kiện thiết yếu, không phụ thuộc tùy tiện vào dịch vụ phụ có thể degrade. Nếu tất cả replica unready, hệ thống mất capacity; cần rolling strategy, minReadySeconds, graceful termination và cảnh báo. Liveness cần đơn giản, ổn định; readiness có thể thay đổi trong runtime. Đo startup latency và thời gian unavailable thực tế trước khi chỉnh thresholds.

## Senior engineer should notice
Tách process health, traffic admission, dependency criticality và deployment rollout budget. Kiểm thử trường hợp một dependency phụ bị lỗi, shutdown/drain, và nhiều replica khởi động đồng thời.
