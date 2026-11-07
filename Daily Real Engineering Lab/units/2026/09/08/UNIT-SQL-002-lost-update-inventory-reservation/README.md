# UNIT-SQL-002 — Lost update trong inventory reservation

Lab này mô phỏng hai request cùng đọc tồn kho, cùng tính giá trị mới trong application và cùng ghi lại row, khiến cả hai báo thành công nhưng dữ liệu cuối không phản ánh tổng reservation.

Mục tiêu: reproduce race condition, thu thập evidence, xác định invariant tồn kho, sửa bằng database-side atomic update, rồi verify.

## Chạy

```powershell
./run.ps1
./reproduce.ps1
./verify.ps1
```

## Nhiệm vụ

1. Reproduce trước khi sửa.
2. Ghi ít nhất hai hypotheses.
3. Xác định race window giữa read và write.
4. Sửa starter để database enforce invariant atomically.
5. Verify trước khi xem solution.

## Expected result

Trước fix, hai reservation 7 đơn vị từ stock 10 đều có thể báo thành công. Sau fix, tối đa một reservation thành công và final quantity phải là 3.

Hints nằm trong `hints/`. Reference solution nằm trong `solution/README.md`.
