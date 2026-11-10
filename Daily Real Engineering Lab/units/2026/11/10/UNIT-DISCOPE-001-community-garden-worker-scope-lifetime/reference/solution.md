# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Batch đầu được ghi, batch sau ném `ObjectDisposedException`.

## Evidence
Worker là singleton; constructor tạo scope rồi dispose trước khi Process được gọi. Repository thuộc scope đó.

## Root cause
Một scoped service bị giữ làm field của singleton sau khi scope tạo ra nó đã kết thúc. Tắt `ValidateScopes` không sửa được quyền sở hữu đối tượng.

## Corrected implementation
Thay **chỉ** class `IrrigationInvoiceWorker` trong `starter/Program.cs` bằng:

```csharp
sealed class IrrigationInvoiceWorker
{
    private readonly IServiceScopeFactory _scopeFactory;
    public IrrigationInvoiceWorker(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    public void Process(string batch)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<InvoiceRepository>();
        repository.Commit(batch);
    }
}
```

## Why it works
Mỗi batch sở hữu scope riêng; repository không tồn tại vượt khỏi scope. Worker chỉ giữ factory có vòng đời phù hợp.

## Verification
Chạy `./reproduce.ps1` trước khi sửa; sau sửa chạy `./verify.ps1`. Verify phải đi qua **starter đã sửa**, không chạy reference riêng. Đối chiếu ledger có chính xác hai ID theo thứ tự.

## Alternatives
Với xử lý bất đồng bộ, dùng `CreateAsyncScope` và `await using` nếu dependency triển khai `IAsyncDisposable`. Với worker xử lý song song, tạo scope riêng cho từng work item, không chia sẻ DbContext.

## Wrong / tempting fixes
- Đăng ký repository singleton: có thể làm hết exception nhưng phá vỡ isolation và thread-safety.
- Không dispose scope: che triệu chứng bằng leak.
- Catch rồi bỏ qua ObjectDisposedException: mất batch và dữ liệu.
- Tạo một scope sống suốt thời gian worker: vẫn chia sẻ state giữa các batch.

## Production implications
Bật `ValidateScopes` khi xây container để phát hiện lifetime mismatch có thể phát hiện tĩnh; test nhiều batch, cancellation và shutdown. Trong production dùng idempotency key nếu retry có thể tạo side effect trùng.

## Trade-offs
Scope mỗi batch có chi phí tạo nhỏ nhưng tăng isolation và dễ cleanup. Scope per item phù hợp khi transaction gắn với từng batch; nếu cần atomicity xuyên nhiều batch, phải thiết kế transaction boundary riêng.

## Senior engineer should notice
Không đánh đồng việc dependency resolve thành công với việc sử dụng an toàn sau đó. Cần xác định ownership, disposal và concurrency contract; test ít nhất hai lượt xử lý thay vì happy path một lượt.
