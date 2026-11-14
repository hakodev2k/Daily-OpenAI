# Reference Solution — chỉ xem sau khi tự reproduce và sửa

## Symptoms và evidence
Fixture có 4 hộ eligible, 2 hộ đã cấp (102, 105), nhưng truy vấn ban đầu trả 0 hàng. Cột household_id của dữ liệu import có NULL.

## Root cause
`NOT IN (102, NULL, 105)` trên các ID không khớp tạo biểu thức UNKNOWN theo SQL three-valued logic. WHERE chỉ giữ TRUE, vì vậy cả 101 và 103 đều bị loại.

## Why the fix works
`NOT EXISTS` dùng tương quan `a.household_id = h.id`. Hàng NULL không khớp ID nào, nên hai hộ chưa cấp được giữ lại. Xem [query.sql](query.sql).

## Verification
Thay nội dung `starter/query.sql` bằng truy vấn tham khảo rồi chạy `./scripts/verify.ps1`; mong đợi `RESULT_IDS: [101, 103]`. Script kiểm tra chính file learner sửa, không kiểm tra file solution.

## Alternative fixes
Lọc `WHERE household_id IS NOT NULL` bên trong NOT IN có thể phù hợp nếu hợp đồng dữ liệu đảm bảo ID hợp lệ; vẫn cần kiểm soát NULL và khóa. LEFT JOIN ... IS NULL cũng có thể hợp lệ khi điều kiện join đúng.

## Wrong / tempting fixes
Xóa bản ghi NULL trong fixture chỉ che khuất vấn đề ingest. Đổi NOT IN thành IN đảo nghĩa nghiệp vụ. Thêm DISTINCT không giải quyết UNKNOWN. COALESCE thành sentinel không an toàn nếu sentinel có thể là khóa hợp lệ.

## Production implications và trade-offs
Chốt chính sách dữ liệu import, ràng buộc NOT NULL và FK nếu nghiệp vụ cho phép; theo dõi bản ghi orphan. So sánh execution plan/index của NOT EXISTS và anti join trên database thật. Không suy ra hiệu năng từ fixture 5 hàng.

## Senior engineer nên nhận ra
Cần phân biệt dữ liệu thiếu với không có phân bổ, xác minh semantics NULL và bổ sung regression cho NULL, trùng lặp, dữ liệu rỗng, không eligible và nhiều allocation.
