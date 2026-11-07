# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms

`GetAmounts` return bình thường. Khi caller bắt đầu enumerate, hai amount đầu được tạo ra rồi `FormatException` thoát ra ngoài. Contract `ImportDataException` không được giữ.

## Evidence

Dòng `Service returned. Enumeration starts now.` xuất hiện trước exception. Điều này chứng minh việc gọi `GetAmounts` và việc thực thi projection không cùng execution boundary.

## Root cause

`Enumerable.Select` sử dụng deferred execution. Trong starter, `try/catch` chỉ bao quanh việc tạo iterator. `ParseAmount` chưa chạy tại thời điểm `return rows.Select(ParseAmount)` hoàn tất. Nó chạy sau đó khi caller enumerate, lúc stack frame chứa `catch (FormatException)` đã kết thúc.

## Reference fix

Một fix phù hợp với contract hiện tại là materialize kết quả bên trong `try`:

```csharp
public IEnumerable<decimal> GetAmounts(IEnumerable<string> rows)
{
    try
    {
        return rows.Select(ParseAmount).ToArray();
    }
    catch (FormatException ex)
    {
        throw new ImportDataException("Import contains an invalid amount.", ex);
    }
}
```

Sau khi sửa `starter/Program.cs`, chạy:

```powershell
./verify.ps1
```

## Why the fix works

`ToArray()` buộc toàn bộ projection chạy trước khi method rời `try`. Vì vậy lỗi parsing phát sinh khi `catch` vẫn còn hiệu lực và được translate đúng contract.

## Alternative fixes

- Nếu cần streaming thật sự, caller có thể sở hữu exception boundary khi enumerate; khi đó API contract phải nói rõ điều này.
- Có thể trả một abstraction khác biểu diễn kết quả từng record, ví dụ success/error per item, nếu batch cần tiếp tục sau row lỗi.
- Iterator method có thể tự đặt `try/catch` quanh logic chạy trong quá trình enumeration, nhưng thiết kế phải rõ ownership và semantics.

## Wrong / tempting fixes

- Thêm `try/catch` chỉ quanh lời gọi `GetAmounts` ở caller nhưng vẫn enumerate bên ngoài: vẫn không bao phủ thời điểm failure.
- Catch `Exception`: mở rộng phạm vi lỗi nhưng không thay đổi execution boundary và dễ che lỗi lập trình khác.
- Bỏ exception translation: có thể đơn giản hơn trong một contract khác, nhưng không đáp ứng contract của lab này.

## Production implications

Deferred execution ảnh hưởng không chỉ exception handling mà còn lifetime của `DbContext`, stream, transaction, file handle và mutable state. API trả `IEnumerable<T>` có thể vô tình chuyển execution ownership sang caller.

Materialization cũng có trade-off: dùng thêm memory và mất streaming. Với dataset lớn, hãy chọn boundary dựa trên contract, kích thước dữ liệu và failure semantics thay vì áp dụng `ToArray()` máy móc.

## What a Senior engineer should notice

Senior engineer cần phân biệt **method-call boundary** với **execution boundary**. Khi API trả deferred sequence, code nhìn như nằm trong service chưa chắc thực thi trong service. Exception ownership, resource lifetime và observability phải được thiết kế theo thời điểm code thực sự chạy.
