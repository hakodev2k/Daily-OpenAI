# UNIT-SQL-004 — Inconsistent Lock Order Deadlock

## Mục tiêu

Điều tra một production-style SQL Server concurrency failure nơi hai transaction hợp lệ khi chạy riêng lẻ nhưng có thể deadlock khi chạy đồng thời.

## Bối cảnh thực tế

Một dịch vụ inventory xử lý chuyển tồn kho giữa hai location. Mỗi transfer cập nhật hai row trong cùng bảng. Ở tải thấp hệ thống ổn định, nhưng khi nhiều transfer ngược chiều chạy cùng lúc, một số request thất bại với SQL Server deadlock victim error dù CPU và I/O đều thấp.

## Bạn cần làm gì

1. Reproduce deadlock bằng starter app.
2. Ghi lại evidence và ít nhất hai hypothesis.
3. Xác định transaction interaction tạo circular wait.
4. Sửa code trong `starter/` để loại bỏ điều kiện deadlock mà vẫn giữ tính đúng đắn của transfer.
5. Chạy `verify.ps1`.

## Yêu cầu môi trường

- Windows
- .NET 8 SDK
- SQL Server LocalDB (`MSSQLLocalDB`)

## Chạy nhanh

```powershell
./run.ps1
```

## Cách reproduce vấn đề

```powershell
./reproduce.ps1
```

Script sẽ chạy nhiều cặp transfer đồng thời để tăng xác suất tái hiện. Kết quả trước fix phải xuất hiện ít nhất một `SqlException` có error number `1205` trong số các vòng thử.

## Những gì cần quan sát

- Hai transaction đều cập nhật cùng hai inventory row nhưng theo hướng ngược nhau.
- Khi chạy tuần tự, cả hai đều thành công.
- Khi chạy đồng thời, lỗi xuất hiện không phụ thuộc vào thiếu CPU hay network timeout.
- SQL Server chọn một transaction làm deadlock victim để phá circular wait.

Không cần đo timing tuyệt đối; tập trung vào transaction order, lock ownership và error `1205`.

## Quy tắc làm lab

1. Reproduce trước.
2. Ghi hypothesis vào `workspace/my-investigation.md`.
3. Thử fix trong `starter/`.
4. Verify.
5. Chỉ sau đó mới xem solution.

## Hints

- [Hint 1](hints/hint-01.md)
- [Hint 2](hints/hint-02.md)
- [Hint 3](hints/hint-03.md)

## Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

- [Reference Solution](solution/README.md)

## Expected Results

### Before

- Sequential transfers succeed.
- Concurrent opposite-direction transfers intermittently produce error `1205`.
- Database remains transactionally consistent because one transaction is rolled back.

### After

- Concurrent transfers complete without deadlock across the verification run.
- Final stock totals remain unchanged.
- No retry loop is required merely to hide the underlying ordering problem.

## Estimated Time

45–60 phút.
