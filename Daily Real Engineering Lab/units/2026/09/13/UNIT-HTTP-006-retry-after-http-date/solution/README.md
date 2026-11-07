# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## Symptoms

`429 Too Many Requests` có `Retry-After: 5` được xử lý đúng, nhưng `Retry-After` chứa HTTP-date hợp lệ bị tính thành `0s`, khiến client retry ngay.

## Evidence

Starter chỉ thử parse toàn bộ header thành integer. Vì vậy delta-seconds đi qua, còn HTTP-date rơi vào fallback.

## Root cause

HTTP `Retry-After` cho phép hai representation: `delay-seconds` hoặc HTTP-date. Parser của client chỉ implement nhánh `delay-seconds`, nên contract wire bị thu hẹp sai so với protocol.

## Why the fix works

Parser phải hỗ trợ cả integer seconds và HTTP-date. Với HTTP-date, delay là `retryAt - now`; nếu kết quả âm thì clamp về zero. Production code cũng nên đặt upper bound hợp lý theo retry policy để tránh sleep ngoài ý muốn.

## How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Cả `DELTA_DELAY` và `DATE_DELAY` phải nằm quanh 5 giây.

## Alternative fixes

- Khi dùng `HttpResponseMessage`, ưu tiên typed header API (`response.Headers.RetryAfter`) thay vì tự parse raw string.
- Nếu resilience library đang được dùng, map typed `Retry-After` vào delay generator của policy.
- Có thể áp dụng server delay kết hợp client-side cap/jitter nếu contract sản phẩm yêu cầu.

## Wrong or misleading fixes

- Luôn retry sau một fixed delay: tránh immediate retry nhưng bỏ qua server guidance.
- Chỉ tăng exponential backoff: không sửa protocol parsing và có thể vẫn vi phạm rate-limit window.
- Catch parse exception rồi retry ngay: biến malformed/unsupported input thành request amplification.

## Production implications

Immediate retries khi nhận `429` có thể tạo retry storm, kéo dài throttling, tăng queue backlog và làm partner API khó hồi phục. Log nên ghi rate-limit response, parsed delay và retry attempt nhưng tránh log secrets.

## Trade-offs

Tôn trọng `Retry-After` cải thiện protocol compliance, nhưng client vẫn cần policy giới hạn maximum delay, total retry budget và cancellation. Không nên sleep vô hạn chỉ vì upstream gửi timestamp rất xa.

## What a Senior engineer should notice

Đây không phải chỉ là parsing bug; nó là mismatch giữa internal assumption và external protocol contract. Khi xử lý standard header, typed framework API hoặc specification-aware parser thường an toàn hơn parser ad-hoc.