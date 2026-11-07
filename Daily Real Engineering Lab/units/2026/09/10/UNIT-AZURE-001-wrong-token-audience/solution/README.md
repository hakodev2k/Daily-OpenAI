# Reference Solution — UNIT-AZURE-001

> Chỉ xem sau khi đã reproduce và thử fix.

## Symptoms

Credential cấp token thành công nhưng fake Blob resource trả `401`.

## Evidence

Starter in ra `TOKEN_AUDIENCE=https://management.azure.com/` trong khi resource đích yêu cầu audience của Azure Storage.

## Root cause

Worker yêu cầu access token cho sai target resource. Scope dùng resource của Azure Resource Manager thay vì Azure Storage, nên token hợp lệ nhưng audience không khớp resource server.

## Why the fix works

Yêu cầu token với scope của resource đích khiến token được phát hành cho đúng audience. Trong lab, đổi `configuredScope` thành `https://storage.azure.com/.default`.

## How to verify

Chạy:

```powershell
./verify.ps1
```

Kết quả phải có `DOWNSTREAM_STATUS=200` và `LAB_VERIFY_PASS`.

## Alternative fixes

- Nếu dùng Azure SDK client (`BlobServiceClient`), ưu tiên để SDK phối hợp `TokenCredential` và service client thay vì tự dựng Authorization header.
- Với custom protected API, dùng scope/audience được API registration công bố thay vì hard-code một Azure resource khác.

## Wrong or misleading fixes

- Gán thêm RBAC role mà không kiểm tra token audience: role đúng không sửa được token gửi tới sai resource.
- Retry `401`: lỗi deterministic configuration/authentication không trở thành đúng nhờ retry.
- Chuyển sang connection string để né lỗi: có thể làm request chạy nhưng thay đổi security model và che mất root cause.
- Tắt validation ở resource: không phải fix hợp lệ.

## Production implications

Khi điều tra `401`, tách rõ ba lớp: credential có lấy được token không, token có dành cho đúng resource không, và principal có quyền cần thiết không. Ba failure mode này cần evidence khác nhau.

## Trade-offs

Dùng service-specific Azure SDK giảm lỗi scope thủ công nhưng tăng dependency vào SDK tương ứng. Custom HTTP client cho nhiều quyền kiểm soát hơn nhưng buộc team hiểu rõ OAuth resource/scope contract.

## What a Senior engineer should notice

`Token acquisition success` chỉ chứng minh identity provider đã cấp một token cho request đã gửi. Nó không chứng minh downstream resource sẽ chấp nhận token đó. Audience/resource boundary phải được kiểm tra trước khi đổ lỗi cho RBAC, network hay retry policy.

## Real Azure mapping

Với Azure Identity, `TokenCredential` nhận scope thông qua `TokenRequestContext`; pattern phổ biến là `{resource}/.default`. Azure-hosted apps có thể dùng `ManagedIdentityCredential`, còn local development thường dùng `DefaultAzureCredential`.
