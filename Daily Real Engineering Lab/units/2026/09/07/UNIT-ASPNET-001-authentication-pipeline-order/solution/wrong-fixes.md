# Wrong Fixes

## 1. Cho phép mọi request `/secure`

Bỏ access gate làm triệu chứng biến mất nhưng đồng thời phá security boundary. Đây không phải fix.

## 2. Hard-code `context.User`

Tự gán `ClaimsPrincipal` trong access gate che giấu pipeline problem và duplicate authentication responsibility.

## 3. Retry request

Lỗi deterministic do pipeline order; retry không thay đổi request processing graph và chỉ tăng noise.

## 4. Chuyển `401` thành `200`

Đổi status code không làm identity xuất hiện. Verification phải kiểm tra cả status và username để tránh false positive.
