# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

CI xanh và test timeout pass, nhưng behavioral probe trả về `0.00` thay vì approved fallback `12.50`.

## 2. Evidence

Test hiện tại chỉ chứng minh hai điều: exception không thoát ra khỏi service và carrier được gọi đúng một lần. Không assertion nào kiểm tra quote được trả về.

## 3. Root cause

Test oracle tập trung vào interaction và absence of exception thay vì business behavior. Vì vậy một giá trị fallback không hợp lệ vẫn được test suite chấp nhận.

## 4. Why the fix works

Sửa failure path để trả về approved fallback `12.50m`, đồng thời thêm regression test assert trực tiếp output contract. Khi implementation hoặc requirement thay đổi sai, test sẽ fail tại boundary mà business quan tâm.

## 5. How to verify

Chạy `verify.ps1`. Script chạy test suite trên learner-editable `starter/` rồi chạy behavioral probe độc lập và yêu cầu `FallbackQuote=12.50`.

## 6. Alternative fixes

Trong production, fallback có thể đến từ typed configuration hoặc policy object thay vì hard-code. Điều quan trọng là test phải xác minh behavior được business chấp thuận, không chỉ cách implementation gọi collaborator.

## 7. Wrong or misleading fixes

- Chỉ thêm `Assert.Equal(1, carrier.CallCount)`: vẫn không kiểm tra quote.
- Chỉ assert “không throw”: availability không đồng nghĩa correctness.
- Đổi probe để chấp nhận `0.00`: sửa test oracle để hợp thức hóa bug.
- Mock nhiều hơn: thêm interaction assertions không tự tạo ra behavioral coverage.

## 8. Production implications

Green CI không phải bằng chứng đủ cho correctness nếu test oracle yếu. Failure paths của payment, shipping, retry, cache và integration boundaries cần assertion trên observable contract.

## 9. Trade-offs

Interaction tests hữu ích khi interaction chính là contract hoặc side effect quan trọng. Nhưng nếu outcome mới là business contract, behavior assertion nên là lớp bảo vệ chính; interaction assertion chỉ bổ sung khi có giá trị.

## 10. What a Senior engineer should notice

Senior engineer không chỉ hỏi “coverage bao nhiêu phần trăm?” mà hỏi test có khả năng phát hiện failure mode quan trọng hay không, test oracle có bám business invariant hay không, và probe/integration test nào độc lập với implementation details.

Reference source: [ShippingQuoteService.cs](ShippingQuoteService.cs)
