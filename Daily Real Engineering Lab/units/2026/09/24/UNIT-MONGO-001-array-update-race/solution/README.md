# Reference Solution — inspect only after reproducing and attempting your own fix

## 1. Symptoms
Hai concurrent request đều báo thành công nhưng final team có thể thiếu một member.

## 2. Evidence
Cả hai request đọc cùng một document version, sửa hai bản copy độc lập rồi lần lượt replace toàn bộ document. Write sau cùng ghi đè state được tạo bởi write trước.

## 3. Root cause
Read-modify-replace không tạo một atomic boundary cho mutation của array. Application đang gửi stale full-document snapshot trở lại store.

## 4. Why the fix works
Trong MongoDB thật, biểu diễn intent trực tiếp bằng atomic update trên document, ví dụ filter theo `_id` và dùng `$addToSet` cho membership. Server áp dụng mutation tại document boundary thay vì client replace một snapshot cũ. `$addToSet` cũng làm operation thêm member trở nên idempotent đối với cùng value.

Ví dụ với MongoDB .NET Driver:
```csharp
var filter = Builders<Team>.Filter.Eq(x => x.Id, teamId);
var update = Builders<Team>.Update.AddToSet(x => x.Members, userId);
await collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
```

Trong simulator của lab, sửa `TeamStore` để expose một operation `AddMemberAtomicAsync` khóa mutation boundary bên trong store và sửa `TeamService` gọi operation đó thay vì Read + Replace.

## 5. How to verify
Chạy `verify.ps1`. Final state phải chứa `owner`, `alice`, `bob`; gọi thêm `alice` lần nữa không được tạo duplicate.

## 6. Alternative fixes
Optimistic concurrency bằng version filter + retry có thể phù hợp khi mutation cần validate nhiều field dựa trên snapshot. Transaction có thể cần khi invariant trải qua nhiều documents/collections, nhưng không cần chỉ để atomic update một document.

## 7. Wrong / misleading fixes
- Thêm `Task.Delay` hoặc đổi timing: chỉ làm race khó thấy hơn.
- Serialize mọi request bằng lock trong một API instance: không bảo vệ khi scale-out nhiều instances.
- Retry full-document replace mà không có version predicate: vẫn có thể ghi stale state.
- Dùng transaction mặc định cho một single-document mutation: tăng complexity khi MongoDB đã có atomicity ở document level.

## 8. Production implications
Lost update có thể trở thành lỗi authorization/membership khó audit vì HTTP response vẫn success. Cần metrics cho matched/modified counts và correlation IDs cho write intent.

## 9. Trade-offs
Atomic operators đơn giản, hiệu quả nhưng cần document model phù hợp. Versioned optimistic concurrency linh hoạt hơn cho complex invariants nhưng yêu cầu conflict handling rõ ràng.

## 10. What a Senior engineer should notice
Đừng bắt đầu từ “cần lock nào?”. Hãy xác định invariant và atomicity boundary, rồi chọn storage primitive biểu diễn business mutation nhỏ nhất có thể.