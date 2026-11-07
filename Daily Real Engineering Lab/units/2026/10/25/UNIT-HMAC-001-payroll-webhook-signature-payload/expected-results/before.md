# Before — starter chưa sửa

- dotnet run --project starter/PartnerWebhookLab.csproj -- reproduce trả exit code 0 khi tái hiện được lỗi.
- compact-valid: expected=True, actual=True.
- pretty-valid: expected=True, actual=False.
- spaced-valid: expected=True, actual=False.
- tampered-body, malformed-signature, truncated-signature: expected=False, actual=False.
- verify trả exit code 1 và VERIFY FAIL.

Nếu không thấy các dòng trên, kiểm tra đang chạy đúng thư mục, code starter chưa bị chỉnh sửa và dotnet SDK có thể build net8.0. Thời gian thực thi không phải tiêu chí pass/fail.
