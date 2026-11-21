# Reference Solution — chỉ xem sau khi tự điều tra
## Symptoms
Request #2, #3, #5, #8, #9, #10 trả sai; baseline 2 MISS, 8 HIT (80% hit), nhưng chỉ 4/10 response đúng.
## Evidence
Origin đúng theo ba chiều route, locale, audience. Request đầu tiên của mỗi route quyết định nội dung cached cho mọi request cùng path.
## Root cause
Shared cache key thiếu các dimension quyết định representation; không phải origin dịch sai.
## Why the fix works
Key `route|normalized-locale|normalized-audience`, allowlist vi/en và general/student; reject hoặc bypass giá trị ngoài allowlist. Replay: 5 distinct entries, 5 MISS, 5 HIT, 10/10 đúng. Header `Vary: Accept-Language, X-Audience` hữu ích nếu đúng các header nguồn, nhưng phải cấu hình cache key tại CDN cụ thể; không tin Vary tự động. Tránh raw Accept-Language không giới hạn.
## How to verify
Replay 10 request, assert 10/10 đúng, 5 entries, 5 hits. Đảo thứ tự để chống first-writer dependency. Test header lạ, casing, purge mọi biến thể R10, response private không shared-cache.
## Alternatives
A: key ba chiều giữ 50% hit, tăng entries và complexity purge. B: bypass edge cache đạt correctness, 0% hit, origin chịu tải. C: canonical variant URL đơn giản hóa key nhưng thay đổi URL contract.
## Wrong fixes
Tăng TTL kéo dài lỗi. Purge định kỳ chỉ thay đổi response đầu tiên. Chỉ thêm Vary mà không test CDN không đảm bảo. Dùng raw header gây key explosion.
## Production implications
Theo dõi synthetic variant probes, origin QPS, hit ratio, key cardinality, purge propagation. Purge bằng surrogate route tag hoặc versioned content.
## Change request
Thêm accessibility dimension default/screen-reader vào key. Tối đa 2 routes × 2 locales × 2 audiences × 2 accessibility = 16, vượt budget 12 nếu đủ tổ hợp: chọn bypass biến thể hiếm hoặc điều chỉnh budget, không được bỏ dimension. Account response cần `Cache-Control: private, no-store` và không lưu tại shared edge.
## Trade-offs
Correctness là hard constraint, hit ratio là mục tiêu thứ cấp. Cache nhiều biến thể tăng memory, giảm reuse và phức tạp invalidation.
## Senior engineer should notice
Kiểm chứng key identity, cacheability, invalidation và header normalization bằng trace; không suy luận correctness từ hit ratio.
