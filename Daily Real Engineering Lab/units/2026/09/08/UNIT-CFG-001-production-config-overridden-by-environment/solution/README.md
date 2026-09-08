# Reference Solution — chỉ xem sau khi đã reproduce và tự thử sửa

## Symptoms
Versioned production configuration chứa `https://payments.prod.example/`, nhưng effective runtime value lại là `https://payments.staging.example/`.

## Evidence
`GetDebugView()` cho thấy `Payments:BaseUrl` được cung cấp bởi `EnvironmentVariablesConfigurationProvider`, không phải provider chứa versioned production value.

## Root cause
Deployment vẫn giữ một environment variable cũ: `Payments__BaseUrl=https://payments.staging.example/`. Environment provider được thêm sau versioned configuration nên override thắng theo configuration precedence của ứng dụng.

## Fix đúng
Xóa stale environment override khỏi deployment input. Trong lab, hãy xóa dòng sau khỏi `starter/deployment.env`:

```text
Payments__BaseUrl=https://payments.staging.example/
```

Sau đó runtime sẽ fallback về versioned production configuration.

## Why the fix works
Ta sửa **nguồn cấu hình sai** thay vì thay đổi precedence để che lỗi. Environment variables vẫn giữ vai trò override hợp lệ cho các setting cần được platform quản lý, nhưng deployment không còn chứa giá trị staging ngoài ý muốn.

## Verify
Chạy:

```powershell
./verify.ps1
```

Kết quả cần có:

```text
EFFECTIVE=https://payments.prod.example/
PASS: effective configuration uses the intended production endpoint.
```

## Wrong fixes
- Hard-code production URL trong business code.
- Đảo thứ tự provider để mọi JSON luôn thắng environment variables.
- Bỏ qua runtime effective value vì file trong repository “đúng”.
- Chỉ sửa log để in expected value thay vì giá trị thực sự được sử dụng.

## Production implications
Configuration drift có thể vượt qua build, test và health checks nếu dependency staging vẫn trả response hợp lệ. Các endpoint, connection string, feature flag và credential reference quan trọng nên có startup validation hoặc deployment verification phù hợp.

## Senior engineering takeaway
Configuration là một runtime composition problem. Khi điều tra, cần xác định **effective value + provenance + precedence**, không chỉ đọc một file cấu hình riêng lẻ.
