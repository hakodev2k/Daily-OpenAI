# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Cùng metric `checkout.dependency.duration` có các giá trị khoảng `78`, `92` nhưng một dependency khác lại phát ra khoảng `0.110`, `0.126`, dù dữ liệu mô phỏng cho thấy tất cả request đều mất hàng chục đến hàng trăm millisecond.

## 2. Evidence

Instrument công bố `unit: "ms"`. Raw measurements từ `MeterListener` cho thấy hai call site ghi các value khác scale vào cùng instrument.

## 3. Root cause

`RecordInventory` ghi `TimeSpan.TotalMilliseconds`, còn `RecordPricing` ghi `TimeSpan.TotalSeconds`. Cùng một histogram vì thế chứa dữ liệu có hai đơn vị khác nhau trong khi metadata của instrument tuyên bố toàn bộ value là millisecond.

## 4. Why the fix works

Mọi call site cùng convert duration sang `TotalMilliseconds` trước khi gọi `Record`. Metric contract trở nên nhất quán: một instrument, một semantic meaning, một unit.

## 5. How to verify

Thay learner code rồi chạy:

```powershell
./verify.ps1
```

Cần thấy đủ 4 measurement, tất cả nằm trong vùng hợp lý và `Telemetry contract: PASS`.

## 6. Alternative fixes

Có thể chọn giây thay vì millisecond nếu đổi instrument contract một cách đồng bộ ở tất cả producer và downstream query/dashboard. Điều quan trọng là không trộn unit trong cùng time series.

Một wrapper/helper như `RecordDependencyDuration(TimeSpan elapsed, ...)` cũng có thể centralize conversion để giảm nguy cơ call site tự chọn unit.

## 7. Wrong / tempting fixes

- Sửa dashboard bằng cách nhân riêng series `pricing` với 1000: che lỗi producer và làm contract metric tiếp tục mơ hồ.
- Đổi threshold alert để bỏ qua các giá trị nhỏ: giảm triệu chứng nhưng không sửa dữ liệu sai.
- Tạo thêm dashboard riêng mà không sửa instrument: downstream consumer khác vẫn nhận telemetry sai.
- Đổi tên metric nhưng giữ hai conversion khác nhau: không giải quyết root cause.

## 8. Production implications

Mixed-unit telemetry có thể làm percentile, SLO, anomaly detection và capacity decisions sai dù application business flow vẫn hoạt động bình thường. Đây là failure của observability contract, không phải failure của dependency.

## 9. Trade-offs

Centralized helper làm contract rõ hơn nhưng tăng một abstraction nhỏ. Ghi trực tiếp tại call site ít code hơn nhưng dễ drift. Với metric được nhiều component emit, một abstraction mỏng hoặc convention được test thường đáng giá.

## 10. What a Senior engineer should notice

Metric name + unit + tags + aggregation meaning là một public data contract. Khi telemetry trông bất thường, cần kiểm tra raw producer semantics trước khi kết luận production system thật sự nhanh/chậm bất thường. Dashboard chỉ phản ánh dữ liệu nó nhận được; nó không thể tự sửa semantic inconsistency từ producer.
