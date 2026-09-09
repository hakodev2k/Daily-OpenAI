# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Hai test đều pass khi chạy riêng. Khi chạy đồng thời qua controlled parallel gate, ít nhất một test nhận provider do test còn lại vừa ghi và assertion fail.

## 2. Evidence

- Failure chỉ xuất hiện khi test lifetime overlap.
- Mỗi test tạo `RenewalRouter` riêng nhưng cả hai router đọc cùng `RenewalProviderSettings.Provider`.
- `Provider` là mutable `static` property nên thuộc process/AppDomain state, không thuộc test instance.

## 3. Root cause

Production code lấy routing configuration từ mutable global state. Test suite thay đổi global state để tạo hai scenario khác nhau. Khi xUnit chạy test song song, các write cạnh tranh và kết quả phụ thuộc thứ tự interleaving.

Đây không phải lỗi của xUnit parallelism. Parallel execution chỉ làm lộ dependency boundary không có isolation.

## 4. Why the fix works

Reference solution biến provider selection thành constructor dependency. Mỗi test tạo một immutable `RenewalOptions` riêng rồi truyền vào router riêng. Không còn mutable process-wide configuration để test khác có thể ghi đè.

```csharp
public sealed record RenewalOptions(string Provider);

public sealed class RenewalRouter
{
    private readonly RenewalOptions _options;

    public RenewalRouter(RenewalOptions options)
    {
        _options = options;
    }

    public string SelectProvider(string subscriptionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        return _options.Provider;
    }
}
```

Mỗi test dùng:

```csharp
var router = new RenewalRouter(new RenewalOptions("Stripe"));
```

hoặc:

```csharp
var router = new RenewalRouter(new RenewalOptions("Legacy"));
```

## 5. How to verify

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

Script vẫn bật controlled parallel gate. Cả hai test phải pass trong cùng run.

## 6. Alternative fixes

- Inject một interface như `IRenewalProviderSelector` nếu production logic thực sự cần strategy động.
- Dùng `IOptions<T>`/`IOptionsSnapshot<T>` trong ASP.NET Core khi configuration ownership phù hợp với application lifetime. Test vẫn nên tạo options riêng cho từng scenario.
- Nếu state là resource thật sự phải shared, dùng fixture/lifecycle rõ ràng và thiết kế assertions quanh shared contract thay vì giả vờ rằng state độc lập.

## 7. Wrong / Tempting Fixes

### Disable test parallelization

Có thể làm suite xanh nhưng giữ nguyên global mutable dependency. Sau này state vẫn có thể leak giữa test theo thứ tự chạy hoặc giữa production operations.

### Add `lock` around each write

`lock` chỉ serialize thao tác ghi. Test có thể release lock rồi test khác ghi giá trị trước khi router đọc, nên ownership problem chưa được giải quyết.

### Retry failed tests in CI

Giảm signal quality và biến nondeterminism thành chi phí pipeline. Retry có thể phù hợp với external transient dependency trong integration testing, nhưng không phải primary fix cho deterministic shared-state interference.

### Reset static state in `Dispose`

Giúp một số sequential tests nhưng không tạo isolation khi lifetimes overlap.

## 8. Production implications

Mutable static configuration cũng nguy hiểm ngoài test: request, background job hoặc tenant khác nhau có thể vô tình phụ thuộc vào cùng process-wide state. Explicit dependency giúp lifetime, ownership và thread-safety dễ reason hơn.

## 9. Trade-offs

Constructor dependency thêm một object/configuration boundary nhưng đổi lại testability và ownership rõ ràng. Không cần tạo abstraction interface nếu immutable options/value object đã đủ cho yêu cầu.

## 10. What a Senior engineer should notice

- Flaky test thường là architecture/lifecycle signal, không chỉ là test-runner nuisance.
- Test isolation phải được thiết kế, không đạt được bằng naming convention.
- Parallelism là một diagnostic amplifier hữu ích: nó phơi bày hidden shared state.
- Fix tốt loại bỏ nguyên nhân nondeterminism thay vì serialize hoặc retry nó.
