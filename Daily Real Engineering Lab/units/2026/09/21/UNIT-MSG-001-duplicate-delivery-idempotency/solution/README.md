# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Một logical message được delivery lại và tạo side effect lần nữa.

## 2. Evidence
Hai deliveries có cùng stable identity nhưng shipment count tăng hai lần.

## 3. Root cause
Consumer không có idempotency boundary gắn business side effect với stable message identity.

## 4. Why the fix works
Lưu identity đã hoàn tất và bỏ qua redelivery của cùng identity. Trong lab có thể dùng in-memory set; production cần durable storage.

## 5. How to verify
Hai deliveries của message đầu chỉ tạo một shipment, trong khi message identity khác vẫn tạo shipment.

## 6. Alternative fixes
Persistent inbox với unique constraint hoặc downstream operation idempotent bằng business key.

## 7. Wrong or misleading fixes
Tắt retry làm suy yếu reliability. Deduplicate chỉ theo OrderId có thể chặn event hợp lệ. In-memory state không đủ cho restart hoặc scale-out.

## 8. Production implications
Dedup record và side effect cần consistency strategy để xử lý crash giữa các bước.

## 9. Trade-offs
Persistent inbox tăng storage và transaction complexity nhưng cung cấp durable idempotency.

## 10. What a Senior engineer should notice
At-least-once delivery khiến duplicate delivery là điều kiện bình thường. Cần xác định stable identity, side-effect boundary, atomicity và crash behavior.