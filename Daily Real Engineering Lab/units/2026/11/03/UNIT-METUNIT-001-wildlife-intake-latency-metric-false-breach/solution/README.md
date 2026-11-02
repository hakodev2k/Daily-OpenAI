# Reference Solution — spoiler

## 1. Symptoms
Dashboard báo p95=600 giây nhưng trace độc lập có p95=600 milliseconds; không request nào vượt 0,75 giây.

## 2. Evidence
Instrument unit="s"; listener nhận 120,180,240,300,600; trace nguồn 120–600 ms. Nearest-rank p95 của năm mẫu là mẫu lớn nhất. CPU/GC bình thường chỉ là tín hiệu phụ.

## 3. Root cause
Starter ghi TimeSpan.TotalMilliseconds vào Histogram<double> công bố unit "s". Consumer diễn giải số theo giây, phóng đại 1000 lần.

## 4. Why fix works
Đổi measurement thành elapsed.TotalSeconds, giữ instrument unit="s"; producer và consumer dùng cùng contract.

## 5. Verification
Sửa starter/LatencyRecorder.cs rồi chạy verify.ps1. MeterListener kiểm tra unit, từng measurement, p95 và SLO. Reference file không được project tests tham chiếu.

## 6. Alternative fixes
Chuẩn hóa toàn pipeline về milliseconds (instrument unit="ms", exporter/query/threshold cùng chuyển) cũng hợp lệ nhưng là migration rộng. Metric version mới tránh trộn dữ liệu lịch sử.

## 7. Wrong fixes
Nâng threshold lên 750 chỉ che cảnh báo; scale-out không sửa dữ liệu; chỉ đổi metadata thành "ms" mà không migrate consumer phá contract; đổi thuật toán p95 không sửa measurement sai.

## 8. Production implications
Gắn owner cho metric contract; đối chiếu trace/histogram khi canary; đánh dấu lịch sử sai và rollback có kiểm soát. Không backfill tùy tiện khi thiếu provenance.

## 9. Trade-offs
Giữ "s" và sửa producer giảm blast radius khi consumer vốn hiểu giây. Chuyển toàn bộ sang "ms" tốn migration. Versioned metric tăng series/cardinality đổi lấy an toàn mixed-version.

## 10. Senior engineering judgment
Phân biệt telemetry corruption với workload degradation bằng independent evidence. Coi metric name/unit/aggregation là public API và test ngay tại instrumentation boundary.

Sau khi tự làm, có thể copy solution/LatencyRecorder.cs vào starter/LatencyRecorder.cs rồi chạy verify.ps1 để đối chiếu.
