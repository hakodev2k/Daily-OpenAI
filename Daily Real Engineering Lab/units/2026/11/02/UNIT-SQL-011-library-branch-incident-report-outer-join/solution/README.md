# Reference Solution — spoiler

**Chỉ đọc sau khi reproduce và thử sửa `starter/BranchReportRepository.cs`.** [Source tham chiếu](BranchReportRepository.cs).

## 1. Symptoms

Báo cáo chỉ có Central mặc dù bảng chi nhánh chứa bốn dòng. Tổng số sự cố nghiêm trọng đang mở là 2, không sai; vấn đề là tính đầy đủ của chiều dữ liệu chi nhánh.

## 2. Evidence

Seed tạo West chỉ có sự cố `Minor`, North không có sự cố, East chỉ có sự cố `Closed`. Cả ba biến mất. Vì `COUNT(i.Id)` chỉ tính những sự cố ghép được, việc tổng số sự cố vẫn đúng không chứng minh tập chi nhánh đúng.

## 3. Root cause

`LEFT JOIN` ban đầu có thể tạo hàng có các cột `i.*` bằng `NULL` cho chi nhánh không có sự cố. Nhưng `WHERE i.Status = 'Open' AND i.Severity = 'Critical'` được áp dụng sau phép join, loại những hàng `NULL` đó (điều kiện nhận giá trị UNKNOWN), đồng thời loại những chi nhánh chỉ có sự cố không phù hợp. Hiệu ứng tương đương inner join đối với tập hàng báo cáo.

## 4. Why the fix works

Chuyển điều kiện của sự cố cần đếm vào phần `ON` của `LEFT JOIN`. Như vậy mọi chi nhánh vẫn có ít nhất một hàng kết quả, còn các sự cố không phù hợp không tham gia ghép. `COUNT(i.Id)` cho 0 khi không có bản ghi sự cố ghép được; **không thay bằng `COUNT(*)`**, vì `COUNT(*)` đếm cả hàng null-extended.

## 5. How to verify

Chỉ copy nội dung truy vấn thích hợp vào file starter (không cần sửa checks). Chạy `./verify.ps1`. Harness tham chiếu duy nhất `starter/Starter.csproj`, không biên dịch code ở `solution/`. Kiểm tra bốn chi nhánh ban đầu, chi nhánh mới chỉ có sự cố Closed, rồi một sự cố Open/Critical mới tại West.

## 6. Alternative fixes

- Dùng correlated subquery đếm sự cố theo từng chi nhánh: dễ đọc cho dữ liệu nhỏ, nhưng phải xem execution plan và index ở dữ liệu lớn.
- Aggregate các sự cố phù hợp theo `BranchId` trong derived table/CTE rồi `LEFT JOIN` kết quả vào `Branches`: thường dễ mở rộng khi cần nhiều chỉ số.
- Dùng conditional aggregation như `SUM(CASE WHEN ... THEN 1 ELSE 0 END)` với join rộng: linh hoạt cho nhiều loại đếm nhưng phải kiểm soát duplicate rows và null handling.

## 7. Wrong / tempting fixes

- Thêm `OR i.Id IS NULL` vào WHERE: chỉ cứu được North (không có sự cố), vẫn làm mất East và West vì họ có bản ghi nhưng không thỏa điều kiện.
- Dùng `COUNT(*)`: tạo kết quả 1 cho chi nhánh không có sự cố phù hợp nếu outer join giữ hàng null-extended.
- Bỏ hết bộ lọc: làm số sự cố tăng do tính cả `Closed` và `Minor`.
- Tự thêm chi nhánh thiếu ở frontend: che lỗi query, gây hai nguồn sự thật và khó giữ đồng bộ khi dữ liệu đổi.

## 8. Production implications

Dashboard compliance phải phân biệt 0 với 'không được trả về'. Một chi nhánh biến mất có thể làm người quản lý hiểu sai coverage. Cần kiểm thử ít nhất: không có bản ghi con, chỉ có bản ghi con không phù hợp, có nhiều bản ghi phù hợp, và dữ liệu thay đổi. Với filter động, dùng parameterized SQL; cân nhắc index trên `(BranchId, Status, Severity)` và đo query plan thực tế trước khi tối ưu.

## 9. Trade-offs

Đặt predicate trong `ON` giữ đúng contract và là sửa đổi nhỏ nhất. CTE/derived table có thể dễ kiểm soát khi tổng hợp nhiều metric, đổi lại query dài hơn. Conditional aggregation tránh một số join nhưng có thể tạo sai số nếu join thêm bảng 1-n. Chọn theo correctness trước, sau đó đo logical reads/latency ở dữ liệu thật.

## 10. What a Senior engineer should notice

Kiểm thử phải bảo vệ **tập chiều** (all branches) chứ không chỉ tổng aggregate. SQL có three-valued logic; predicate placement là một phần của data contract. Phân biệt `COUNT(*)`, `COUNT(column)` và `SUM(CASE ...)`, kiểm soát duplicate joins, lập regression cho zero-case, và không dùng tổng số đúng để kết luận toàn bộ báo cáo đúng.
