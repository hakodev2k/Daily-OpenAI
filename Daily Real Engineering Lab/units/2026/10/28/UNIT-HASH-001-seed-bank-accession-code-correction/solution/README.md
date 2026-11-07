# Reference Solution — chỉ đọc sau khi reproduce và tự fix

## 1. Symptoms
Sau correction, object hiển thị mã mới nhưng lookup thất bại, duplicate lọt vào registry và remove để sót bản ghi.

## 2. Evidence
Baseline xác nhận `FOUND_CORRECTED=False`, `DUPLICATE_ACCEPTED=True` và `REMAINING_COUNT=1`. Không có I/O hoặc database nên tập trung vào bất biến collection.

## 3. Root cause
`SeedAccession.GetHashCode()` và `Equals` phụ thuộc `Code`. `HashSet` đưa object vào bucket dựa trên mã cũ; mutation `Code` không tự tái lập bucket. Lookup bằng mã mới đi đến bucket khác. Đây là lỗi mutable hash key, không phải thiếu một dòng kiểm tra trong UI.

## 4. Why the fix works
Reference dùng `Dictionary<string, SeedAccession>` với key string bất biến. Khi đổi mã, kiểm tra record và collision, xóa key cũ, sửa object, rồi thêm key mới. Cả tra cứu, xóa và uniqueness dựa vào cùng một index. Trong bài lab single-thread, chuỗi thao tác này đủ; production concurrent cần atomicity mạnh hơn.

## 5. How to verify
Chỉ thay nội dung `starter/AccessionRegistry.cs` bằng reference code, giữ nguyên các file còn lại, chạy `./verify.ps1`. Bảy hợp đồng gồm initial lookup, correction, duplicate, remove, collision, missing/noop và reuse old code. `reproduce.ps1` vẫn xác nhận baseline cố định.

## 6. Alternative fixes
- Dùng identity bất biến (ID) làm hash key, duy trì index riêng cho mã có thể sửa.
- Thực hiện remove-before-mutate/re-add trên `HashSet` với collision check, rollback và giới hạn mutation ngoài registry.
- Dùng persistent unique constraint nếu mã được dùng chung giữa nhiều process.

## 7. Wrong or tempting fixes
- Thay `Contains` bằng `Any` chỉ che lỗi lookup; duplicate/remove vẫn sai.
- Chỉ gọi `Clear` hoặc tạo `HashSet` mới sau mỗi lần sửa có thể che triệu chứng nhưng tốn chi phí và dễ bỏ sót.
- Đổi comparer mà vẫn cho phép thay đổi trường ảnh hưởng hash không khôi phục invariant.
- Đổi sang `List` có thể đúng ở quy mô nhỏ nhưng cần tự bảo vệ uniqueness và chi phí tìm kiếm.

## 8. Production implications
Không cho bên ngoài mutate trực tiếp key của object đang nằm trong hash collection. Xác định API đổi mã có kiểm soát, audit trail và conflict policy.

## 9. Trade-offs
Dictionary key string đơn giản và O(1) trung bình, nhưng rename cần đồng bộ index và record. Một hệ thống nhiều writer phải có lock/transaction và chính sách uniqueness ở storage; reference chỉ là single-thread fixture.

## 10. Senior engineer should notice
Cần kiểm thử bất biến sau **chuỗi thao tác** chứ không chỉ từng method; test phải phát hiện duplicate, stale index, xóa không hết và khả năng tái sử dụng mã cũ. Khi có persistence, cần phân biệt business code có thể đổi với immutable surrogate identity.
