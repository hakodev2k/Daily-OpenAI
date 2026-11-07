# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Hai request trả thành công nhưng audit record của request A có thể mang tenant của request B.

## 2. Evidence

Reproduction cố ý giữ request A bên trong downstream await, sau đó cho request B đi vào cùng middleware instance. Output cho thấy state tenant bị thay đổi trước khi request A resume.

## 3. Root cause

Conventional middleware instance được tái sử dụng giữa nhiều request. Starter lưu dữ liệu chỉ thuộc một request vào mutable instance field `_currentTenant`. Khi request A đang `await`, request B ghi đè field đó. Request A resume và đọc state của request B.

## 4. Why the fix works

Giữ tenant trong local variable của `InvokeAsync` làm state thuộc riêng invocation. Mỗi async state machine giữ giá trị của request tương ứng, nên request khác không thể ghi đè nó qua shared middleware field.

## 5. How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Kết quả phải chứa đúng `request-a -> tenant-a` và `request-b -> tenant-b`.

## 6. Alternative fixes

Nếu middleware thực sự cần dependency scoped, có thể inject dependency vào tham số `InvokeAsync` hoặc dùng `IMiddleware` với lifetime phù hợp. Nhưng request-specific scalar state vẫn nên giữ local khi không cần chia sẻ.

## 7. Wrong / tempting fixes

- Dùng `lock` quanh toàn bộ middleware: có thể che race nhưng serialize request và phá throughput.
- Tăng ThreadPool: không thay đổi ownership/lifetime của state.
- Đổi sang `AsyncLocal` chỉ để giữ một giá trị đơn giản: tăng complexity không cần thiết khi local variable đã đúng boundary.
- Retry request: chỉ chạy lại triệu chứng correctness và có thể nhân side effect.

## 8. Production implications

Cross-request contamination có thể làm sai audit, tenant routing, authorization context hoặc telemetry dimensions. Đây là correctness/security boundary issue chứ không chỉ là logging defect.

## 9. Trade-offs

Local state đơn giản nhất và có ownership rõ. Scoped services phù hợp khi state cần dependency-managed request lifetime. Global synchronization chỉ hợp lệ nếu thật sự có shared invariant cần serialize.

## 10. What a Senior engineer should notice

Khi code có `await`, hãy hỏi state nào có thể bị request khác mutate trước khi continuation resume. Lifetime của object và lifetime của data không giống nhau. Shared object có thể an toàn nếu immutable/stateless; mutable per-request state trên shared object là dấu hiệu cần điều tra ngay.
