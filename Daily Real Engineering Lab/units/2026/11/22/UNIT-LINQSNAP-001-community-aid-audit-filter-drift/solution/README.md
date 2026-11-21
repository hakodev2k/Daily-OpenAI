# Reference Solution — chỉ đọc sau khi tự điều tra

Nguồn lỗi là deferred execution: `Where` tạo truy vấn, không tạo snapshot. `DeferredList` triển khai `IReadOnlyList` nhưng mỗi `Count`, index và enumeration đều thực hiện lại truy vấn. Predicate còn tham chiếu trực tiếp tới đối tượng `ExportFilter` có thể thay đổi; nguồn dữ liệu cũng có thể thay đổi.

Reference lưu giá trị tenant/status tại ranh giới nghiệp vụ rồi materialize thành mảng bất biến về membership bằng `ToArray()`. Kiểm thử cả thay đổi filter và thay đổi source sau Prepare, cùng phép đọc lặp lại.

Không sửa bằng cách gọi `ToList()` chỉ khi render cuối cùng: lúc đó dữ liệu đã drift. Không chỉ sao chép filter mà vẫn trả deferred sequence: source mutation vẫn ảnh hưởng. Không cache toàn cục theo tenant vì export có tính thời điểm và có thể lộ dữ liệu giữa yêu cầu.

Trade-off: snapshot tốn O(n) bộ nhớ cho kết quả. Với export cực lớn, cần snapshot/transaction hoặc cursor ổn định ở storage layer thay vì giữ toàn bộ trong RAM. `ToArray` chỉ cố định membership của record bất biến; nếu record mutable, cần deep copy hoặc immutable DTO.
