# UNIT-OBS-005 — Logging Template Cardinality

## Mục tiêu

Điều tra một API có chức năng đúng nhưng hệ thống quan sát bắt đầu sinh quá nhiều nhóm log khác nhau khi lưu lượng tăng.

## Bối cảnh thực tế

Một endpoint tra cứu đơn hàng ghi log cho mỗi request. Dashboard vẫn nhận đủ log, nhưng số lượng message template khác nhau tăng gần theo số order được truy cập. Điều này làm việc tìm kiếm, tổng hợp và tạo alert trở nên khó kiểm soát.

## Bạn cần làm gì

1. Chạy starter.
2. Reproduce triệu chứng và ghi lại evidence.
3. Viết hypothesis trước khi sửa.
4. Chỉnh code trong `starter/` để giữ nguyên thông tin cần thiết nhưng làm cho cấu trúc telemetry ổn định hơn.
5. Chạy `verify.ps1`.
6. Sau đó mới đọc reference solution.

## Yêu cầu môi trường

- .NET SDK 8.x
- PowerShell 7+ khuyến nghị

## Chạy nhanh

```powershell
./run.ps1
./reproduce.ps1
```

## Cách reproduce vấn đề

`reproduce.ps1` chạy workload với nhiều order ID và kiểm tra số lượng logging template quan sát được.

## Những gì cần quan sát

- Tổng số event được ghi.
- Số lượng template/signature khác nhau.
- Quan hệ giữa số order ID khác nhau và số template khác nhau.
- Thông tin order ID có còn truy vấn được hay không.

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

[Spoiler — chỉ xem sau khi tự thử](solution/README.md)

## Expected Results

Trước khi sửa, workload nhỏ đã tạo ra nhiều template/signature khác nhau. Sau khi sửa, số template phải được giới hạn trong khi dữ liệu order ID vẫn được giữ dưới dạng field có thể truy vấn.

## Estimated Time

35–50 phút.
