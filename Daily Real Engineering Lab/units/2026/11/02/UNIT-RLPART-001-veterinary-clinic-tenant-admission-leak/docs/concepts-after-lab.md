# Concepts after lab — đọc sau khi verify

- `PartitionedRateLimiter<TResource>` ánh xạ request vào key; mỗi key sở hữu limiter riêng. Sai dimension gây noisy-neighbor hoặc quota bypass.
- `FixedWindowRateLimiter` đếm permit trong cửa sổ. `AutoReplenishment=false` giữ cửa sổ ổn định trong harness ngắn, cho deterministic test.
- Authenticated principal claim đáng tin hơn header client tự gửi. Ứng dụng thật phải chạy authentication trước limiter phụ thuộc claim; missing claim cần trả 401/403 ở boundary, không tạo 500.
- Rate limiting không thay authorization. State trong memory là per-instance, không phải global quota trên nhiều replica.
- Key cardinality không giới hạn có thể tăng memory; tránh partition theo dữ liệu tùy ý và metric label có cardinality cao.
- Sliding window/token bucket giảm burst biên cửa sổ nhưng phức tạp hơn; chỉ thêm distributed coordinator nếu contract đòi quota toàn cluster.

Tham khảo: tài liệu chính thức Microsoft ASP.NET Core rate limiting và System.Threading.RateLimiting theo phiên bản .NET đang dùng.
