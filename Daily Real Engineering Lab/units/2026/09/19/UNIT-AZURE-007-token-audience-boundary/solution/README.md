# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Identity provider cấp access token thành công, Bearer token có mặt, token chưa hết hạn, nhưng Invoice API trả `401`.

## 2. Evidence
Starter in audience mà API quan sát. Giá trị này không khớp audience mà Invoice API chấp nhận. Không có evidence cho thấy token acquisition thất bại hay token đã expire.

## 3. Root cause
Client acquire token bằng scope của một resource khác. Token hợp lệ về mặt phát hành nhưng không dành cho Invoice API, nên resource server từ chối token.

## 4. Why the fix works
Đổi token request boundary để scope trỏ tới resource của Invoice API:

```csharp
const string requestedScope = "api://invoice-service/.default";
```

Fake identity provider ánh xạ scope này thành audience `api://invoice-service`, đúng với validation contract của API.

## 5. How to verify
Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả phải có `http-status=200` và `VERIFY_PASS`.

## 6. Alternative fixes
Trong hệ thống thật, resource identifier có thể là Application ID URI hoặc scope cụ thể do API expose. Cấu hình nên bind rõ downstream client với scope tương ứng thay vì dùng một scope global cho mọi dependency.

## 7. Wrong or misleading fixes
- Tắt audience validation ở API: làm yếu security boundary thay vì sửa client.
- Retry `401`: token sai audience sẽ không tự trở thành đúng sau retry.
- Tăng token lifetime: token chưa expire nên không giải quyết nguyên nhân.
- Gửi token của Azure Resource Manager tới mọi API nội bộ: token được cấp không đồng nghĩa token hợp lệ cho resource khác.

## 8. Production implications
Service-to-service authentication cần quan sát cả acquisition và validation. Log an toàn nên cho biết downstream resource, status và classification của auth failure; không log raw access token.

## 9. Trade-offs
Centralized token acquisition giảm duplication nhưng abstraction phải giữ resource/scope boundary rõ ràng. Một helper quá generic dễ khiến token của resource A bị tái sử dụng cho resource B.

## 10. What a Senior engineer should notice
`401` sau khi acquire token thành công là dấu hiệu cần kiểm tra contract giữa issuer, client và resource server: audience, scope, issuer, expiry và authorization requirements. Không nên mặc định lỗi nằm ở Managed Identity hay network chỉ vì token đã được cấp.
