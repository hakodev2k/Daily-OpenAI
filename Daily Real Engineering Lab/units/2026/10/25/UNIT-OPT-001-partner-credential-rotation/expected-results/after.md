# Sau khi sửa

Chạy `scripts/verify.ps1` trên **starter đã được bạn sửa**:
- Request v1 vẫn Accepted.
- Sau rotation v2: provider=v2, control partner=Accepted, gateway=Accepted, sentVersion=v2.
- Sau rotation v3: provider=v3, control partner=Accepted, gateway=Accepted, sentVersion=v3.
- Credential version v1 không được đối tác chấp nhận sau rotation.
- `VERIFICATION_PASSED`, exit code 0.

Kiểm tra regression: không chỉnh harness, không hard-code v2/v3, không tạo instance gateway mới giữa các lần gửi. `reproduce.ps1` không còn báo `INCIDENT_REPRODUCED` sau khi fix — đây là hành vi đúng.
