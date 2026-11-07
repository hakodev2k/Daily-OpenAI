# Synthetic incident timeline

Simulator có 8 ticks, failureThreshold=2 cho liveness; không phải log từ Kubernetes cluster thật.

| Tick | Process | Store | Risk engine | Routed response | Restart |
|---|---|---|---|---|---|
| 0 | healthy | healthy | healthy | 200-personalized | no |
| 1 | healthy | healthy | healthy | 200-personalized | no |
| 2 | healthy | healthy | down | 503-unrouted | no |
| 3 | healthy | healthy | down | 503-unrouted | yes |
| 4 | healthy | healthy | down | 503-unrouted | no |
| 5 | healthy | healthy | down | 503-unrouted | yes |
| 6 | healthy | healthy | healthy | 200-personalized | no |
| 7 | healthy | healthy | healthy | 200-personalized | no |

Tín hiệu phụ: không có process crash hoặc lỗi primary store. Phân biệt correlation với causation bằng kiểm tra policy.

Giới hạn: không mô phỏng startup delay, probe interval thực tế, restart downtime, EndpointSlice propagation hay network partitions.
