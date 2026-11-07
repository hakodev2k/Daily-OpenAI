# Reference Solution — inspect only after reproducing and attempting your own fix

## Symptoms
Payload chỉ chứa `displayName`, nhưng `phoneNumber` đang tồn tại bị xóa.

## Evidence
`System.Text.Json` tạo DTO với `PhoneNumber == null` cả khi property bị omitted. Update logic sau đó gán giá trị này vào entity.

## Root cause
Contract partial-update cần phân biệt ít nhất hai trạng thái cho mỗi field: **omitted** và **present**. Nullable DTO property chỉ giữ value, không giữ presence, nên omitted và explicit `null` bị collapse thành cùng runtime value.

## Why the fix works
Preserve field presence. Một cách gọn cho lab này là deserialize payload thành `JsonDocument`/custom patch model, kiểm tra `TryGetProperty`, rồi chỉ mutate field khi property thực sự xuất hiện. Nếu API cho phép clear phone, property present với JSON `null` có thể được map thành chủ đích clear.

Pseudo-flow:
```csharp
if (root.TryGetProperty("phoneNumber", out var phone))
    customer.PhoneNumber = phone.ValueKind == JsonValueKind.Null ? null : phone.GetString();
```

Trong production, có thể dùng một explicit optional-field type hoặc JSON Patch nếu semantics phù hợp; điều quan trọng là contract phải giữ được presence.

## How to verify
- Payload chỉ có `displayName`: phone giữ nguyên.
- Payload có `phoneNumber: null`: clear phone nếu contract cho phép.
- Payload có `phoneNumber: "..."`: thay phone.
- Các field không có trong payload không đổi.

## Alternative fixes
- `Optional<T>`/`PatchField<T>` có converter giữ `IsSpecified`.
- JSON Patch (`application/json-patch+json`) khi operation semantics phù hợp.
- Endpoint command chuyên biệt nếu update intents ít và rõ.

## Wrong / tempting fixes
- `if (request.PhoneNumber != null)`: tránh accidental clear nhưng đồng thời làm mất khả năng explicit clear.
- Đổi nullable annotation: compiler nullability không lưu JSON property presence.
- Load lại entity sau update: chỉ che symptom, không sửa contract.

## Production implications
Partial update semantics phải được document và test ở boundary. OpenAPI/client generation cũng cần biểu diễn đúng optional-vs-nullable; nếu không bug có thể chuyển từ server sang generated client.

## Trade-offs
Generic patch model linh hoạt nhưng tăng abstraction. Command-specific endpoint rõ intent hơn nhưng nhiều endpoint/type hơn. JSON Patch chuẩn hóa operations nhưng cần validation/authorization ở từng path.

## What a Senior engineer should notice
Đây là lỗi information loss tại serialization boundary, không chỉ là một câu `if`. Fix tốt phải bảo toàn business intent xuyên qua wire format → DTO/patch representation → domain mutation → tests.
