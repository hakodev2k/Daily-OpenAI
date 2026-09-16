# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms
Refresh báo thành công nhưng deterministic timeline kết thúc với session bị xóa.

## 2. Evidence
Expiry actor quyết định trên version 1. Refresh sau đó tạo version 2 và TTL mới, nhưng delete không kiểm tra rằng state hiện tại vẫn là state đã được scan.

## 3. Root cause
Một mutation dựa trên stale observation được áp dụng vô điều kiện lên state mới hơn. Refresh/value/TTL và expiry decision không chia sẻ một concurrency invariant đủ mạnh.

## 4. Why the fix works
Delete phải chỉ thành công nếu version hiện tại vẫn bằng version mà expiry actor đã quan sát. Tương tự, trong hệ thống thật, refresh nên cập nhật các field thuộc cùng invariant trong một atomic boundary. Version check ngăn stale actor xóa state mới.

## 5. How to verify
Sửa `starter/` để stale expiry delete bị từ chối khi version đã thay đổi. `./verify.ps1` yêu cầu final state chứa `value=v2` và `expiresAt=10`.

## 6. Alternative fixes
- Redis transaction với optimistic condition.
- Lua script thực hiện read/check/write atomically.
- Thiết kế lại data model để TTL là thuộc tính của chính key và không cần custom expiry worker nếu requirement cho phép.

## 7. Wrong or misleading fixes
- Thêm `Task.Delay`: chỉ đổi xác suất interleaving.
- Retry refresh sau khi session mất: có thể che symptom nhưng không bảo vệ invariant và có thể hồi sinh stale data.
- Global lock trong mọi process: khó đúng trong distributed deployment nếu lock không thực sự distributed và ownership không được bảo vệ.

## 8. Production implications
Race có thể hiếm nhưng gây logout ngẫu nhiên hoặc mất cache state. Khi dùng Redis thật, cần hiểu atomicity của từng command và boundary giữa nhiều command; network round-trip giữa các bước mở rộng race window.

## 9. Trade-offs
Lua/transaction tăng độ phức tạp nhưng đưa invariant về server-side atomic boundary. Đơn giản hóa data model có thể tốt hơn nếu native TTL semantics đã đủ cho business requirement.

## 10. What a Senior engineer should notice
Concurrency correctness không nằm ở việc từng command có atomic hay không; nó nằm ở invariant xuyên qua cả workflow. Một decision được tạo từ snapshot cũ phải được validate trước khi mutate state hiện tại.