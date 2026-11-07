# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms
Một statement vi phạm constraint được catch, nhưng statement hợp lệ tiếp theo hoặc `COMMIT` vẫn thất bại.

## 2. Evidence
Starter mô phỏng PostgreSQL semantics: sau statement error, transaction chuyển sang aborted state. C# exception handler tiếp tục loop nhưng transaction không tự trở lại usable state.

## 3. Root cause
Failure boundary của application và transaction boundary không khớp nhau. Application coi lỗi một row là recoverable, nhưng không thiết lập database recovery boundary trước statement đó. PostgreSQL yêu cầu rollback transaction hoặc rollback tới savepoint phù hợp trước khi tiếp tục commands.

## 4. Why the fix works
Tạo savepoint trước mỗi row có thể bị bỏ qua. Nếu insert thất bại theo policy cho phép, rollback tới savepoint để loại bỏ failed statement state rồi tiếp tục. Các thay đổi hợp lệ trước savepoint vẫn nằm trong outer transaction.

Pseudo-fix cho starter:
```csharp
foreach (var row in rows)
{
    tx.CreateSavepoint();
    try
    {
        tx.Insert(row);
        Console.WriteLine($"IMPORTED {row}");
    }
    catch (ConstraintViolationException ex)
    {
        tx.RollbackToSavepoint();
        Console.WriteLine($"SKIPPED {row}: {ex.Message}");
    }
}
```

## 5. How to verify
Chạy `./verify.ps1`. Learner-editable starter phải in `COMMIT OK`, `PERSISTED A,B`, và không còn command nào thất bại vì transaction aborted.

## 6. Alternative fixes
- Mỗi row một transaction riêng nếu atomicity toàn batch không cần thiết.
- Validate trước khi insert nếu validation có thể phản ánh chính xác constraint, nhưng vẫn phải xử lý race/constraint errors từ DB.
- Staging table + set-based validation cho import lớn thường hiệu quả hơn per-row processing.

## 7. Wrong / misleading fixes
- Chỉ `catch` exception: chỉ xử lý C# control flow, không repair DB transaction state.
- Retry statement ngay trong transaction aborted: command tiếp theo vẫn bị từ chối.
- Rollback toàn transaction cho mọi invalid row: có thể đúng nếu business yêu cầu all-or-nothing, nhưng sai với requirement hiện tại là giữ các row hợp lệ.

## 8. Production implications
Savepoint per row có overhead. Với batch lớn, cần cân nhắc staging/set-based approach, error reporting, transaction duration, lock lifetime và retry semantics.

## 9. Trade-offs
Partial success làm import semantics phức tạp hơn: phải định nghĩa row nào được skip, lỗi nào phải abort toàn batch, và cách audit/retry. Savepoint là mechanism, không thay thế business policy.

## 10. What a Senior engineer should notice
Exception boundary, transaction boundary và business atomicity là ba khái niệm khác nhau. Code chỉ đúng khi chúng được thiết kế nhất quán. Một `catch` không có nghĩa resource/subsystem đã trở lại state an toàn để tiếp tục.