# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Starter ghi đúng 500 measurements nhưng tạo gần 500 distinct metric series dù hệ thống chỉ có hai route và một status code trong mô phỏng.

## 2. Evidence

`MeterListener` gom các tổ hợp tag được ghi vào `search.request.duration`. Khi traffic gồm nhiều user khác nhau, số tổ hợp tăng gần tuyến tính theo số identity thay vì giữ ổn định theo các operational dimensions hữu hạn.

## 3. Root cause

`user.id` được ghi trực tiếp làm metric tag. Giá trị này có cardinality không bị chặn: mỗi user mới có thể tạo thêm một time series mới. Metrics backend thường aggregate và lưu dữ liệu theo từng tổ hợp tag, nên một identity dimension như vậy biến một instrument đơn giản thành hàng trăm, hàng nghìn hoặc hàng triệu series.

## 4. Why the fix works

Reference solution giữ các dimensions phục vụ aggregation vận hành (`http.route`, `http.response.status_code`) và loại `user.id` khỏi metric tags. Trong mô phỏng, 500 measurements lúc này chỉ tạo hai series.

Thông tin identity nếu thực sự cần điều tra một request cụ thể nên được đưa sang structured logs hoặc traces, nơi dữ liệu high-cardinality phù hợp hơn với kiểu truy vấn chi tiết.

## 5. How to verify

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

Contract:

- `Measurements=500`
- `DistinctSeries<=4`
- process trả exit code 0 và in `VERIFY_PASS`

## 6. Alternative fixes

- Bucket một dimension chỉ khi các bucket có ý nghĩa nghiệp vụ rõ ràng và số giá trị được kiểm soát.
- Loại bỏ dimension khỏi metric nhưng giữ correlation/context trong trace/log.
- Nếu một dimension có tập giá trị hữu hạn nhưng lớn, đánh giá giới hạn của collector/backend trước khi quyết định giữ nó.

## 7. Wrong or misleading fixes

- **Tăng quota hoặc retention budget ngay lập tức:** chỉ làm symptom đắt hơn mà không sửa telemetry model.
- **Sampling measurements nhưng vẫn giữ unbounded identity tag:** sampling có thể giảm số điểm dữ liệu nhưng cardinality vẫn tăng theo identity xuất hiện.
- **Hash user ID rồi dùng hash làm tag:** vẫn giữ cardinality gần tương đương vì mỗi user vẫn có giá trị khác nhau.
- **Đưa mọi context vào metric tags để query tiện hơn:** metrics không phải datastore cho arbitrary high-cardinality dimensions.

## 8. Production implications

High-cardinality metrics có thể gây memory pressure ở collector, series-limit drops, dashboard/query chậm và chi phí telemetry tăng mạnh. Vấn đề có thể xuất hiện trước khi application CPU hoặc latency cho thấy dấu hiệu bất thường.

## 9. Trade-offs

Ít tags hơn làm metric aggregation ổn định và rẻ hơn nhưng giảm khả năng drill-down trực tiếp từ metric. Thiết kế observability tốt thường phân vai: metrics cho bounded aggregation/alerting, traces cho request flow và logs cho chi tiết sự kiện/context.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi “metric value có đúng không?” mà còn kiểm tra telemetry cardinality contract. Mỗi tag phải được đánh giá về tập giá trị có thể phát sinh, growth theo traffic, usefulness cho alert/dashboard và nơi phù hợp nhất để lưu context chi tiết.

## References

- Microsoft Learn — Creating Metrics: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-instrumentation
- Microsoft Learn — Collecting Metrics: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics-collection
