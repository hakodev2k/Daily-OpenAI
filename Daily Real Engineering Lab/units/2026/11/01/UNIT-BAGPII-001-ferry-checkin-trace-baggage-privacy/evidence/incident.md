# Incident evidence — dữ liệu mô phỏng

Ngày 01/11, 09:48 ICT: security reviewer thấy một bản ghi downstream telemetry chứa header:

```text
trace_id=4bf92f3577b34da6a3ce929d0e0e4736
baggage: request_id=req-742,passenger_email=mina.test%40invalid.example,passport_last4=7319
result=ACCEPTED
```

09:52: check-in service log chỉ chứa `trace`, `gate` và `accepted`, không chứa email hoặc số giấy tờ. 09:55: API trả 200 và không có thay đổi bất thường ở dữ liệu booking. 10:02: đối tác xác nhận bản ghi telemetry được lưu ở hệ thống quan sát ngoài phạm vi kiểm soát của đội check-in.

**Ràng buộc:** Phải giữ trace correlation giữa các service; không được đưa PII vào outbound telemetry; không dùng dữ liệu hành khách thật; không tắt toàn bộ tracing để che vấn đề.

**Giả thuyết cần đánh giá:** logging tại service, serialization request, propagation context, sampling/exporter configuration. Phân biệt nguồn rò rỉ và nơi quan sát được rò rỉ.

Dữ liệu trên là synthetic. `harness/Program.cs` mô phỏng một downstream export, không phải OpenTelemetry exporter thật.
