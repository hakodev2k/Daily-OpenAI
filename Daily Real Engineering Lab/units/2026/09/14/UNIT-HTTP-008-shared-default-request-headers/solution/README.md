# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Hai outbound call cho hai tenant khác nhau có thể gửi cùng một authorization value khi chúng overlap. Test tuần tự thường không lộ lỗi.

## 2. Evidence

Fake handler ghi lại header thực sự có trên `HttpRequestMessage` khi request được gửi. Output cho thấy tenant A mong đợi `Bearer token-A` nhưng có thể gửi `Bearer token-B`.

## 3. Root cause

Code đặt credential thay đổi theo từng request vào `HttpClient.DefaultRequestHeaders`. `HttpClient` là shared object và default headers là shared mutable state. Giữa lúc operation A ghi token A và lúc A thực sự gọi `GetAsync`, operation B có thể ghi token B. Request A sau đó được tạo từ state chung đã bị thay đổi.

## 4. Why the fix works

Giữ `HttpClient` được reuse, nhưng tạo một `HttpRequestMessage` riêng cho từng outbound call và gắn authorization vào chính request đó. Request-specific state không còn phụ thuộc vào mutable default state dùng chung giữa concurrent operations.

## 5. How to verify

Sửa code trong `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Verification chạy hai tenant đồng thời và yêu cầu cả hai request đều mang đúng credential riêng.

## 6. Alternative fixes

- Typed client có credential cố định cho một partner/identity duy nhất là hợp lý nếu lifetime và identity thực sự cố định.
- `DelegatingHandler` có thể gắn credential từ immutable request context nếu thiết kế context rõ ràng và không dùng shared mutable tenant state.
- Có thể truyền token explicit xuống integration method và tạo request tại boundary gửi HTTP.

## 7. Wrong or misleading fixes

- Tạo `new HttpClient()` cho mỗi request: có thể che race nhưng tạo lifecycle/connection-management problem khác và không giải quyết ownership model đúng cách.
- Dùng `lock` quanh toàn bộ HTTP call: serialize traffic, giảm throughput và biến một vấn đề request isolation thành bottleneck.
- Retry khi nhận `401`: request retry vẫn có thể dùng sai credential và còn tăng side effect/traffic.
- Tăng logging nhưng giữ nguyên shared state: giúp quan sát nhưng không sửa invariant bị vi phạm.

## 8. Production implications

Cross-tenant credential bleed là lỗi correctness và có thể trở thành security incident. Với multi-tenant systems, mọi context như authorization, tenant ID, correlation metadata và idempotency key cần có ownership/lifetime rõ ràng.

## 9. Trade-offs

Per-request `HttpRequestMessage` thêm một object nhỏ cho mỗi call nhưng giữ được connection pooling của shared `HttpClient`. Đây thường là trade-off đúng: connection lifetime được share, request metadata thì không.

## 10. What a Senior engineer should notice

Câu hỏi quan trọng không phải “có nên reuse HttpClient không?” mà là “state nào được phép share?”. Senior engineer tách resource có lifetime dài (handler/connection pool/client) khỏi context chỉ hợp lệ trong một request. Khi concurrent code sai ngẫu nhiên nhưng tuần tự đúng, hãy kiểm tra mutable shared state và ownership boundaries trước khi thêm retry hoặc scale-out.
