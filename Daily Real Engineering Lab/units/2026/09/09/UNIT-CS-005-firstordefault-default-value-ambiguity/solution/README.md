# Reference Solution

> Chỉ xem sau khi đã reproduce và thử fix trong `starter/`.

## 1. Symptoms

`M-ZERO` có một override record hợp lệ với `FeePercent = 0`, nhưng service trả về `2.5` giống merchant không có override.

## 2. Evidence

Starter in ra `matches=1 raw=0 resolved=2.5` cho `M-ZERO`, trong khi `M-NONE` có `matches=0` và cũng resolve thành `2.5`.

## 3. Root cause

Pipeline project trực tiếp sang `decimal`, sau đó dùng `FirstOrDefault()`. Với sequence rỗng, `FirstOrDefault()` trả default của `decimal`, tức `0m`. Domain đồng thời cho phép `0m` là override hợp lệ, nên code làm mất information về presence.

## 4. Why the fix works

Giữ matched record (hoặc một representation explicit của presence) đến business decision. Khi record tồn tại, dùng value của record kể cả bằng `0`; chỉ fallback khi thật sự không có record.

## 5. How to verify

Chạy:

```powershell
./verify.ps1
```

Verification yêu cầu `M-ZERO -> 0`, `M-LOW -> 0.75`, `M-NONE -> 2.5`.

## 6. Alternative fixes

- Project sang `decimal?` rồi phân biệt `null` với `0`.
- Dùng `SingleOrDefault()` trên record nếu uniqueness là invariant và validate duplicate configuration riêng.
- Dùng explicit result type/Try-pattern khi boundary này được tái sử dụng rộng hơn.

## 7. Wrong / tempting fixes

- Đổi fallback condition từ `== 0` sang `< 0`: chỉ đổi sentinel, không sửa contract nếu negative cũng có thể xuất hiện sau này.
- Thêm special case cho `M-ZERO`: che symptom và phá generic behavior.
- Dùng một magic value khác như `decimal.MinValue`: vẫn multiplex presence và value vào cùng một primitive.

## 8. Production implications

Lỗi kiểu này thường silent: không exception, query đúng và data tồn tại. Nó xuất hiện khi default CLR value trùng với valid domain value. Các boundary lookup nên mô hình hóa absence một cách explicit.

## 9. Trade-offs

Giữ whole record thường đơn giản và readable nhất. Nullable projection ngắn gọn nhưng chỉ phù hợp nếu `null` không phải valid domain state khác. Result type rõ contract hơn nhưng thêm abstraction cost.

## 10. Senior engineer should notice

Đừng chỉ nhìn LINQ API; hãy kiểm tra semantic contract xuyên qua boundary. Một primitive value không phải lúc nào cũng đủ để biểu diễn cả dữ liệu và trạng thái tồn tại của dữ liệu.
