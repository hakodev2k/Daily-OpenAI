# Senior challenge

Thiết kế publish manifest khi có hai publisher đồng thời, crash giữa upload, CDN stale pointer, object eventual visibility và rollback khẩn cấp. Vẽ state machine STAGING → VALIDATED → ACTIVE → RETIRED; chỉ ra preconditions cho từng transition, ETag CAS và fencing token. Đề xuất telemetry đo mixed-release read = 0, failed validation, orphan staging cleanup, publish latency. Tách cơ chế consistency của một HTTP response khỏi freshness của CDN.
