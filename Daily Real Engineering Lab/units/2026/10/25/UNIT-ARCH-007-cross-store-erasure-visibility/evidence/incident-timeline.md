# Evidence — Incident CA-2026-1025 (synthetic)

Tất cả timestamp UTC+7; cùng tenant T-018, record R-8842. Log bên dưới được cố tình cung cấp ở các lớp khác nhau. Không có dòng nào tự chứng minh toàn bộ yêu cầu đã hoàn tất.

| Time | Component | Evidence |
|---|---|---|
| 10:58:02 | reindex-export | snapshot export E-491 bắt đầu, watermark 10:58:02 |
| 11:03:04 | public-api | DELETE /records/R-8842 -> 202; request id req-73 |
| 11:03:06 | sql-writer | record R-8842 status=removed; transaction committed |
| 11:03:07 | notification | support ticket marked completed; user message sent |
| 11:03:10 | search-worker | delete document R-8842 -> 200, attempt=1 |
| 11:04:20 | public-search | query returned 0 hits for R-8842 |
| 11:05:01 | reindex-worker | processing export E-491 (created before removal) |
| 11:05:10 | search-worker | index R-8842 -> 201; source=E-491 |
| 11:06:03 | public-search | query returned R-8842; result-cache=MISS |
| 11:06:12 | public-search | same query returned R-8842; result-cache=HIT |
| 11:08:00 | blob-edge | GET previously issued attachment link -> 200 |
| 11:13:00 | blob-edge | same attachment link -> 403 (expired) |

## Competing hypotheses

Đánh giá từng giả thuyết bằng evidence, không kết luận chỉ từ tên dịch vụ:

- Transaction SQL chưa commit.
- Public search trả cached response.
- Một batch reindex có nguồn dữ liệu cũ.
- Link attachment có vòng đời khác API authorization.
- Event delivery bị duplicate hoặc out-of-order.
- Multi-region replication bị lag.

## Thiếu bằng chứng nào?

Đề xuất ít nhất năm trường log/metric hoặc phép đo bổ sung để xác nhận đường đi của một record qua từng boundary. Không đưa raw PII vào logs. Nêu metric nào có thể đo bằng synthetic canary.
