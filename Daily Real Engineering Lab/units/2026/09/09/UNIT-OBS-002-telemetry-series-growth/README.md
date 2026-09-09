# UNIT-OBS-002 — Telemetry tăng mạnh dù traffic ổn định

## Mục tiêu

Điều tra một production observability incident nơi số lượng metric series tăng rất nhanh dù request volume và business behavior gần như không đổi.

## Bối cảnh thực tế

Checkout API vừa bổ sung telemetry chi tiết để hỗ trợ vận hành. Sau deployment, request rate vẫn ổn định nhưng số lượng metric series tăng theo số người dùng hoạt động, exporter sử dụng nhiều memory hơn và chi phí telemetry có xu hướng tăng mạnh.

Không có exception rõ ràng. API vẫn trả response đúng.

## Bạn cần làm gì

1. Chạy starter và reproduce symptom.
2. Ghi lại số request, measurements, metric series và dimension names.
3. Đọc evidence và đưa ra ít nhất 3 hypothesis.
4. Xác định telemetry contract nào đang làm số series tăng theo dữ liệu business.
5. Sửa code trong `starter/` để telemetry vẫn hữu ích cho aggregation nhưng số series không tăng theo số user.
6. Chạy `verify.ps1`.
7. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ hoặc Windows PowerShell
- Không cần database, Docker hay cloud account.

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script phải chứng minh deterministic rằng số metric series lớn bất thường so với số dimension business hữu hạn của scenario.

## Những gì cần quan sát

- `requests`
- `measurements`
- `series_count`
- `dimension_names`
- quan hệ giữa số user khác nhau và số series

Đừng giả định vấn đề nằm ở request throughput chỉ vì memory của telemetry pipeline tăng.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thu thập evidence.
4. Thử fix trong `starter/`.
5. Chạy verify.
6. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Spoiler: chỉ mở sau khi đã reproduce và thử fix.

[Reference Solution](solution/README.md)

## Expected Results

### Starter

- 2.000 request tạo đúng 2.000 measurements.
- số metric series tăng lên khoảng hàng nghìn.
- request behavior vẫn đúng.

### Sau khi sửa

- vẫn ghi đủ 2.000 measurements.
- metric aggregation vẫn giữ các dimension vận hành cần thiết.
- số series còn ở mức hữu hạn nhỏ và không tăng theo số user khác nhau.

Nếu không reproduce được, kiểm tra đang chạy đúng project trong `starter/Lab` và chưa sửa starter trước khi chạy `reproduce.ps1`.

## Estimated Time

45–60 phút.
