# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Service ổn định ngoài rollout nhưng có request 503 trong đúng termination window.

## 2. Evidence
`R2` được admission khi readiness vẫn true. Process dừng trước khi request hoàn thành. `R1` trước rollout thành công và request sau khi readiness false không được nhận vào instance.

## 3. Root cause
Traffic-admission state không chuyển sang unavailable khi termination bắt đầu. Instance tiếp tục nhận work mới trong khi shutdown timeline đã chạy, nên lifecycle budget của request mới không còn được bảo đảm.

## 4. Why the fix works
Khi termination bắt đầu, đánh dấu instance không ready cho traffic mới trước, rồi dành một drain window cho in-flight work trước khi process dừng. Điều này tách hai khái niệm: `accept new traffic` và `still alive to finish accepted traffic`.

Trong simulation, cập nhật `BeginTermination()` để `_ready = false`, và bảo đảm shutdown xảy ra sau drain window đủ cho work đã admission trước transition.

## 5. How to verify
Chạy `./verify.ps1`. Yêu cầu: không còn failed request, `R1` vẫn thành công, và `R2` bị từ chối admission vào instance đang terminating.

## 6. Alternative fixes
Có thể phối hợp application shutdown hooks, readiness endpoint, `preStop`, `terminationGracePeriodSeconds` và load-balancer propagation tùy platform. Giá trị cụ thể phải dựa trên request duration và routing behavior thực tế.

## 7. Wrong / tempting fixes
- Tăng replica count mà không sửa lifecycle contract: giảm xác suất nhưng vẫn còn race.
- Chỉ tăng timeout client: request vẫn có thể vào instance đang rời service.
- Chỉ tăng termination grace period nhưng vẫn nhận traffic mới suốt window: cửa sổ rủi ro vẫn tồn tại.
- Dùng liveness probe để biểu diễn draining: liveness và traffic admission giải quyết hai câu hỏi khác nhau.

## 8. Production implications
Cần đo request duration, endpoint shutdown behavior, ingress/load-balancer propagation và background work. Long-lived connections, WebSocket/gRPC streaming có drain semantics khác request ngắn.

## 9. Trade-offs
Drain window dài hơn giảm nguy cơ cắt request nhưng làm rollout chậm. Ngừng readiness quá sớm khi capacity thấp có thể gây overload các replica còn lại. Deployment settings phải phù hợp capacity headroom.

## 10. What a Senior engineer should notice
Graceful shutdown là protocol giữa orchestrator, routing layer và application, không phải một timeout đơn lẻ. Hãy lập timeline: stop admission → propagate routing change → drain in-flight work → stop process. Sau đó xác minh bằng telemetry thay vì chỉ chỉnh YAML theo cảm tính.