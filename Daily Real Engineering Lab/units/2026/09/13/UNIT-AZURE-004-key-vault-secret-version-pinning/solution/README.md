# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

- Security team đã rotate `PaymentApiKey` từ `v1` sang `v2`.
- Vault báo `v2` là version hiện hành.
- Application vẫn resolve `v1`.
- Payment provider chỉ chấp nhận credential mới và trả về `401` cho credential cũ.

## 2. Evidence

Starter in ra ba evidence quan trọng:

- configured secret reference chứa `/v1`;
- vault current version là `v2`;
- application resolved version vẫn là `v1`.

Điều này tách lỗi secret storage khỏi lỗi application reference/refresh contract.

## 3. Root cause

Application configuration pin một secret version cụ thể. Rotation tạo ra version mới nhưng không thay đổi reference đã cố định ở version cũ, vì vậy application tiếp tục đọc credential đã retired.

## 4. Why the fix works

Reference solution sử dụng logical/versionless secret reference và resolve secret tại một refresh boundary rõ ràng. Khi vault current version đổi, lần refresh tiếp theo lấy version hiện hành thay vì tiếp tục giữ version đã pin.

Trong Azure thật, cơ chế cụ thể phụ thuộc cách application tích hợp Key Vault. Ví dụ, SDK có thể lấy secret theo name mà không truyền version; App Service Key Vault references cũng có thể dùng reference không pin version. Điều quan trọng là phải thiết kế cả **version selection** và **refresh behavior**, không chỉ đổi nơi lưu secret.

## 5. How to verify

Sau khi sửa `starter/`:

```powershell
./verify.ps1
```

Kết quả mong đợi:

- resolved version là `v2`;
- simulated provider trả `200`;
- process exit code là `0`.

## 6. Alternative fixes

### Update pinned version during deployment

Có thể giữ version pinning nếu tổ chức muốn deployment artifact/config xác định chính xác secret version. Khi rotate, pipeline phải cập nhật reference và redeploy/reload một cách có kiểm soát.

Ưu điểm: deterministic rollback/audit. Nhược điểm: rotation phụ thuộc deployment coordination.

### Versionless reference plus explicit cache/refresh policy

Phù hợp khi secret rotation cần được application quan sát mà không cần deploy code/config mới. Cần định nghĩa refresh interval, cache behavior và failure handling.

### Dual-credential overlap

Một số provider hỗ trợ old/new credential cùng tồn tại trong thời gian chuyển tiếp. Cách này giảm risk khi rollout nhưng không thay thế việc sửa application reference contract.

## 7. Wrong or misleading fixes

### Retry `401` nhiều lần

Không giải quyết việc application đang gửi cùng một retired credential.

### Restart process mà không thay đổi reference

Có thể tạo cảm giác đang “refresh config”, nhưng nếu config vẫn pin `v1` thì restart vẫn resolve `v1`.

### Hard-code credential mới

Che symptom nhưng phá secret-management boundary, rotation workflow và auditability.

### Giữ secret cũ active vô thời hạn

Có thể tạm khôi phục service nhưng làm mất mục tiêu rotation và kéo dài exposure window.

## 8. Production implications

Secret rotation là một end-to-end operational contract gồm:

- secret version lifecycle;
- application reference semantics;
- cache/refresh boundary;
- rollout order;
- downstream acceptance window;
- monitoring sau rotation;
- rollback/recovery plan.

## 9. Trade-offs

Version pinning không tự động sai. Nó hữu ích khi reproducibility và deterministic rollout quan trọng. Nhưng nếu chọn pinning, rotation phải được orchestration cùng config/deployment update.

Versionless lookup giảm coordination nhưng yêu cầu application hiểu refresh semantics và failure behavior khi secret mới chưa usable hoặc access policy có vấn đề.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi “Key Vault có secret mới chưa?”. Họ xác định toàn bộ đường đi:

`rotation event → secret version → application reference → refresh/cache → credential actually sent → downstream acceptance`.

Điểm cần thiết kế là contract giữa các bước này, bao gồm telemetry để chứng minh application đang dùng secret version nào mà không log secret value.
