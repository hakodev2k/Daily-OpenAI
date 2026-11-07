# Incident replay — Asia/Ho_Chi_Minh

| Time (+07) | Event | Evidence |
|---|---|---|
| 16/10 11:00 | C106 DB cleanup thành công | Blob vẫn xuất hiện trong listing |
| 21/10 08:00 | C105 delete request | Client timeout, chưa có definitive receipt |
| 02/11 18:00 | C107 nhân viên tranh chấp xem ảnh | DB ACTIVE, storage 404 |
| 03/11 03:00 | C108 delete message queued | Worker chưa gọi storage |
| 03/11 03:10 | C108 hold API trả ACCEPTED | DB hold ACTIVE |
| 03/11 03:11 | Worker C108 nhận message cũ | Chưa có confirmed physical delete |
| 03/11 03:13 | Snapshot | Xem `cases.csv` |

## Telemetry (giả lập)
`retention_jobs_processed=1200`, `retention_jobs_failed=17`, `delete_api_2xx=1183`, `legal_hold_active=64`. Orphan scan gần nhất cách đây 19 ngày. Một deployment lúc 02:30 chưa có chứng cứ là nguyên nhân.

Hãy phân loại tín hiệu nào có giá trị, cái nào chỉ là correlation. Timeout không chứng minh object còn hoặc đã mất.
