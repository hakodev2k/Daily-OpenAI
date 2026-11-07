# Incident evidence — synthetic NOC timeline

`02:00:03` — Deployment manifest accepted: 6 declared steps, ring A, firmware v5.4.2.

`02:00:04` — Planner emits: `download-image`, `verify-signature`, `backup-config`.

`02:00:04` — Planner returns: `status=SUCCESS planned=3/6 diagnostic=<none>`.

`02:03:20` — Device audit: ring A still on v5.4.1; no `announce-completion` entry.

`02:05:10` — Operator reruns the same manifest; output remains `SUCCESS planned=3/6`.

Distractors: artifact checksum is correct, image download latency is normal, no device authentication failure occurred. Những dữ liệu này không chứng minh các bước còn lại đã được lên lịch.

**Lưu ý:** Đây là log synthetic; không có hệ thống mạng thật hoặc dữ liệu khách hàng.
