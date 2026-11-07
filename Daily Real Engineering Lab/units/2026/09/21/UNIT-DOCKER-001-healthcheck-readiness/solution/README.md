# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Process chạy và health signal trả success trong startup, nhưng business request vẫn nhận `503`.

## 2. Evidence
Timeline cho thấy `ProcessAlive=true` trong khi `InitializationComplete=false`; hai trạng thái có ý nghĩa vận hành khác nhau.

## 3. Root cause
Routing health contract chỉ kiểm tra process liveness nên instance được xem là có thể nhận traffic trước khi required initialization hoàn tất.

## 4. Why the fix works
Tách liveness khỏi readiness. Readiness phản ánh khả năng phục vụ traffic; liveness chỉ phản ánh process có cần restart hay không.

## 5. How to verify
`verify.ps1` chạy learner-editable starter và yêu cầu routing health signal từ chối traffic trong trạng thái startup chưa hoàn tất.

## 6. Alternative fixes
Có thể dùng framework health checks với tag riêng cho readiness/liveness hoặc một startup gate ở reverse proxy/orchestrator, miễn contract và ownership rõ ràng.

## 7. Wrong or misleading fixes
Thêm `sleep` cố định chỉ che race và phụ thuộc tốc độ môi trường. Kéo dài timeout không sửa semantic của probe. Dùng một probe cho mọi quyết định có thể gây restart loop khi dependency tạm thời unavailable.

## 8. Production implications
Readiness nên phản ánh các điều kiện thực sự bắt buộc để nhận traffic nhưng không nên biến mọi dependency phụ thành hard dependency. Probe quá nghiêm có thể tự gây outage.

## 9. Trade-offs
Readiness sâu hơn tăng độ chính xác routing nhưng có thể tăng coupling với dependency health. Cần chọn điều kiện theo khả năng degrade của application.

## 10. What a Senior engineer should notice
Health endpoint không chỉ là URL trả 200; nó là operational contract giữa application và platform. Liveness, readiness và startup semantics phục vụ các quyết định khác nhau.