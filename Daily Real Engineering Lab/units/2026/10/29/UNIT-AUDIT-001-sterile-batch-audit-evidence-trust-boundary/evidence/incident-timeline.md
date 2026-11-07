# Incident timeline — fixture (UTC)

| Time | Quan sát | Nguồn |
|---|---|---|
| 09:05 | Export B-4721 trả 4 events, HTTP 200 | Export log + snapshot T0 |
| 09:06 | Compliance recorder lưu receipt 4 events / lastSequence 204 | Hệ thống riêng |
| 09:17 | PostgreSQL maintenance window bắt đầu; ticket chỉ ghi "cleanup" | Change calendar |
| 09:21 | Export worker retry một job không liên quan vì network timeout | Worker log (noise) |
| 09:27 | p95 API 140 ms; CPU 37% | Metrics (noise) |
| 09:31 | Export B-4721 trả 3 events, HTTP 200 | Export log + snapshot T1 |
| 09:38 | Đội QA báo bản xuất không khớp bản đã nhận | Support ticket |
| 09:43 | API health check 200; không có integrity alert | Monitoring |

Không có đủ dữ liệu để kết luận ai thực hiện thay đổi, thay đổi xảy ra ở lớp nào hoặc có chủ đích hay không. Hãy liệt kê evidence bổ sung cần thu thập và cách bảo toàn chain of custody.
