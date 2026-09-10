# Reference Solution

> Chỉ xem sau khi đã tự viết quyết định.

## 1. Symptoms

Search p95 cao hơn mục tiêu, nhưng dữ liệu hiện có chưa chứng minh PostgreSQL là bottleneck duy nhất và cũng chưa chứng minh Elasticsearch sẽ xử lý đúng phần workload đang chậm.

## 2. Evidence

Trước khi thay đổi architecture, cần lấy representative query shapes, execution plans, logical reads, row estimates, index usage, DB wait events, latency breakdown ở API, tỷ lệ structured-only so với free-text, độ sâu offset pagination và chi phí permission filtering.

## 3. Root cause của quyết định kiến trúc yếu

Sai lầm chính là nhảy từ symptom “search chậm” sang technology “Elasticsearch” trước khi xác định bottleneck và workload boundary. 85% request hiện là structured filters; việc đưa toàn bộ read path sang một search platform mới có thể chuyển bài toán latency thành bài toán consistency, authorization drift, indexing lag và vận hành hai data stores.

## 4. Một quyết định có thể bảo vệ được

Ở constraints hiện tại, ưu tiên PostgreSQL-first trong giai đoạn đầu:

- capture và tối ưu query plans cho top query shapes;
- thiết kế composite/partial indexes theo filter + sort thực tế;
- bỏ deep offset pagination nếu nó là bottleneck và dùng stable keyset/cursor khi phù hợp;
- đo riêng full-text workload;
- thử PostgreSQL full-text/trigram cho 15% free-text trước khi thêm hệ thống mới.

Chỉ đưa Elasticsearch vào nếu evidence cho thấy search semantics hoặc scale vượt khả năng hợp lý của PostgreSQL, ví dụ relevance ranking phức tạp, fuzzy matching lớn, aggregations/search features quan trọng, hoặc full-text workload tăng đáng kể và SQL tuning không đạt SLO với chi phí hợp lý.

Nếu Elasticsearch được thêm, nên xem nó là derived read model; PostgreSQL vẫn là source of truth. Indexing cần idempotent pipeline, lag metrics, replay/reindex strategy và reconciliation. Authorization phải được thiết kế rõ: hoặc index security attributes đủ để filter an toàn, hoặc dùng candidate IDs từ search rồi authoritative-filter tại source với trade-off latency/complexity được đo.

## 5. Verification

Một quyết định chưa hoàn tất nếu không có measurement plan. Sau PostgreSQL optimization, chạy workload representative và so sánh p50/p95/p99, DB CPU, logical reads, query plan stability và correctness của tenant/permission filters. Nếu thử Elasticsearch, đo thêm indexing lag, query latency, stale-result window, failure behavior khi indexer/search cluster unavailable và chi phí vận hành.

## 6. Alternative fixes

- PostgreSQL-only với targeted indexes và full-text/trigram.
- Hybrid: structured filters ở PostgreSQL, Elasticsearch chỉ cho search-heavy flow.
- Elasticsearch read model rộng hơn nếu product requirements chuyển sang search-centric workload.

## 7. Wrong / tempting fixes

- “Elasticsearch nhanh hơn SQL” không phải evidence.
- Dual-write trực tiếp từ request handler sang PostgreSQL + Elasticsearch dễ tạo partial-success inconsistency.
- Copy toàn bộ permission logic vào index mà không có reconciliation làm tăng nguy cơ stale authorization.
- Scale-up database trước khi biết slow-query shape có thể chỉ che triệu chứng.

## 8. Production implications

Thêm search engine tạo thêm deployment, monitoring, backup/rebuild, mapping/versioning, indexing throughput, queue lag, security và incident-response responsibilities. Operational ownership phải được tính như một phần của architecture cost.

## 9. Trade-offs

PostgreSQL-first có complexity thấp, strong consistency và authorization path đơn giản hơn, nhưng search features có giới hạn. Elasticsearch có search capability mạnh và scale read/search tốt nhưng đổi lấy eventual consistency và vận hành hệ thống thứ hai.

## 10. Senior engineer should notice

Senior engineer không chọn technology từ tên symptom. Họ yêu cầu evidence, chia workload thành các nhóm, tối ưu boundary nhỏ nhất trước, định nghĩa source of truth, failure semantics và tiêu chí cụ thể để revisit quyết định khi assumptions thay đổi.
