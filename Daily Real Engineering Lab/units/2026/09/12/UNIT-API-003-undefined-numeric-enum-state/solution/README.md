# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

API contract document dùng string enum (`"Pending"`, `"Paid"`, `"Shipped"`, `"Cancelled"`), nhưng payload `{"status":999}` vẫn bind thành `OrderStatus` và endpoint trả success.

## 2. Evidence

Starter cấu hình `JsonStringEnumConverter()` nhưng converter mặc định vẫn cho phép integer values. C# enum có thể chứa underlying integer không tương ứng với member được khai báo, nên strongly typed binding không tự đảm bảo domain validity.

## 3. Root cause

`JsonStringEnumConverter` được dùng với `allowIntegerValues` mặc định là `true`. JSON number `999` vì vậy được deserialize thành `(OrderStatus)999`, dù enum không có member nào mang giá trị đó.

## 4. Why the fix works

Nếu public wire contract yêu cầu enum names dạng string, cấu hình converter để không nhận integer values:

```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
});
```

Khi đó JSON number bị từ chối ngay tại serialization boundary thay vì chảy vào domain workflow.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Script kiểm tra đồng thời:

- `{"status":"Shipped"}` vẫn trả `200`.
- `{"status":999}` trả HTTP 4xx.

## 6. Alternative fixes

Nếu contract cố ý cho phép numeric enum, hãy validate tập giá trị bằng `Enum.IsDefined` hoặc custom validation trước khi state đi vào domain. Với public APIs cần versioning nghiêm túc, request DTO cũng có thể dùng string rồi parse/validate rõ ràng để tách wire contract khỏi enum implementation.

## 7. Wrong or misleading fixes

- Thêm `default` branch downstream và tiếp tục lưu `999`: chỉ che symptom, invalid state vẫn tồn tại.
- Cast về `int` rồi clamp vào khoảng 0–3: biến input sai thành một business state khác mà client không yêu cầu.
- Chỉ sửa mobile client: integration khác vẫn có thể gửi payload ngoài contract.
- Chỉ kiểm tra JSON syntax: payload này vốn là JSON hợp lệ.

## 8. Production implications

Invalid enum values có thể đi qua persistence, event payload, cache và analytics trước khi lỗi xuất hiện. Vì vậy contract validation càng gần ingress boundary càng giảm blast radius.

## 9. Trade-offs

`allowIntegerValues: false` là lựa chọn gọn nhất khi contract công khai dùng string enum. Nếu hệ thống đã có legacy numeric clients, thay đổi này có thể breaking; khi đó cần explicit compatibility policy, validation và migration timeline.

## 10. What a Senior engineer should notice

Type safety của C# không đồng nghĩa với domain safety ở serialization boundary. Senior engineer cần phân biệt wire representation, deserialization semantics, domain invariant và backward compatibility trước khi chọn fix.