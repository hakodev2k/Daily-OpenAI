# Reference Solution — UNIT-PERF-001

> Chỉ đọc sau khi đã reproduce và thử fix.

## Symptoms

Validator xử lý input bình thường nhanh, nhưng một near-match input dài có thể chạm `RegexMatchTimeoutException`, làm worker giữ CPU lâu hơn và giảm throughput batch.

## Evidence

- Không có I/O trong đường validation.
- Latency tăng theo độ dài near-match input.
- Timeout chỉ xuất hiện với một hình dạng input cụ thể.

## Root cause

Pattern `^(a+)+$` chứa quantifier lồng nhau. Với traditional backtracking engine, một input dài gồm nhiều `a` rồi kết thúc bằng ký tự không match buộc engine thử rất nhiều cách phân chia cùng chuỗi trước khi kết luận thất bại. Chi phí có thể tăng phi tuyến mạnh theo input.

## Why the fix works

`RegexOptions.NonBacktracking` chuyển sang engine có đặc tính thời gian tuyến tính cho pattern được hỗ trợ. Contract match không đổi, nhưng engine không thực hiện cây thử lại combinatorial như backtracking engine. Timeout vẫn giữ lại như defense-in-depth.

## How to verify

Sau khi áp dụng fix tương đương trong `starter/`, chạy:

```powershell
./verify.ps1
```

Kết quả phải là `VERIFY_PASS`, đồng thời valid/invalid cases vẫn đúng contract và adversarial input không timeout.

## Alternative fixes

- Viết lại pattern thành `^a+$`, đây là lựa chọn đơn giản nhất nếu business rule thực sự chỉ yêu cầu một hoặc nhiều ký tự `a`.
- Dùng parser/validation code thông thường nếu rule có thể biểu diễn rõ ràng hơn regex.
- Giảm input size ở boundary khi business contract có giới hạn hợp lý.

## Wrong or misleading fixes

- Chỉ tăng timeout: triệu chứng xuất hiện muộn hơn nhưng complexity vẫn còn.
- Chỉ scale thêm worker: tăng capacity nhưng không loại bỏ input-dependent CPU amplification.
- Bắt `RegexMatchTimeoutException` rồi retry cùng pattern/input: có thể nhân chi phí CPU mà không thay đổi điều kiện thất bại.
- Tắt timeout: làm mất guardrail và có thể khiến worker bị giữ CPU lâu hơn.

## Production implications

Regex trên dữ liệu không tin cậy là một performance boundary. Validator cần có complexity phù hợp, input bounds hợp lý, telemetry cho timeout/latency và regression test với near-match/adversarial cases.

## Trade-offs

`NonBacktracking` không hỗ trợ mọi regex construct. Nếu pattern dùng feature không tương thích, cần redesign pattern hoặc dùng validation code khác. Timeout vẫn hữu ích như safety net nhưng không thay thế việc giảm algorithmic complexity.

## What a Senior engineer should notice

- CPU spike có thể do shape của input, không chỉ volume.
- Performance bug có thể trở thành availability risk khi attacker hoặc bad data điều khiển input.
- Verification phải giữ functional contract, không chỉ làm benchmark nhanh hơn.
- Fix tốt xử lý cơ chế khuếch đại chi phí thay vì chỉ tăng tài nguyên hoặc timeout.
