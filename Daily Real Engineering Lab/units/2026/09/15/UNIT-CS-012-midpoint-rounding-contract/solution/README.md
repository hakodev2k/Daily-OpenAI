# Reference Solution — chỉ xem sau khi đã tự điều tra

## Symptoms

Một số monetary values nằm đúng midpoint bị lệch `0.01` so với accounting contract, trong khi các giá trị khác vẫn đúng.

## Evidence

Starter cho thấy `1.005` và `2.345` không cho kết quả mà contract yêu cầu, nhưng các non-midpoint values vẫn đúng. Điều này chỉ ra vấn đề nằm ở rounding semantics chứ không phải precision của `decimal` nói chung.

## Root cause

`Math.Round(decimal, int)` dùng midpoint rounding mặc định là `MidpointRounding.ToEven`. Accounting contract của lab yêu cầu midpoint được làm tròn `AwayFromZero`. Code dựa vào default behavior nên domain policy không được thể hiện explicit.

## Why the fix works

Dùng overload thể hiện chính sách trực tiếp:

```csharp
Math.Round(amount, 2, MidpointRounding.AwayFromZero)
```

Khi midpoint policy trong code khớp accounting contract, các contract cases cho kết quả nhất quán.

## How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Verification chỉ pass khi mọi case khớp expected result.

## Alternative fixes

Có thể đóng gói rounding policy trong một `MoneyRoundingPolicy` hoặc value object nếu nhiều bounded context cùng cần quy tắc này. Chỉ nên thêm abstraction khi có giá trị tái sử dụng hoặc cần kiểm soát policy tập trung.

## Wrong / tempting fixes

- Cộng một epsilon trước khi round: tạo magic number và gây sai ở case khác.
- Chuyển sang `double`: không giải quyết mismatch domain policy và còn đưa binary floating-point vào monetary calculation.
- Chỉ format bằng `ToString("0.00")`: thay đổi presentation chứ không sửa numeric value dùng cho posting.
- Hard-code riêng các input đang fail: chỉ che symptom.

## Production implications

Rounding là business rule, không chỉ là chi tiết formatting. Payment, tax, invoice và reconciliation systems cần chốt rõ precision, midpoint policy và vị trí rounding trong calculation pipeline. Nếu mỗi service dùng default khác nhau, sai lệch nhỏ có thể tích lũy thành reconciliation mismatch lớn.

## Trade-offs

`AwayFromZero` không phải policy đúng cho mọi domain. Một số accounting systems dùng banker's rounding (`ToEven`). Senior engineer cần bám vào authoritative business contract, centralize policy khi cần và test midpoint cases rõ ràng.

## What a Senior engineer should notice

Điểm quan trọng không phải ghi nhớ một overload. Code xử lý tiền cần làm explicit domain semantics thay vì phụ thuộc default framework behavior, đặc biệt ở boundary tạo dữ liệu tài chính có tính đối soát.
