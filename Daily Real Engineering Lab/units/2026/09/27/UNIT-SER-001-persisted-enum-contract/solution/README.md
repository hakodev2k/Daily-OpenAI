# Reference Solution — inspect only after reproducing and attempting your own fix

## 1. Symptoms
Historical JSON deserialize không throw nhưng một persisted order được hiểu thành business state khác sau refactor.

## 2. Evidence
Fixture lịch sử chứa numeric status `1`. Model hiện tại gán numeric value theo thứ tự declaration mới, nên cùng wire value không còn giữ business meaning cũ.

## 3. Root cause
Persisted numeric enum representation đã trở thành một wire/storage contract ngoài ý muốn. Refactor enum thay đổi implicit numeric assignments, nhưng historical data vẫn mang meaning của contract cũ.

## 4. Why the fix works
Reference solution gán explicit stable numeric values tương thích dữ liệu đã tồn tại và đưa state mới vào value chưa sử dụng. Nhờ đó historical payload giữ nguyên meaning thay vì phụ thuộc declaration order.

## 5. How to verify
Chạy `verify.ps1`. Historical fixture phải deserialize thành `Paid` và process exit code phải bằng 0.

## 6. Alternative fixes
- Dùng dedicated persistence DTO với explicit mapping thay vì serialize domain enum trực tiếp.
- Chuyển sang string discriminator/versioned contract cho dữ liệu mới và duy trì migration reader cho legacy payload.
- Thực hiện data migration có versioning nếu storage cho phép và rollout có thể kiểm soát.

## 7. Wrong or misleading fixes
- Chỉ reorder enum để test hiện tại pass mà không ghi rõ stable values: một refactor sau có thể tái tạo lỗi.
- Catch exception: không giúp vì payload hợp lệ và deserialize thành công.
- Xóa historical snapshots: làm mất khả năng resume/audit và né tránh contract migration.

## 8. Production implications
Persisted payload, event, cache entry và message có thể sống lâu hơn version code tạo ra chúng. Những representation tưởng là implementation detail có thể trở thành compatibility contract khi vượt process boundary.

## 9. Trade-offs
Explicit numeric values giữ compatibility tốt nhưng cần discipline và reserved values. String contracts dễ đọc hơn nhưng payload lớn hơn và vẫn cần versioning/rename policy. Dedicated DTO tách domain tốt nhất nhưng tăng mapping code.

## 10. What a Senior engineer should notice
Trước refactor model được persist hoặc publish, phải inventory các externalized representations, xác định compatibility window và thiết kế migration/reader strategy. Refactor compile-success không đồng nghĩa production-safe.