> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

# Reference Solution

## Symptoms

- Retry với cùng `Idempotency-Key` và cùng payload trả lại receipt cũ, đúng kỳ vọng.
- Khi cùng key được dùng cho một payload khác, starter vẫn trả receipt của request đầu tiên.
- Payment provider chỉ được gọi một lần nên không có duplicate charge, nhưng response contract bị sai nghiệp vụ.

## Evidence

Điểm quan trọng không phải chỉ là `providerCalls=1`. Hãy so sánh payload của request thứ ba với `order` trong response. Response thuộc `ORDER-100` dù request mới là `ORDER-200`.

## Root cause

Idempotency store chỉ map `idempotencyKey -> response`. Nó không lưu identity/fingerprint của request gốc. Vì vậy mọi request dùng lại cùng key đều được xem như retry hợp lệ, kể cả khi payload đã thay đổi.

Idempotency key không phải global cache key cho mọi request có cùng chuỗi. Contract cần ràng buộc key với request gốc mà nó đại diện.

## Why the fix works

Reference solution lưu cả request fingerprint và response. Khi key đã tồn tại:

- fingerprint giống nhau → đây là retry cùng logical request, trả response cũ và không gọi provider lần nữa;
- fingerprint khác nhau → đây là key reuse sai contract, trả `409` và không thực hiện side effect mới.

## How to verify

```powershell
./verify.ps1
```

Expected:

- first request: `200`
- retry same payload: `200` và cùng receipt
- same key + different payload: `409`
- `providerCalls=1`

## Alternative fixes

1. Lưu canonical request payload cùng response thay vì hash. Dễ audit nhưng tốn storage hơn và có thể chứa dữ liệu nhạy cảm.
2. Lưu fingerprint từ các field tạo nên business identity thay vì toàn bộ JSON. Phù hợp khi serialization không ổn định, nhưng contract canonicalization phải rất rõ.
3. Để gateway/idempotency middleware quản lý invariant này nếu toàn hệ thống dùng chung một chuẩn đã được kiểm chứng.

## Wrong or misleading fixes

### Luôn trả cached response theo key

Đây chính là hành vi hiện tại: ngăn duplicate side effect nhưng có thể trả response của request khác.

### Xóa entry khi payload khác rồi xử lý request mới

Cách này có thể biến client bug thành một charge mới ngoài ý muốn. Với payment flow, reject rõ ràng thường an toàn hơn.

### Chỉ dùng `OrderId` làm fingerprint

Có thể sai nếu cùng order cho phép nhiều operation khác nhau hoặc amount/currency có thể thay đổi. Fingerprint phải phản ánh semantic identity của request theo API contract.

### Hash raw JSON bytes mà không canonicalize

Hai payload tương đương về semantic có thể khác whitespace, property order hoặc serializer settings. Hashing strategy phải ổn định theo contract.

## Production implications

Production implementation còn cần xử lý race giữa hai request đồng thời cùng key. `TryGetValue` rồi thực hiện provider call rồi mới lưu entry không đảm bảo atomicity trên nhiều process. Một hệ thống thật thường cần durable idempotency store với unique constraint / compare-and-set / transaction boundary phù hợp.

Lab này cố ý giới hạn ở invariant `key + request identity` để không trộn thêm distributed concurrency vào cùng một unit.

## Trade-offs

- Lưu full payload tăng khả năng debug nhưng tăng storage/privacy cost.
- Lưu hash nhỏ gọn hơn nhưng cần canonicalization chính xác.
- Reject mismatch bằng `409 Conflict` là contract dễ quan sát, nhưng mã lỗi cụ thể phải nhất quán với API conventions của hệ thống.

## What a Senior engineer should notice

Senior engineer không chỉ hỏi “có chống duplicate charge không?”, mà còn hỏi:

- idempotency key đại diện cho logical request nào;
- payload mismatch được xử lý thế nào;
- fingerprint được canonicalize ra sao;
- entry tồn tại bao lâu;
- concurrent first-writer race được khóa ở đâu;
- failure xảy ra sau provider side effect nhưng trước khi persist response được recovery thế nào.
