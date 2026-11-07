# Reference Solution

> Chỉ xem sau khi đã reproduce vấn đề và tự thử sửa.

## 1. Symptoms

Report chạy thành công nhưng chỉ trả warehouse `1`; warehouse `2` và `3` biến mất dù business contract yêu cầu hiển thị toàn bộ warehouse.

## 2. Evidence

Bảng `Warehouses` có ba row. Warehouse `1` có active snapshot, warehouse `2` chỉ có inactive snapshot, warehouse `3` chưa có snapshot.

## 3. Root cause

`LEFT JOIN` ban đầu tạo null-extended row cho phía phải khi không có matching row. Nhưng `WHERE s.IsActive = 1` được áp dụng sau join. Với null-extended row, predicate không phải `TRUE`, vì vậy những row đó bị loại và query hành xử như inner join đối với điều kiện này.

## 4. Why the fix works

Đặt điều kiện mô tả row nào được phép match vào `ON` predicate:

```sql
SELECT w.Id, s.Quantity
FROM Warehouses AS w
LEFT JOIN InventorySnapshots AS s
    ON s.WarehouseId = w.Id
   AND s.IsActive = 1
ORDER BY w.Id;
```

Warehouse vẫn được giữ bởi outer join; chỉ active snapshot mới được attach nếu tồn tại.

## 5. How to verify

Áp dụng cách sửa tương đương vào `starter/Program.cs`, rồi chạy `./verify.ps1`. Kết quả phải chứa `ROWS=1:25;2:NULL;3:NULL` và `RESULT=PASS`.

## 6. Alternative fixes

Một số requirement khác có thể dùng `WHERE s.IsActive = 1 OR s.Id IS NULL`, nhưng cách đó khác semantics khi warehouse có **chỉ inactive rows**: warehouse có thể vẫn bị loại. Vì vậy cần xuất phát từ business contract, không chỉ vá predicate.

## 7. Wrong / tempting fixes

- Đổi `LEFT JOIN` thành `INNER JOIN`: làm row loss trở thành chủ ý nhưng vẫn sai contract.
- Thêm `DISTINCT`: không khôi phục row đã bị filter mất.
- Thêm index: có thể ảnh hưởng performance, không sửa semantics.
- Chỉ thêm `OR s.IsActive IS NULL`: dễ bỏ sót trường hợp warehouse có inactive child row nhưng không có active child row.

## 8. Production implications

Outer-join filter bugs thường là silent correctness failures trong reporting, reconciliation và operational dashboards. Nên test ít nhất ba trạng thái: matching child, non-matching child, và no child.

## 9. Trade-offs

Filter trong `ON` và filter trong `WHERE` không chỉ là hai cách viết tương đương đối với outer join. Vị trí predicate là một phần của semantics. Sau khi correctness đúng, mới đánh giá execution plan và indexes.

## 10. What a Senior engineer should notice

- Mô hình hóa rõ preserved side và optional side của outer join.
- Phân biệt predicate chọn matching row với predicate lọc final result set.
- Viết regression tests theo business states, không chỉ happy path có matching child.
