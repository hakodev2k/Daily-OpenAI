# Reference solution — SPOILER

## Root cause
JSON cho phép parser đọc object chứa nhiều property cùng tên, nhưng các consumer có thể chọn giá trị đầu tiên hoặc cuối cùng. Trong replay, `EnumerateObject().First(...)` đọc `false`, còn `GetProperty(...)` đọc `true`. Audit và payout vì thế mâu thuẫn mà không phát sinh exception.

## Fix tại trust boundary
Duyệt object bằng `JsonDocument` trước khi gọi business logic; từ chối mọi tên thuộc tính trùng nhau theo `StringComparer.Ordinal`, bao gồm object lồng nhau. Giữ schema validation và authorization độc lập. Ví dụ thay `TryProcess` bằng:

```csharp
public static bool TryProcess(string json, out (bool audit, bool payout) result)
{
    result = default;
    try
    {
        using var doc = JsonDocument.Parse(json);
        if (!UniqueNames(doc.RootElement)) return false;
        result = Process(json);
        return true;
    }
    catch (JsonException) { return false; }
    catch (InvalidOperationException) { return false; }
    catch (KeyNotFoundException) { return false; }
}

private static bool UniqueNames(JsonElement element)
{
    if (element.ValueKind == JsonValueKind.Array)
        return element.EnumerateArray().All(UniqueNames);
    if (element.ValueKind != JsonValueKind.Object) return true;
    var names = new HashSet<string>(StringComparer.Ordinal);
    foreach (var property in element.EnumerateObject())
        if (!names.Add(property.Name) || !UniqueNames(property.Value)) return false;
    return true;
}
```

## Verification
`./verify.ps1` phải PASS; bổ sung test duplicate trong object lồng nhau, key phân biệt hoa/thường, JSON thiếu `approved`, và input quá lớn. Dùng giới hạn request body, `JsonDocumentOptions.MaxDepth` và schema validation cho production.

## Wrong fixes
- Chỉ sửa audit đọc giá trị cuối: che giấu payload mơ hồ, không bảo đảm downstream đồng nhất.
- Chỉ thêm try/catch: không có exception trong đường đi lỗi.
- Dựa vào thứ tự property: thứ tự không phải chính sách authorization.

## Trade-offs
Reject-at-ingress có thể phá tương thích với client cũ đang gửi duplicate keys; cần telemetry, rollout và hợp đồng API rõ ràng. Không dùng canonicalization ngầm để che lỗi input.
