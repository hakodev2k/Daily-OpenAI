# Reference Solution

> Chỉ xem sau khi đã reproduce và thử fix.

## Symptoms

Session document hết hạn vẫn tồn tại dù TTL index đã được tạo trên `ExpiresAt`.

## Evidence

Starter serialize `ExpiresAt` thành BSON `String`, không phải BSON `DateTime`.

## Root cause

TTL index chỉ có thể áp dụng expiration semantics khi indexed field chứa BSON date-compatible value. Một ISO-8601 string có thể trông giống timestamp nhưng không mang cùng storage type contract.

## Why the fix works

Đổi model sang UTC `DateTime` để MongoDB .NET driver serialize field thành BSON DateTime:

```csharp
sealed class SessionDocument
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
```

và gán:

```csharp
ExpiresAt = DateTime.UtcNow.AddMinutes(-30)
```

Sau serialization, `ExpiresAt` phải có `BsonType.DateTime`.

## How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

## Alternative fixes

- Dùng `DateTimeOffset` với serialization convention được xác định rõ và kiểm tra BSON output.
- Nếu business model buộc giữ string cho hiển thị, thêm field expiration riêng có kiểu BSON DateTime và đặt TTL index trên field đó.

## Wrong / tempting fixes

- Giảm `expireAfterSeconds`: không giúp nếu field không có storage type phù hợp.
- Parse string trong application rồi xóa thủ công ở mỗi request: chuyển retention policy thành coupling ở application path và tăng operational complexity.
- Chỉ kiểm tra JSON/log output: representation bên ngoài không chứng minh BSON type đã persist.

## Production implications

Data-retention policy là contract giữa schema thực tế và index semantics. Silent type drift có thể gây storage growth, tăng chi phí và giữ dữ liệu lâu hơn policy dự kiến.

## Trade-offs

UTC `DateTime` là lựa chọn đơn giản cho expiration instant. Nếu domain cần timezone gốc cho UX/audit, lưu timezone metadata riêng thay vì biến TTL field thành display string.

## What a Senior engineer should notice

- Schema-less database vẫn có schema contract ở runtime.
- Index existence không chứng minh index đang áp dụng đúng cho stored values.
- Khi retention không hoạt động, kiểm tra cả index definition lẫn BSON type của dữ liệu thực tế.
