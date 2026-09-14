# UNIT-OBS-005 — Metric Series Explosion

## Mục tiêu

Điều tra một vấn đề observability trong đó lượng traffic vẫn nhỏ nhưng số metric series tăng gần như theo số request, làm collector/backend phải theo dõi quá nhiều time series và khiến chi phí lưu trữ, xử lý tăng bất thường.

## Bối cảnh thực tế

Một search API vừa bổ sung custom metric để theo dõi request latency. Functional behavior vẫn đúng, CPU ứng dụng không có dấu hiệu bất thường, nhưng telemetry backend bắt đầu cảnh báo số lượng series tăng rất nhanh khi có nhiều người dùng khác nhau.

Lab mô phỏng 500 request cục bộ và dùng `MeterListener` để đếm số tổ hợp tag khác nhau mà metric tạo ra.

## Bạn cần làm gì

1. Chạy trạng thái starter và xác nhận symptom.
2. Ghi ít nhất 2 hypothesis vào `workspace/my-investigation.md`.
3. Điều tra cách metric dimensions/tags được tạo.
4. Sửa code trong `starter/` để vẫn giữ metric hữu ích cho vận hành nhưng tránh số series tăng theo từng request/user.
5. Chạy `verify.ps1`.
6. Chỉ sau đó mới xem reference solution.

## Yêu cầu môi trường

- .NET 8 SDK
- PowerShell

Không cần database, Docker hay cloud service.

## Chạy nhanh

```powershell
./reproduce.ps1
```

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` build và chạy starter, sau đó kiểm tra rằng:

- có đúng 500 measurements được ghi;
- số distinct metric series lớn bất thường so với số route/status thực tế của hệ thống mô phỏng.

## Những gì cần quan sát

- `Measurements`
- `DistinctSeries`
- những dimensions xuất hiện trong mỗi measurement
- dimension nào có tập giá trị hữu hạn và dimension nào tăng theo traffic

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis.
3. Thử fix.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — chỉ xem sau khi đã reproduce và tự thử fix.

[Reference Solution](solution/README.md)

## Expected Results

Before:
- 500 measurements.
- hàng trăm distinct series.

After:
- vẫn ghi đủ 500 measurements.
- số distinct series chỉ còn một tập nhỏ, ổn định theo các operational dimensions hữu hạn.
- business behavior không thay đổi.

## Estimated Time

45–60 phút.
