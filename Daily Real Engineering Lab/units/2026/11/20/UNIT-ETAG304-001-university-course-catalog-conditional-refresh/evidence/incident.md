# Incident evidence — University course catalog
- 09:00 GET /courses → 200, ETag "v1", body: CS101,NET202
- 09:05 GET /courses with If-None-Match: "v1" → 304, no response body
- 09:05 job reports HttpRequestException; last accepted snapshot still contains two courses
- Upstream uptime monitor: green; no elevated CPU
- 09:10 partner publishes version v2 with SEC303
- Chỉ dùng log làm chứng cứ ban đầu, không xem lời giải trước khi tự đặt giả thuyết.
