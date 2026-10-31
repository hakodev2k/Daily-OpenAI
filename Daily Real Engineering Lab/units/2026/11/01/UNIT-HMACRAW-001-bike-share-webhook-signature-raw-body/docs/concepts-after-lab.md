# Concepts after lab

- JSON cùng ý nghĩa có thể có các byte khác nhau: whitespace, escapes, property order.
- HMAC-SHA256 xác thực byte payload bằng shared secret, không xác thực semantic JSON object.
- Xác thực signature không thay thế schema validation, authorization, replay prevention hoặc idempotency.
- Dùng constant-time digest comparison, giới hạn body, không log shared secret.
- ASP.NET Core middleware đọc body cần buffering/rewind có giới hạn để verifier và parser cùng đọc được.
- Nếu giao thức quy định canonical JSON thì hai bên phải dùng **cùng** specification; không tự ý canonicalize ở một phía.

Senior extension: thiết kế timestamp/nonce được ký, replay window và key rotation mà không làm hỏng contract hiện tại.
