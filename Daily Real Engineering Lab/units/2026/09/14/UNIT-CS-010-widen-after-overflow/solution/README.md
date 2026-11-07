# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Billing total của high-volume customer thấp hơn expected rất nhiều nhưng vẫn là số dương, method không ném exception và return type là `long`.

## 2. Evidence

`quantity = 70,000` và `unitPriceCents = 70,000` đều hợp lệ dưới `int.MaxValue`. Expected product là `4,900,000,000`, lớn hơn `int.MaxValue`. Starter trả về `605,032,704`.

## 3. Root cause

Cả hai operands đều là `int`, vì vậy expression `quantity * unitPriceCents` được evaluate bằng `int`. Overflow xảy ra trước khi kết quả đã sai được implicit-convert sang `long`. Return type rộng hơn không làm phép nhân trước đó tự động chạy bằng `long`.

## 4. Why the fix works

Promote ít nhất một operand sang `long` trước phép nhân để arithmetic diễn ra bằng `long`. Dùng `checked` làm failure explicit nếu sau này domain vượt cả range của type đã chọn:

```csharp
checked((long)quantity * unitPriceCents)
```

## 5. How to verify

Sửa `starter/`, sau đó chạy:

```powershell
./verify.ps1
```

Expected total là `4,900,000,000` cents.

## 6. Alternative fixes

- Đổi domain model sang `long` sớm hơn nếu quantity/amount thực sự có thể vượt `int`.
- Với tiền có fractional unit hoặc rounding rules, cân nhắc `decimal` theo business contract thay vì dùng floating point.
- Bật overflow checking ở project/build level có thể bổ sung guardrail, nhưng vẫn cần type/domain design đúng.

## 7. Wrong or misleading fixes

- `(long)(quantity * unitPriceCents)`: cast xảy ra sau khi overflow đã xảy ra.
- Chỉ đổi return type từ `int` sang `long`: starter đã chứng minh cách này chưa đủ.
- `Math.Abs` trên kết quả sai: chỉ che symptom, không khôi phục dữ liệu đã overflow.
- Dùng `double` chỉ để có range lớn hơn: có thể đưa thêm precision issues vào money calculation.

## 8. Production implications

Numeric overflow trong financial/usage calculation nguy hiểm vì kết quả có thể vẫn nhìn hợp lệ và đi qua validation. Nếu không có reconciliation hoặc invariant checks, sai lệch có thể tồn tại tới invoice/reporting downstream.

## 9. Trade-offs

`checked` giúp fail fast nhưng exception strategy phải phù hợp boundary. `long` tăng range nhưng không vô hạn. `decimal` phù hợp nhiều money domain nhưng có semantics và performance khác. Chọn type dựa trên range, unit, precision và business invariant.

## 10. What a Senior engineer should notice

Senior engineer phải reasoning về **type của expression**, không chỉ type của variable nhận kết quả. Với business-critical arithmetic, cần document units/ranges, đặt invariant tests ở boundary, dùng explicit overflow policy và test các giá trị gần giới hạn chứ không chỉ happy path nhỏ.
