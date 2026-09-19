# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Request hoàn tất, không exception, nhưng `CreditHold` vẫn là `false` trong persisted state. Log đồng thời cho thấy cùng account ID xuất hiện dưới hai in-memory instance khác nhau.

## 2. Evidence
Hai repository đang sở hữu hai persistence scope. Mỗi scope có identity map riêng, vì vậy cùng row `Account(10)` được materialize thành hai object. Business step sửa instance A, còn `SaveChanges` cuối operation chỉ flush instance B.

## 3. Root cause
Trong EF Core thật, identity resolution và change tracking là phạm vi của một `DbContext`. Việc mỗi repository tự tạo `DbContext` cho các bước thuộc cùng một transactional business operation làm mất một tracking/identity boundary thống nhất. Đây không phải lỗi vì hai object có cùng primary key; lỗi nằm ở ownership/lifetime của Unit of Work.

## 4. Why the fix works
Tạo một persistence scope cho toàn bộ operation và inject cùng scope đó vào các repository tham gia. Trong simulator, thay hai dòng tạo scope bằng một instance dùng chung:

```csharp
var operationScope = new PersistenceScope(store);
var customerRepositoryScope = operationScope;
var accountRepositoryScope = operationScope;
```

Hai lần `Load(10)` lúc này trả cùng tracked instance. Mutation và flush cùng đi qua một identity map.

Trong ASP.NET Core + EF Core, cách tương ứng thường là một scoped `DbContext` cho request/application-command boundary, thay vì repository tự `new DbContext()` hoặc tự tạo scope tùy ý.

## 5. How to verify
Chạy `./verify.ps1`. Kết quả phải có `VERIFY_PASS`, persisted hold là `true`, và hai repository path phải quan sát cùng object identity trong operation.

## 6. Alternative fixes
Nếu workflow cố ý cần nhiều `DbContext`, hãy chuyển dữ liệu qua explicit DTO/command boundary và attach/update với concurrency policy rõ ràng. Với operation dài, có thể dùng explicit transaction/outbox/workflow thay vì giữ một `DbContext` sống quá lâu.

## 7. Wrong or misleading fixes
- Gọi `SaveChanges` trên cả hai context có thể làm symptom biến mất nhưng không giải quyết ownership và có thể tạo lost update/order-dependent behavior.
- `Attach` object từ context khác một cách mù quáng dễ gây conflict hoặc overwrite field không mong muốn.
- Biến repository thành singleton để “dùng chung context” làm lifetime sai nghiêm trọng hơn.
- Bỏ tracking toàn bộ không tự giải quyết transactional consistency.

## 8. Production implications
Cross-context object graphs thường tạo lỗi khó thấy: stale state, duplicate tracked instances, overwrite, transaction boundary không rõ và test pass ở happy path nhưng fail khi nhiều repository phối hợp.

## 9. Trade-offs
Một `DbContext` scoped theo operation đơn giản và dễ reasoning, nhưng không nên sống quá lâu hoặc track graph quá lớn. Multi-context architecture có thể đúng khi boundary thực sự độc lập, miễn consistency và transaction semantics được thiết kế explicit.

## 10. What a Senior engineer should notice
Repository Pattern không được phép che mất Unit of Work boundary. Câu hỏi quan trọng là ai sở hữu `DbContext`, lifetime của nó khớp business transaction nào, và các aggregate/repository tham gia có đang vô tình dùng các identity map độc lập hay không.
