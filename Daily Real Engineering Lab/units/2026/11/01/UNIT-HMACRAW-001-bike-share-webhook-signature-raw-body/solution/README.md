# Reference Solution — spoiler

## Symptoms
Một biên nhận có chữ ký hợp lệ bị từ chối sau khi đối tác thay đổi cách trình bày JSON.

## Evidence
Contract ký chính xác raw UTF-8 bytes của compact, pretty và reordered JSON. Starter chỉ từ chối mẫu pretty; tampered payload vẫn bị từ chối.

## Root cause
Starter deserialize rồi serialize JSON trước khi tính HMAC. Byte sequence đã thay đổi (ví dụ whitespace), trong khi chữ ký được tạo trên original bytes.

## Why the fix works
Reference tính `HMACSHA256.HashData(_key, receivedBody)` trên **byte nhận được**, không serialize lại. So sánh bằng `CryptographicOperations.FixedTimeEquals`; từ chối hex sai và body quá lớn.

## How to verify
`scripts/verify.ps1` kiểm tra starter learner đã sửa. `scripts/check-solution.ps1` kiểm tra reference độc lập. Reproduce phải thất bại sau fix vì triệu chứng cũ không còn.

## Alternative fixes
Canonical JSON chỉ hợp lệ nếu protocol hai phía định nghĩa chính thức và triển khai thống nhất. Với hợp đồng raw-body hiện tại, đây không phải thay thế tương thích.

## Wrong fixes
Tắt signature verification; `Trim()` body; sort property; so sánh digest bằng string equality — các lựa chọn này hoặc phá contract hoặc giảm an toàn.

## Production implications
Đọc raw request body có giới hạn, verify trước khi parse; sau đó validate schema, enforce replay window, authorize partner, quản lý key rotation, tránh log secret và PII.

## Trade-offs
Raw-body signing đơn giản, chính xác và tương thích nhưng format-sensitive; canonical signing có thể giúp serializer interoperability với chi phí phức tạp và thay đổi giao thức.

## What a Senior engineer should notice
Phân biệt transport, message authentication, JSON schema, replay, business idempotency và observability. Không giải quyết incident bằng cách bỏ security gate.
