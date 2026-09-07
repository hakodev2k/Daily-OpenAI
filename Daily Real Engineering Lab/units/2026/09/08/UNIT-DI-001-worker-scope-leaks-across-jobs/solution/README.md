# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

Worker xử lý đủ ba job nhưng cả ba dòng log có cùng `ContextId`.

## Evidence

- `JobId` thay đổi theo từng iteration.
- `ContextId` không thay đổi.
- Không có exception và handler vẫn chạy đủ ba lần.

## Root cause

DI scope được tạo một lần cho toàn batch, nên `OrderReconciliationHandler` và `JobContext` scoped cũng chỉ được resolve một lần. Lifetime thực tế của dependency vì vậy kéo dài qua nhiều job, dù business processing boundary yêu cầu mỗi job độc lập.

## Fix tham chiếu

Đưa scope creation và dependency resolution vào processing boundary của từng job:

```csharp
foreach (var jobId in jobs)
{
    using var jobScope = provider.CreateScope();
    var handler = jobScope.ServiceProvider
        .GetRequiredService<OrderReconciliationHandler>();

    handler.Handle(jobId);
}
```

## Vì sao fix hoạt động

Mỗi iteration tạo một DI scope mới. `OrderReconciliationHandler` và `JobContext` scoped được tạo lại trong scope đó rồi dispose khi job kết thúc. Lifetime kỹ thuật lúc này khớp với unit-of-work boundary của business operation.

## Cách verify

Chạy:

```powershell
./verify.ps1
```

Script yêu cầu ba job vẫn được xử lý và ba `ContextId` phải khác nhau.

## Alternative fixes

- Nếu worker dùng `BackgroundService`, inject `IServiceScopeFactory` rồi tạo scope bên trong mỗi lần xử lý message/job.
- Với pipeline phức tạp, có thể đóng gói `ProcessOneJobAsync` để scope boundary rõ ràng và dễ test.
- Nếu dependency thật sự nên tồn tại xuyên batch, đổi lifetime chỉ khi semantics của dependency cho phép; không đổi lifetime chỉ để làm test pass.

## Wrong / tempting fixes

### Đổi `JobContext` thành transient

Có thể tạo instance mới thường xuyên hơn nhưng không giải quyết đúng ownership boundary. Các scoped dependency khác như `DbContext` vẫn có thể bị dùng xuyên nhiều job.

### Reset state thủ công trong `JobContext`

Che triệu chứng nhưng giữ nguyên lifetime sai. Khi dependency tích lũy thêm state/resource, lỗi sẽ quay lại dưới dạng khác.

### Tạo ServiceProvider mới cho từng job

Quá nặng và phá composition root. Scope là primitive đúng cho unit-of-work lifetime; không cần dựng lại toàn container.

### Tắt `validateScopes`

Không thay đổi behavior của scope hiện tại và làm giảm khả năng phát hiện lifetime misuse khác.

## Production implications

Scope quá dài trong background processing có thể làm state theo request/job bị rò sang operation kế tiếp, giữ `DbContext` quá lâu, tăng change tracker, giữ connection/resource lâu hơn dự kiến và tạo correlation/logging sai.

## Trade-offs

Scope-per-job tạo thêm object/lifetime bookkeeping nhưng chi phí thường nhỏ so với lợi ích isolation. Với batch cực nhỏ và dependency stateless, scope-per-batch có thể hợp lệ nếu đó thực sự là business unit of work; quyết định phải dựa trên semantics chứ không dựa trên convenience.

## Điều Senior engineer nên nhận ra

`Scoped` không định nghĩa business boundary. Container chỉ bảo đảm “một instance trong một scope”; engineer phải chủ động đặt scope boundary đúng với request, message, job hoặc transaction lifecycle của hệ thống.
