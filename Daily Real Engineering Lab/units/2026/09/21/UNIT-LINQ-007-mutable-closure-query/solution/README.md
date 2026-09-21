# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Query được khai báo với ngưỡng 150 nhưng khi enumerate sau một thay đổi option, export chứa cả invoice 180.

## 2. Evidence
`Where` tạo deferred sequence. Predicate đóng trên local variable `maximumAmount`; delegate đọc giá trị của variable khi enumeration xảy ra.

## 3. Root cause
Report vô tình phụ thuộc vào mutable captured variable xuyên qua deferred-execution boundary. Query definition không snapshot giá trị 150; closure giữ variable và giá trị đã đổi thành 250 trước khi predicate chạy.

## 4. Why the fix works
Reference solution copy option sang `reportMaximumAmount` dành riêng cho report trước khi tạo query. Biến snapshot đó không bị phase sau thay đổi, nên enumeration giữ contract của report boundary.

Nếu business contract yêu cầu snapshot cả nguồn dữ liệu tại boundary, materialize kết quả tại đó (`ToArray`/`ToList`) có thể phù hợp hơn. Hai lựa chọn giải quyết hai contract khác nhau.

## 5. How to verify
Chạy `verify.ps1` trên learner-editable `starter/`. Kết quả phải chỉ có IDs 1 và 2 dù `maximumAmount` dùng cho report kế tiếp đổi thành 250.

## 6. Alternative fixes
- Truyền filter value dưới dạng immutable parameter vào một method tạo report.
- Materialize tại report boundary nếu cần snapshot cả source lẫn filter.
- Thiết kế report request immutable chứa toàn bộ criteria.

## 7. Wrong or misleading fixes
- Đưa assignment `maximumAmount = 250` xuống sau enumeration chỉ phụ thuộc ordering và dễ tái phát.
- Gọi `ToList()` ở một vị trí tùy ý có thể che symptom nhưng thay đổi memory/performance và snapshot semantics mà chưa xác định contract.
- Đổi LINQ sang vòng `foreach` không tự giải quyết mutable state nếu vẫn đọc cùng biến tại thời điểm muộn.

## 8. Production implications
Deferred execution rất hữu ích nhưng lifetime của query có thể vượt lifetime logic mà developer tưởng tượng. Captured mutable state làm query behavior phụ thuộc timeline, đặc biệt trong pipelines, callbacks và asynchronous workflows.

## 9. Trade-offs
Snapshot criteria giữ deferred source evaluation nhưng cố định filter. Materialization snapshot cả result, đơn giản reasoning hơn nhưng tăng allocation và có thể xử lý dữ liệu sớm hơn cần thiết.

## 10. What a Senior engineer should notice
Câu hỏi không chỉ là “LINQ có deferred execution không”, mà là business boundary cần cố định thứ gì: criteria, source snapshot, hay cả hai. Fix nên thể hiện contract đó rõ ràng thay vì chỉ làm test pass.