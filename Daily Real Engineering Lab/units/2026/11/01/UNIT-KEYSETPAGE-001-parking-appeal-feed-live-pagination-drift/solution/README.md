# Reference Solution — chỉ xem sau khi đã tự reproduce và thử sửa

## 1. Symptoms
Trang đầu trả `106,105,104`; khi hồ sơ 107 xuất hiện trước request tiếp theo, starter trả `104,103,102`. HTTP 200 và mỗi trang đủ ba item nhưng toàn phiên có ID 104 lặp và chưa thấy 101.

## 2. Evidence
Timeline chỉ ra insert nằm giữa hai request. Sort key ổn định, không có lỗi DB hay timeout. Phân trang phụ thuộc vị trí của hàng trong tập kết quả hiện tại; insert ở đầu làm dịch toàn bộ vị trí.

## 3. Root cause
`Skip(offset)` / SQL `OFFSET` tính theo vị trí trên **mỗi snapshot mới**, không theo ranh giới của trang trước. `offset=3` sau khi insert sẽ trỏ tới ID 104, dù ID này đã là item cuối của trang đầu. Delete trước offset gây bỏ sót tương tự.

## 4. Why the fix works
[Reference implementation](Program.cs) dùng seek/keyset: sort `CreatedAt DESC, Id DESC` rồi chỉ nhận bản ghi có cặp khóa **nhỏ hơn** item cuối của trang trước. Với timestamp trùng nhau, ID là tie-breaker. Cursor giữ hai khóa này; `Offset` được giữ trong record vì tương thích chữ ký nhưng không còn được dùng.

## 5. How to verify
Sau khi tự sửa `starter/Program.cs`, chạy `./scripts/verify.ps1`. Bộ check kiểm tra feed không thay đổi, insert, delete, tie timestamp, end-of-feed và input validation. Chạy `dotnet run --project ./solution/AppealFeed.Reference.csproj` chỉ để đối chiếu output; **verify không dùng solution**.

## 6. Alternative fixes
- Snapshot cố định hoặc server-side cursor có trạng thái: cần khi business bắt buộc nhìn đúng một tập dữ liệu tại thời điểm T, kể cả bản ghi bị xóa hoặc chỉnh sort key; tốn lưu trữ/TTL.
- Seek cursor stateless: hợp lý cho feed mới nhất, hiệu quả với index, nhưng không phải snapshot isolation.
- Offset + khóa tie-breaker: vẫn sai khi insert/delete giữa các trang; chỉ xử lý nondeterministic ties.

## 7. Wrong / tempting fixes
- Tăng `pageSize`: giảm xác suất nhưng không sửa contract.
- Dedupe trên frontend: che duplicate nhưng không tìm lại item bị bỏ sót.
- `ORDER BY CreatedAt, Id` với `OFFSET`: sort deterministic vẫn bị dịch vị trí.
- Lock toàn bảng giữa hai HTTP requests: giữ lock lâu, giảm throughput và tạo operational risk.

## 8. Production implications
SQL Server ví dụ cho trang tiếp theo:
```sql
SELECT TOP (@PageSize) Id, CreatedAt, Status
FROM dbo.ParkingAppeals
WHERE CreatedAt < @LastCreatedAt
   OR (CreatedAt = @LastCreatedAt AND Id < @LastId)
ORDER BY CreatedAt DESC, Id DESC;
```
Trang đầu bỏ `WHERE` cursor. Tạo index thích hợp trên `(CreatedAt DESC, Id DESC)`, kiểm tra execution plan, độ chọn lọc và policy filter. Cursor public nên được encode và bảo vệ chống sửa đổi nếu chứa tenant/filter/scope. Không dùng cursor của tenant này cho tenant khác.

## 9. Trade-offs
Keyset không hỗ trợ nhảy thẳng đến trang 500 theo số thứ tự. Khi sort key thay đổi, bản ghi có thể chuyển qua ranh giới; với yêu cầu snapshot tuyệt đối cần snapshot token hoặc versioned read model. Cần thống nhất hướng sắp xếp, NULL policy, timezone và tính duy nhất của tie-breaker.

## 10. What a Senior engineer should notice
Một endpoint trả HTTP 200 và trang dữ liệu hợp lệ không đủ chứng minh workflow đúng. Test phải mô phỏng **hai request với thay đổi ở giữa**; so sánh tập ID của cả phiên, không chỉ assert `Count==pageSize`. Tách correctness, consistency guarantee, performance và UX expectation.

## Regression checklist
Không lặp, không bỏ sót bản ghi ban đầu trong kịch bản insert/delete trước cursor; thứ tự tie đúng; kết thúc có `Next=null`; cursor thuộc cùng filter/sort/tenant; query có index; không suy diễn snapshot isolation từ keyset.
