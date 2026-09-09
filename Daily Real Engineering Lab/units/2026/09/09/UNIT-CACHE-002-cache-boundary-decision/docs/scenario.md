# Scenario

Employee Profile API hiện chạy 3 instances, đọc SQL Server và không có cache.

Constraints:
- 1.2M requests/day hiện tại; forecast 5x trong 3 tháng.
- Read/write ratio khoảng 98/2.
- p95 hiện 120 ms; SLO là 250 ms.
- SQL CPU peak 42%; read latency ổn định.
- Profile status và job title phải phản ánh thay đổi trong tối đa 5 giây.
- Team có 4 backend developers, không có Redis production experience.
- Service phải tiếp tục phục vụ khi một dependency phụ bị lỗi.
- Budget ưu tiên tránh thêm managed service nếu chưa có evidence cần thiết.

Hãy đánh giá ít nhất:
1. Không cache.
2. `IMemoryCache` trên mỗi instance.
3. Redis cache-aside.

Với mỗi phương án, phân tích latency, consistency, invalidation, failure behavior, scale-out, cost và operational complexity. Kết luận bằng một decision hiện tại và các measurable trigger để revisit.
