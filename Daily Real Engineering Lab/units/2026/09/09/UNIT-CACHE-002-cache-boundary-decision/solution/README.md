# Reference Solution

> Một phương án defensible dưới constraints hiện tại, không phải kiến trúc duy nhất đúng.

## Decision

Chưa thêm distributed cache ở thời điểm hiện tại. Tiếp tục đọc trực tiếp từ SQL Server, đo database headroom và query latency, đồng thời chuẩn bị metric/alert để biết khi nào SLO hoặc database capacity bắt đầu bị đe dọa.

## Evidence

- p95 hiện 120 ms, còn khoảng cách đáng kể tới SLO 250 ms.
- SQL CPU peak 42%, chưa có evidence database đang là bottleneck.
- Read/write ratio cao nhưng consistency budget chỉ 5 giây, khiến invalidation trở thành phần quan trọng của thiết kế.
- Team chưa vận hành Redis production; thêm Redis tạo dependency, failure mode và runbook mới.

## Vì sao phương án này hợp lý

Cache là optimization có cost. Khi hệ thống chưa có bottleneck được đo lường, thêm distributed cache có thể đổi một hệ thống đơn giản thành hệ thống có hai nguồn state cần giữ consistency mà chưa tạo giá trị tương xứng.

## Option: IMemoryCache

Có thể hợp lý nếu một nhóm dữ liệu reference có TTL rõ ràng, chấp nhận per-instance staleness và không cần invalidation đồng bộ. Với profile status/job title yêu cầu phản ánh thay đổi trong tối đa 5 giây, cache local trên ba instances cần TTL rất ngắn hoặc invalidation mechanism; lợi ích cần được đo trước.

## Option: Redis cache-aside

Hợp lý hơn khi database load hoặc latency trở thành bottleneck thực sự, hoặc khi nhiều instances cần shared cache semantics. Khi dùng, cần định nghĩa rõ:
- cache key và TTL
- invalidation/update policy
- behavior khi Redis unavailable
- timeout budget
- telemetry cho hit rate, miss latency và stale data

Redis failure không nên tự động làm API unavailable nếu SQL vẫn healthy; cache nên có degradation strategy phù hợp.

## Wrong / Tempting Fixes

### “Traffic sẽ tăng 5x nên thêm Redis ngay”
Forecast không phải bottleneck evidence. Capacity planning nên dùng load test và measured headroom.

### “Dùng TTL 5 giây là giải quyết consistency”
TTL giới hạn staleness theo cách gần đúng nhưng vẫn cần hiểu race, multi-instance behavior và business semantics.

### “Cache mọi profile field”
Cache granularity quá rộng làm invalidation phức tạp hơn. Chỉ cache dữ liệu có reuse và consistency model phù hợp.

## Verification của decision

Decision tốt phải trả lời được:
1. bottleneck hiện tại ở đâu?
2. cache giải quyết metric nào?
3. staleness được phép bao nhiêu?
4. cache outage ảnh hưởng request thế nào?
5. team sẽ vận hành dependency mới ra sao?

## Measurable triggers để revisit

Cân nhắc cache khi một hoặc nhiều điều kiện xuất hiện:
- p95 tiến gần SLO do database read latency
- SQL CPU/read I/O đạt mức headroom không còn an toàn dưới load forecast
- load test 5x cho thấy database là bottleneck
- cùng dữ liệu được đọc lặp lại với reuse đủ cao để cache hit rate có ý nghĩa
- chi phí scale database vượt chi phí và complexity của cache

## Senior engineer nên nhận ra

Senior decision không phải chọn công nghệ mạnh nhất mà là chọn mức complexity nhỏ nhất đáp ứng constraints, đồng thời thiết kế observable triggers để biết khi nào assumptions không còn đúng.
