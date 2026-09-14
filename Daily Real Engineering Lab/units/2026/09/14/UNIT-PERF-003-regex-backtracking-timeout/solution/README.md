# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Validator xử lý input thông thường rất nhanh nhưng một số chuỗi gần hợp lệ làm thời gian xử lý tăng mạnh và cuối cùng chạm `RegexMatchTimeoutException`.

## 2. Evidence

Starter cho thấy cùng một business rule nhưng runtime cost thay đổi không tuyến tính khi input gồm nhiều ký tự hợp lệ liên tiếp rồi kết thúc bằng một ký tự làm toàn bộ match thất bại.

## 3. Root cause

Pattern `^([a-z]+)+$` chứa nested quantifier: một nhóm `+` nằm bên trong một `+` khác. Với near-match input, backtracking engine phải thử rất nhiều cách phân hoạch cùng chuỗi ký tự trước khi kết luận rằng ký tự cuối không thể match. Số đường thử có thể tăng rất nhanh theo độ dài input.

## 4. Why the fix works

Business rule thực tế chỉ yêu cầu một hoặc nhiều ký tự `a-z`, nên pattern có thể biểu diễn trực tiếp bằng `^[a-z]+$`. Reference solution đồng thời dùng `RegexOptions.NonBacktracking`, khiến engine sử dụng chiến lược có runtime gần tuyến tính cho pattern được hỗ trợ. Timeout vẫn được giữ như một defense-in-depth boundary.

## 5. How to verify

Sau khi sửa `starter/Program.cs`:

```powershell
./verify.ps1
```

Expected:

- `VALID_RESULT=True`
- `INVALID_RESULT=False`
- `ADVERSARIAL_TIMEOUTS=0`

## 6. Alternative fixes

- Với rule rất đơn giản, bỏ regex hoàn toàn và validate từng character có thể dễ đọc, predictable và ít overhead hơn.
- Nếu regex thực tế cần feature không tương thích với `NonBacktracking`, hãy redesign pattern để loại ambiguous nested quantifier và vẫn giữ timeout.
- Với regex tĩnh trong application code, `GeneratedRegex` có thể giảm một số startup/compilation overhead, nhưng nó không tự động sửa một pattern có complexity xấu.

## 7. Wrong or misleading fixes

- Chỉ tăng timeout: symptom xuất hiện muộn hơn nhưng algorithmic cost vẫn còn.
- Chỉ scale out: thêm capacity không loại bỏ input-dependent CPU amplification.
- Chặn mọi input dài bằng một ngưỡng tùy ý: input length limit có thể là defense hữu ích, nhưng không thay thế việc sửa pattern khi business rule không cần nested quantifier.
- Cache kết quả validation: không giải quyết first-hit cost và có thể tạo thêm state/complexity không cần thiết.

## 8. Production implications

Regex nằm trên request path cần được xem như executable logic có cost phụ thuộc input. Với input do client kiểm soát, pathological backtracking có thể trở thành availability risk. Production code nên kết hợp pattern đơn giản, timeout hợp lý, input bounds theo domain và telemetry cho timeout/latency bất thường.

## 9. Trade-offs

`RegexOptions.NonBacktracking` ưu tiên predictable runtime nhưng không hỗ trợ toàn bộ feature của backtracking engine. Nếu business rule cần backreferences hoặc một số constructs nâng cao, có thể phải giữ engine mặc định và thiết kế pattern cẩn thận hơn. Với validation đơn giản, explicit character validation thường là lựa chọn dễ vận hành nhất.

## 10. What a Senior engineer should notice

Senior engineer không chỉ sửa pattern. Họ cần nhận ra đây là một capacity và trust-boundary problem: input shape có thể khuếch đại CPU cost. Họ sẽ kiểm tra timeout, input limits, telemetry, các regex tương tự trên hot paths và tránh coi scale-out là root-cause fix.
