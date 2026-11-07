# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Cycle đầu đúng; cycle sau vẫn quan sát state đã được giữ từ cycle trước.

## 2. Evidence
Cùng `CatalogSession` identity xuất hiện qua nhiều cycle và giá trị đầu tiên tiếp tục được trả về.

## 3. Root cause
Worker có lifetime dài giữ trực tiếp một scoped/stateful dependency. Processing cycle mới không tạo unit-of-work boundary mới, nên state vốn chỉ phù hợp trong một scope bị kéo dài theo worker.

## 4. Why the fix works
Giữ `IServiceScopeFactory` trong worker và tạo một scope cho mỗi cycle. Resolve `CatalogSession` bên trong scope đó rồi dispose scope khi cycle kết thúc. Mỗi cycle nhận state container độc lập.

Pseudo-shape:
```csharp
public sealed class CatalogWorker
{
    private readonly IServiceScopeFactory _scopeFactory;
    public CatalogWorker(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public void RunCycle(int cycle, string databaseVersion)
    {
        using var scope = _scopeFactory.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<CatalogSession>();
        var observed = session.Read(databaseVersion);
        Console.WriteLine($"cycle={cycle}; database={databaseVersion}; observed={observed}; session={session.GetHashCode()}");
    }
}
```

## 5. How to verify
`verify.ps1` phải thấy cycle 1 -> v1, cycle 2 -> v2 và hai session identity khác nhau.

## 6. Alternative fixes
Tách processing operation thành một scoped handler và resolve handler mỗi cycle. Với EF Core, `IDbContextFactory<TContext>` cũng có thể phù hợp khi cần tạo context theo operation.

## 7. Wrong / tempting fixes
- Restart worker định kỳ: chỉ reset symptom.
- Clear tracked state thủ công sau mỗi cycle: có thể giảm một biểu hiện nhưng vẫn để lifecycle contract sai.
- Đăng ký mọi dependency thành singleton: làm state sharing rõ hơn chứ không tạo unit-of-work boundary.

## 8. Production implications
Với `DbContext`, giữ context quá lâu có thể gây stale tracking, memory growth và conflict khó đoán. Boundary nên phản ánh business operation thay vì lifetime của process.

## 9. Trade-offs
Scope-per-cycle thêm một lượng nhỏ lifecycle overhead nhưng tạo ownership/disposal rõ ràng. Nếu một cycle quá lớn, có thể cần scope nhỏ hơn theo message/batch transaction boundary.

## 10. Senior engineer should notice
DI lifetime không chỉ là syntax đăng ký service; nó mô tả ownership, state isolation và disposal boundary. Hosted service là singleton-like long-lived component nên dependency graph cần được xem xét theo operation boundary.