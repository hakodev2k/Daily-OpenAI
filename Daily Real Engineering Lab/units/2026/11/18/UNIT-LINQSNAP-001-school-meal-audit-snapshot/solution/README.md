# Reference Solution — chỉ đọc sau khi tự sửa
## Symptoms
Tổng số là 2 nhưng danh sách chi tiết có 3 bản ghi.
## Evidence
Callback thêm bản ghi thỏa điều kiện giữa hai lần đọc; Count và ToList cho kết quả khác nhau.
## Root cause
LINQ Where trên IEnumerable thực thi trì hoãn. Mỗi terminal operation enumerate nguồn một lần.
## Why the fix works
Materialize một snapshot duy nhất trước callback, sau đó tính count và trả chi tiết từ snapshot đó.
## Reference patch
Thay thân Export bằng:
```csharp
var snapshot = source.Where(x => x.Portions >= 20).ToList();
var reportedCount = snapshot.Count;
afterSummary();
return new AuditReport(reportedCount, snapshot);
```
## How to verify
Chạy pwsh ./verify.ps1; hai trường bằng 2, sourceCount bằng 3, exit 0.
## Alternative fixes
Dùng array snapshot hoặc immutable collection. Với nhiều database queries, cân nhắc snapshot isolation.
## Wrong or misleading fixes
Count lại sau callback chỉ che vấn đề; AsEnumerable không materialize; khóa từng phép đọc không đảm bảo toàn bộ report nhất quán.
## Production implications
Định nghĩa mốc snapshot, nhất là audit và reconciliation.
## Trade-offs
Materialize tăng RAM; snapshot transaction tăng chi phí lưu phiên bản và có thể kéo dài transaction.
## What a Senior engineer should notice
Cần test concurrent mutation và consistency của toàn bộ report, không chỉ test một trường.
