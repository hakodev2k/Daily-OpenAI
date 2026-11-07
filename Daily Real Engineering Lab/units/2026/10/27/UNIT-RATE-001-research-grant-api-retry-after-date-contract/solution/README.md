# Reference Solution — chỉ đọc sau khi reproduce và tự thử sửa

## 1. Symptoms

Một nhóm phản hồi 429 có chỉ dẫn chờ hợp lệ nhưng client vẫn lập lịch retry sau 2 giây, dẫn tới thêm 429. Nhóm dùng số giây không có vấn đề.

## 2. Evidence

Clock UTC được cố định. Ba fixture có giá trị dạng HTTP-date đều sai ở starter; delta-seconds, fallback, malformed header và maxDelay của dạng delta vẫn đúng. Evidence không cho thấy CPU saturation.

## 3. Root cause

HTTP \`Retry-After\` cho phép hai dạng: \`delay-seconds\` và HTTP-date. Starter chỉ dùng \`RetryConditionHeaderValue.Delta\`; khi response chứa HTTP-date thì \`Delta\` null, khiến fallback được chọn dù header hợp lệ.

## 4. Why the fix works

Ưu tiên \`RetryAfter.Delta\`; nếu không có, dùng \`RetryAfter.Date - now\`. Đồng hồ \`now\` được inject để test deterministic. Clamp cùng một chính sách cho âm, maxDelay và fallback. Method signature không đổi.

## 5. How to verify

Copy logic vào \`starter/RetryPlanner.cs\`, chạy \`./verify.ps1\` để kiểm tra **learner-editable starter**, không phải reference. Tùy chọn: \`dotnet run --project ./solution/RetryWindowLab.Solution.csproj\` để self-check reference riêng.

## 6. Alternative fixes

Có thể parse string với parser chuẩn RFC, nhưng typed header của .NET giảm code và lỗi locale. Khi tích hợp nhiều resilience policies, có thể dùng pipeline chuyên dụng, nhưng phải kiểm chứng HTTP-date, max elapsed time, jitter, cancellation và idempotency.

## 7. Wrong / tempting fixes

Tăng fallback lên 30 giây chỉ che lỗi và tăng latency khi header thiếu. Retry ngay sau 429 làm tải tăng. Parse HTTP-date bằng local timezone hoặc current culture dễ gây sai lệch. Retry mọi POST có thể lặp side effects nếu không có idempotency contract.

## 8. Production implications

Bổ sung giới hạn số lần thử, tổng deadline, phân biệt 429/503, cancellation và cơ chế defer-to-queue khi cửa sổ chờ vượt SLA. HTTP-date phụ thuộc clock: so sánh với Date header của server khi có bằng chứng clock skew; không tự động tin mọi header.

## 9. Trade-offs

Tuân thủ date có thể tăng latency nhưng giảm áp lực lên đối tác. MaxDelay 30 giây là policy giả lập, không phải chuẩn chung; clamp rồi retry sớm có thể vi phạm mong đợi của đối tác, nên production có thể cần dừng và defer.

## 10. Senior engineer should notice

Cần tách protocol semantics, scheduling policy, clock accuracy và retry side effects. Chứng minh bằng regression fixtures cho cả hai representation, không chỉ sửa một test đang fail.

## Official references

- [RFC 9110 — Retry-After](https://www.rfc-editor.org/rfc/rfc9110.html#name-retry-after)
- [HttpResponseHeaders.RetryAfter](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpresponseheaders.retryafter)
