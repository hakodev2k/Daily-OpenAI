# Synthetic incident evidence — dữ liệu giả lập để điều tra

Đội vận hành báo cáo: dashboard realtime hiển thị số liệu nhưng telemetry phía client không có correlation ID; health check có correlation ID. Chưa có bằng chứng proxy loại header.

| Route | Status | Content-Type | X-Trace-Id | Payload |
|---|---|---|---|---|
| GET /health | 204 | (none) | present | empty |
| GET /api/readings/live | 200 | text/event-stream | absent | event: vibration / data: 2.4 |

Bằng chứng này là **kỳ vọng từ starter được static-review**, không phải log đã thực thi bởi automation. Hãy chạy harness để thu thập evidence thật trên máy của bạn.
