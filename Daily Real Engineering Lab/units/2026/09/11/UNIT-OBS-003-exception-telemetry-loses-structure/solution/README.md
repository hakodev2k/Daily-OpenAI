# Reference Solution — UNIT-OBS-003

> Chỉ xem sau khi đã reproduce và thử sửa.

## 1. Symptoms

Worker tạo error log và rendered message vẫn chứa thông tin exception, nhưng logging provider nhận `exception == null`.

## 2. Evidence

`CaptureLogger<T>` ghi riêng rendered message và `Exception?` được truyền vào `ILogger.Log`. Starter cho thấy message tồn tại nhưng exception channel rỗng.

## 3. Root cause

Starter gọi overload `LogError(string message, params object?[] args)` và đưa `ex` vào message-template arguments. Vì vậy exception chỉ là một formatting value, không phải exception parameter riêng của logging abstraction.

## 4. Why the fix works

Dùng overload nhận `Exception` trước message template:

```csharp
_logger.LogError(ex, "Payment capture failed for {OrderId}", orderId);
```

Provider giờ nhận exception object riêng, đồng thời `OrderId` vẫn là structured business context.

## 5. How to verify

Thay `starter/PaymentWorker.cs` bằng implementation tương đương rồi chạy:

```powershell
./verify.ps1
```

Verification chỉ PASS khi learner-editable starter giữ `InvalidOperationException` trong structured exception channel và vẫn giữ `OrderId` trong message.

## 6. Alternative fixes

- Source-generated logging với `LoggerMessage` có thể chuẩn hóa event contract ở codebase lớn.
- Một wrapper logging nội bộ có thể bắt buộc exception-aware API cho các failure event quan trọng.

Các lựa chọn này chỉ có giá trị khi giảm lỗi contract mà không che mất semantics của `ILogger`.

## 7. Wrong or misleading fixes

- Chỉ gọi `ex.ToString()` rồi concatenate vào message: text có thể giàu hơn nhưng exception vẫn không có structured channel.
- Tăng log level lên Critical: không sửa telemetry shape.
- Bổ sung thêm text stack trace property rồi bỏ exception parameter: có thể hữu ích cho một sink đặc thù nhưng không thay thế exception semantics chuẩn.

## 8. Production implications

Structured exception data thường được telemetry backend dùng cho exception grouping, type, stack, failure drill-down và correlation. Rendered text đơn thuần làm những khả năng này kém tin cậy hơn.

## 9. Trade-offs

Giữ exception object giúp diagnosis tốt hơn nhưng cần chính sách logging phù hợp để tránh ghi dữ liệu nhạy cảm từ exception/message. Structured context cũng cần cardinality hợp lý.

## 10. What a Senior engineer should notice

- Logging API là một data contract với provider, không chỉ là cách in string.
- Test observability quan trọng nên kiểm tra event shape, không chỉ `message.Contains(...)`.
- Refactor logging cần giữ cả business context và diagnostic semantics.