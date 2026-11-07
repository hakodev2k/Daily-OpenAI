# Synthetic metrics — 30 bucket / scenario

| Scenario | Requests/min | Errors/min | Duration | Observation from on-call |
|---|---:|---:|---|---|
| Low-volume outage | 100 | 12 | 30 min | Nhiều khách không nhận được notification |
| High-volume peak | 30,000 | 25 | 30 min | Traffic tăng nhưng tỷ lệ lỗi vẫn thấp |
| Brief spike | 100 | 15 | 1 min cuối | Tự phục hồi ngay sau spike |
| Moderate drift | 1,000 | 2 | 30 min | Có degradation nhỏ cần theo dõi |
| Zero traffic | 0 | 0 | 30 min | Không có request để đánh giá success rate |
| Recovered incident | 100 | 12 (15 min đầu), sau đó 0 | 30 min | Sự cố đã phục hồi trong 15 phút gần đây |

Dữ liệu synthetic, deterministic; không phải số liệu production thực. Dùng `./scripts/run.ps1` để xem policy hiện tại phân loại từng scenario. Chú ý phân biệt số lỗi tuyệt đối, tỷ lệ lỗi và thời gian kéo dài.
