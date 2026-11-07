# Incident evidence — 2026-11-01 (synthetic)

| Time | Component | Observation |
|---|---|---|
| 09:00 | Source | W-17 emits evt-01, version 1, OPEN |
| 09:02 | Broker | evt-02, version 2, SUSPENDED cannot be delivered; moves to DLQ |
| 09:06 | Source | W-17 emits evt-03, version 3, CLOSED |
| 09:07 | Projection | Public read model shows CLOSED (version 3) |
| 09:12 | Operations | DLQ redrive sends evt-02 to the same consumer |
| 09:12 | Projection | Public read model now shows SUSPENDED (version 2) |
| 09:13 | Broker | evt-03 redelivered after transient acknowledgement failure |
| 09:14 | Monitoring | CPU 19%, no DB timeout; read cache TTL 0 in local simulation |

Đây là dữ liệu giả lập phục vụ điều tra, không phải log production thật. Tìm sự khác nhau giữa thứ tự phát hành, thứ tự delivery và thứ tự áp dụng trạng thái. Không mặc định CPU hoặc cache là nguyên nhân.
