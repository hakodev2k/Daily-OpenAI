# Reference Solution

## Symptoms
Hai reservation cùng đọc `Quantity=10`, cùng kết luận đủ hàng, cùng báo thành công, nhưng final quantity chỉ phản ánh một lần trừ.

## Root cause
Business invariant được kiểm tra ở application dựa trên dữ liệu đã đọc trước đó. Giữa read và write tồn tại race window, nên hai request có thể ra quyết định trên cùng một snapshot cũ và ghi đè lẫn nhau.

## Fix
Đưa điều kiện đủ hàng vào cùng database write operation. Một cách điển hình là cập nhật số lượng theo delta và chỉ cho phép update khi quantity hiện tại vẫn đủ. Dùng affected-row count để quyết định reservation thành công hay thất bại.

Ví dụ logic:

```csharp
var cmd = connection.CreateCommand();
cmd.CommandText = "UPDATE Inventory SET Quantity = Quantity - $amount WHERE Sku = 'SKU-1' AND Quantity >= $amount;";
cmd.Parameters.AddWithValue("$amount", amount);
var affected = await cmd.ExecuteNonQueryAsync();
return affected == 1;
```

## Why it works
Kiểm tra điều kiện và thay đổi dữ liệu được thực hiện như một atomic database statement. Writer thứ hai đánh giá điều kiện trên state mới nhất, nên không thể cùng thành công nếu inventory còn lại không đủ.

## Verification
Chạy `verify.ps1`. Kết quả mong đợi: `SUCCESS_COUNT=1` và `FINAL_QUANTITY=3`.

## Wrong fixes
- Thêm `Task.Delay` khác: chỉ thay đổi xác suất race.
- Retry toàn bộ flow mà vẫn giữ read-modify-write tách rời: race vẫn tồn tại.
- Dùng lock in-process khi service có thể chạy nhiều instance: không bảo vệ cross-process concurrency.
- Chỉ kiểm tra quantity lần nữa ở application mà không gắn check với write: vẫn còn race window.

## Trade-offs
Atomic conditional update đơn giản và hiệu quả cho invariant cục bộ trên một row. Với workflow phức tạp hơn có thể cần optimistic concurrency token, transaction isolation phù hợp, stored procedure hoặc domain-specific reservation model.

## Senior takeaway
Khi correctness phụ thuộc vào state hiện tại của database, hãy đặt câu hỏi liệu invariant có thể được enforce tại nơi state được ghi hay không. Read-modify-write ở application là một dấu hiệu cần xem xét concurrency semantics kỹ lưỡng.
