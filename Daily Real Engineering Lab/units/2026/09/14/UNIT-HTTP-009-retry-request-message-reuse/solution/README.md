# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Attempt đầu tiên tới fake downstream và nhận `503 Service Unavailable`. Retry tiếp theo không tới handler và chương trình báo `InvalidOperationException`.

## 2. Evidence

Starter in `ATTEMPTS=1` dù retry loop có tối đa 2 attempt. Điều này cho thấy failure thứ hai xảy ra ở client trước khi downstream nhận request.

## 3. Root cause

Một `HttpRequestMessage` instance chỉ đại diện cho một send operation. Starter tạo message một lần bên ngoài retry loop rồi gọi `SendAsync` lại với cùng instance. Sau send đầu tiên, .NET không cho gửi lại chính message đó.

## 4. Why the fix works

Reference solution tạo một `HttpRequestMessage` mới cho từng attempt, trong khi giữ command data ở dạng có thể tái tạo request. Mỗi retry vì vậy có lifecycle riêng và có thể đi qua `HttpClient` tới handler.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Expected:

```text
RESULT=OK
STATUS=200
ATTEMPTS=2
```

## 6. Alternative fixes

- Đóng gói việc tạo request vào một factory method nhận command immutable.
- Với retry middleware/policy, retry delegate nên tạo request mới cho mỗi execution.
- Với payload stream lớn, thiết kế nguồn dữ liệu có thể mở/tạo stream mới cho từng attempt thay vì giữ một stream đã consume.

## 7. Wrong or misleading fixes

- Catch `InvalidOperationException` rồi coi như retry thành công: che symptom nhưng command không được gửi.
- Tăng retry count: cùng lifecycle sai sẽ chỉ tiếp tục thất bại.
- Tạo `HttpClient` mới cho mỗi attempt: không giải quyết ownership của request message và còn làm lifecycle connection khó quản lý hơn.

## 8. Production implications

Retry boundary phải xác định rõ object nào reusable và object nào thuộc về một attempt. Khi payload, headers hoặc content stream được build động, request factory cần tái tạo đầy đủ dữ liệu nghiệp vụ cho từng lần gửi.

## 9. Trade-offs

Tạo request object mới có allocation nhỏ nhưng đổi lại semantics rõ ràng và retry an toàn. Với payload lớn, việc buffer toàn bộ dữ liệu để retry có thể tốn memory; khi đó cần chiến lược replay phù hợp với loại content.

## 10. What a Senior engineer should notice

Retry không chỉ là vòng lặp quanh `SendAsync`. Một retry đáng tin cậy phải xem xét lifecycle/ownership của request, replayability của body, idempotency của operation, timeout/cancellation và retry condition. Đặc biệt với non-idempotent command, chỉ retry khi contract phía server và idempotency strategy cho phép.
