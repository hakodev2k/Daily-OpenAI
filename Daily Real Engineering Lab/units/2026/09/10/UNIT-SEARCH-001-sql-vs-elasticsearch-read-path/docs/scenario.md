# Scenario

Support Case Search hiện chạy trên PostgreSQL qua một ASP.NET Core API.

## Current workload

- 2.000.000 case hiện hữu.
- Tăng khoảng 70.000 case/tháng.
- 85% request chỉ dùng structured filters + sort + pagination.
- 15% request có free-text query trên `subject` và `body`.
- Mọi query phải filter theo `tenant_id` và tập case mà agent được phép xem.
- Sort phổ biến: `updated_at DESC`.
- Pagination hiện dùng offset/limit.

## Current evidence

- p50: 180–260 ms.
- p95 giờ cao điểm: 1,4–2,1 s.
- CPU database thường 45–60%, đôi lúc 75%.
- API CPU dưới 35%.
- Chưa có execution-plan capture cho các slow query đại diện.
- Chưa có query-frequency breakdown theo shape.
- Chưa đo logical reads / buffer hit ratio cho từng query shape.
- Chưa biết tỷ lệ latency đến từ DB, serialization hay permission enrichment.

## Constraints

- p95 mục tiêu: < 500 ms cho truy vấn phổ biến.
- Search result freshness: <= 10 giây.
- Permission filtering phải chính xác, không được leak dữ liệu tenant/user.
- Team: 5 backend developers; không có search/platform team.
- Near-zero downtime migration.
- Production support ngoài giờ hạn chế.
- Nếu thêm Elasticsearch, team phải sở hữu indexing pipeline, mapping, reindex, drift detection, monitoring và recovery.

## Candidate options

Bạn phải đánh giá tối thiểu ba phương án:

1. tiếp tục PostgreSQL-only;
2. PostgreSQL + targeted full-text/search optimization;
3. Elasticsearch read model cho một phần hoặc toàn bộ search workload.

Có thể bổ sung option khác nếu có lý do.

## Decision questions

- Evidence nào cần thu thập trước khi chốt?
- Structured-filter workload có cần Elasticsearch không?
- Nếu dùng dual-store, source of truth là gì?
- Permission changes propagate thế nào?
- Reindex và schema evolution vận hành ra sao?
- Failure mode khi indexing pipeline lag hoặc Elasticsearch unavailable là gì?
- Trigger nào khiến quyết định hiện tại cần được revisit?
