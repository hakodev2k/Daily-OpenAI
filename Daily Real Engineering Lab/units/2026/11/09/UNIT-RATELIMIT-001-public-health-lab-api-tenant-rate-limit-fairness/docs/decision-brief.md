# Decision brief

## Yêu cầu
- Bảo vệ tối thiểu 30 accepted/10s cho A khi A gửi 30, với workload đã cho.
- Không vượt **120 accepted/10s** toàn hệ thống.
- Tránh một tenant độc chiếm capacity; không giả định client tự điều tiết.
- Phân biệt 429 do quota tenant, global saturation và lỗi dependency.
- Cân nhắc two-replica consistency, latency overhead, Redis unavailable, rollout và rollback.
- B không cần được đảm bảo throughput khi A/C bận, nhưng phải có cơ hội sử dụng spare capacity.

## Phương án bắt buộc so sánh
A. Nâng global fixed-window từ 100 lên 120, giữ FIFO.

B. Per-tenant fixed quota: A=30, B=50, C=20 / 10s, không borrowing.

C. Global cap 120 + reservation A=30, C=20, phần còn lại cấp cho B; cho phép borrowing capacity còn trống với quy tắc rõ ràng.

D. Weighted token bucket per tenant + global cap, có admission priority, burst cap và giới hạn retry.

## Replay
- Với mỗi phương án, ghi accepted/rejected theo tenant ở W1–W4.
- Đối với C/D, định nghĩa **thứ tự admission** và borrowing trước khi tính. Không thể tính kết quả duy nhất nếu chưa xác định policy.
- Kiểm tra invariants: tổng accepted <=120; A>=30 khi A demand=30; no double-spend khi 2 replicas.
- Với phương án D, chọn capacity/refill/burst cụ thể và ghi rõ initial state; nếu không thể mô phỏng chính xác, giải thích giới hạn và bổ sung test cần chạy.

## Sản phẩm bàn giao
ADR một trang: context, options, decision, capacity math, risks, metrics, canary plan, rollback, rejected alternatives.
