# Incident evidence — municipal tree survey sync

Các log sau là fixture tổng hợp, không phải dữ liệu production thật.

| Time (UTC) | Batch | Partner HTTP status | Content-Type | App log | Support outcome |
|---|---|---|---|---|---|
| 08:01:02 | invalid | 422 | application/problem+json | HttpRequestException | retry thủ công không cải thiện |
| 08:01:12 | offline | 503 | application/problem+json | HttpRequestException | retry sau bảo trì có thể thành công |
| 08:01:18 | legacy | 500 | text/html | HttpRequestException | cần trace ID từ response header |
| 08:01:22 | ok | 200 | application/json | success | batch được tiếp nhận |

Gợi ý quan sát không mang tính kết luận: HTTP status đã được ghi lại, nhưng nhóm vận hành vẫn không xác định được mã lỗi đối tác hoặc trace ID. Lưu ý body có thể chứa dữ liệu không được phép đưa nguyên văn vào log. Các response là deterministic fake HTTP handler, không có network request.
