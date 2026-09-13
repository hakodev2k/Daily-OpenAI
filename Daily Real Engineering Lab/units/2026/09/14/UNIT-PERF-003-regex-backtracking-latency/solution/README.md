# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Validation nhanh với input bình thường nhưng một near-match input dài làm một call CPU-bound kéo dài đến timeout dù kết quả cuối cùng chỉ là `false`.

## 2. Evidence

Ba case đi qua cùng validator nhưng chỉ near-match tăng mạnh elapsed time. Không có I/O trong path này, nên evidence hướng vào CPU work của matching engine thay vì database/network.

## 3. Root cause

Pattern `^(A+)+Z$` có nested quantifier. Với chuỗi gồm nhiều `A` nhưng không có `Z` cuối cùng, backtracking engine có nhiều cách chia cùng đoạn `A` cho group bên trong và group lặp bên ngoài. Engine thử một lượng lớn partition trước khi chứng minh không match.

## 4. Why the fix works

Business rule thực tế chỉ cần một hoặc nhiều `A` rồi `Z`, nên pattern có thể đơn giản thành `^A+Z$`. Reference solution đồng thời dùng `RegexOptions.NonBacktracking`, làm execution cost predictable hơn cho pattern tương thích. Timeout vẫn được giữ như defensive bound ở validation boundary.

## 5. How to verify

Copy cách sửa tương đương vào `starter/` rồi chạy:

```powershell
./verify.ps1
```

Functional expectations:

- `AAAAAZ` => `true`
- `BAAAAA` => `false`
- long near-match => `false`
- không timeout và không vượt ngưỡng verify trên learner-editable path

## 6. Alternative fixes

- Nếu rule đủ đơn giản, bỏ regex và validate bằng code tuyến tính rõ nghĩa.
- Dùng pattern khác không tạo nested ambiguous quantifier.
- Với pattern hỗ trợ, `RegexOptions.NonBacktracking` giúp tránh backtracking behavior.
- Luôn cân nhắc match timeout cho regex xử lý input không tin cậy.

## 7. Wrong or misleading fixes

- Chỉ tăng timeout: làm request chờ lâu hơn nhưng không loại bỏ work explosion.
- Retry validation: lặp lại cùng CPU work và có thể làm incident nặng hơn.
- Scale out ngay: tăng capacity nhưng không sửa input-dependent algorithmic behavior.
- Chỉ giới hạn request timeout ở tầng HTTP: bảo vệ request lifetime nhưng CPU work bên trong vẫn có thể gây contention nếu cancellation/bound không được áp dụng đúng chỗ.

## 8. Production implications

Regex chạy trên request path với input do client kiểm soát có thể trở thành latency/capacity risk. Một số input hiếm có thể chiếm CPU lâu hơn dữ liệu bình thường nhiều bậc, làm p95/p99 tăng và giảm throughput dù dependency telemetry vẫn sạch.

## 9. Trade-offs

`NonBacktracking` không hỗ trợ mọi regex feature, nên không phải drop-in replacement cho mọi pattern. Regex timeout là safety net chứ không thay cho thiết kế pattern tốt. Với rule đơn giản, validation code tuyến tính thường dễ đọc và dễ reasoning hơn regex phức tạp.

## 10. What a Senior engineer should notice

Senior engineer không dừng ở việc tìm một pattern nhanh hơn. Cần nhận ra đây là boundary nhận input không tin cậy, đo cost theo input shape, đặt execution bound, giữ regression case đại diện cho near-match, và chọn công cụ đơn giản nhất diễn tả đúng business rule.
