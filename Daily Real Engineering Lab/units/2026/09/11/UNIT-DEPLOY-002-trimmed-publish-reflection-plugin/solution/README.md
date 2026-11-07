# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

`dotnet run` xử lý được formatter đã cấu hình, nhưng artifact self-contained được publish với `PublishTrimmed=true` không thể tạo formatter đó.

## 2. Evidence

Business input và file cấu hình không đổi. Điểm khác biệt quan trọng là publish pipeline chạy IL trimming. Formatter chỉ được chọn bằng tên type đọc từ JSON runtime, nên static analysis không có một reference code path rõ ràng tới implementation.

## 3. Root cause

`CompactReportFormatter` chỉ được discover bằng runtime reflection từ chuỗi cấu hình. Trimmer tối ưu theo reachability và có thể loại bỏ type không có static reference/preservation contract. Local untrimmed build vẫn chứa type nên tạo được object; trimmed artifact thì không.

## 4. Why the fix works

Một fix dễ bảo trì là thay type-name reflection bằng explicit registry. Ví dụ thay `plugin.json` bằng:

```json
{"formatter":"compact"}
```

và thay runtime lookup bằng:

```csharp
public sealed record PluginSettings(string Formatter);

var settings = JsonSerializer.Deserialize<PluginSettings>(File.ReadAllText(configPath))
    ?? throw new InvalidOperationException("Invalid plugin configuration.");

IReportFormatter formatter = settings.Formatter switch
{
    "compact" => new CompactReportFormatter(),
    _ => throw new InvalidOperationException($"Unknown formatter '{settings.Formatter}'.")
};

Console.WriteLine(formatter.Format("acme"));
return 0;
```

Static reference tới `CompactReportFormatter` làm ownership và deployment contract rõ ràng, đồng thời tránh coupling configuration vào assembly-qualified type name.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Script publish chính `starter/` với trimming vẫn bật rồi chạy artifact. Thành công khi output chính xác là `FORMATTED:ACME` và exit code bằng `0`.

## 6. Alternative fixes

Nếu runtime plugin discovery là requirement thật sự, có thể dùng trim annotations/preservation metadata phù hợp, source-generated registration, hoặc plugin assembly boundary được deployment rõ ràng. Chọn giải pháp dựa trên mức dynamic thực sự cần thiết, không chỉ để giữ nguyên implementation cũ.

## 7. Wrong / tempting fixes

- **Tắt trimming**: có thể hợp lệ nếu size/startup không quan trọng, nhưng nó né regression thay vì chứng minh architecture hiện tại trim-safe.
- **Catch exception rồi fallback formatter**: che symptom và có thể silently thay business behavior.
- **Copy thêm DLL một cách thủ công**: không giải quyết reachability contract nếu type nằm trong assembly đang bị trim; đồng thời làm pipeline khó kiểm soát.
- **Giữ mọi type bằng wildcard**: có thể chạy nhưng làm mất phần lớn lợi ích trimming và tạo maintenance debt.

## 8. Production implications

Deployment optimization là một phần của runtime contract. CI nên test **published artifact**, không chỉ test `dotnet run`/Debug build. Với trimming/AOT, reflection, serializers, DI scanning và plugin discovery đều cần review riêng.

## 9. Trade-offs

Explicit registry đơn giản, dễ quan sát và trim-safe nhưng yêu cầu deploy lại khi thêm formatter. Dynamic plugin discovery linh hoạt hơn nhưng đòi hỏi preservation contract, versioning, isolation và deployment validation chặt hơn.

## 10. What a Senior engineer should notice

Regression không nằm trong business logic mà ở khoảng cách giữa **development execution model** và **production artifact model**. Senior engineer cần đưa publish configuration vào test boundary và giảm các runtime assumptions mà build tooling không thể chứng minh.