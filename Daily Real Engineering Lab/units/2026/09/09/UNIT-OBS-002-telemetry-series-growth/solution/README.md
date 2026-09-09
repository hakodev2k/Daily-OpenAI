# Reference Solution — chỉ xem sau khi đã tự điều tra

## 1. Symptoms

Traffic và business behavior ổn định nhưng số metric series tăng theo số user hoạt động; telemetry exporter dùng nhiều memory hơn.

## 2. Evidence

Starter ghi đúng 2.000 measurements nhưng tạo khoảng 1.500 series. `dimension_names` cho thấy metric đang gắn cả `region`, `status_code` và `user_id`.

## 3. Root cause

`user_id` là dimension có cardinality rất cao. Metric backend tạo series riêng cho từng tổ hợp dimension values, nên số series tăng gần theo số user thay vì chỉ theo các trạng thái vận hành hữu hạn.

## 4. Why the fix works

Reference solution chỉ giữ `region` và `status_code` làm metric dimensions. Hai dimension này có tập giá trị hữu hạn trong scenario, nên số series bị chặn ở vài tổ hợp trong khi 2.000 measurements vẫn được ghi đầy đủ.

User/request identifiers nếu cần correlation nên đi vào logs hoặc traces, nơi dữ liệu định danh không tạo metric time series mới.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Expected:

- `measurements=2000`
- `series_count` nằm trong khoảng nhỏ hữu hạn
- `dimension_names=region,status_code`

## 6. Alternative fixes

- Bucket một dimension thành nhóm hữu hạn nếu business thực sự cần phân tích theo phân khúc.
- Tách metric tổng hợp và trace/log có correlation ID.
- Áp dụng allow-list cho metric dimensions tại telemetry boundary.

## 7. Wrong / misleading fixes

- Tăng memory cho exporter: chỉ kéo dài thời gian trước khi cardinality tiếp tục tăng.
- Giảm sampling request nhưng vẫn giữ `user_id`: có thể giảm tốc độ tăng nhưng không sửa metric contract.
- Scale telemetry collector: có thể cần ở tải lớn, nhưng không giải quyết việc tạo series theo business identifiers.
- Hash `user_id`: vẫn giữ gần như cùng cardinality, chỉ đổi representation.

## 8. Production implications

High-cardinality metrics có thể tăng memory, storage, query cost và làm dashboard/alert chậm. Vấn đề thường xuất hiện sau một instrumentation change dù application traffic không tăng.

## 9. Trade-offs

Loại identifier khỏi metrics làm mất khả năng filter metric trực tiếp theo từng user. Đổi lại, metric trở nên ổn định và aggregatable. Khi cần điều tra một user cụ thể, traces/logs phù hợp hơn.

## 10. What a Senior engineer should notice

Observability schema là một production contract. Khi thêm metric dimension, phải review cardinality giống như review index/key design: ước lượng tập giá trị, growth theo thời gian, chi phí backend và mục đích truy vấn trước khi deploy.
