# Reference Solution — chỉ mở sau khi tự điều tra

## 1. Symptoms

Những response có `Retry-After` dạng HTTP-date bị lên lịch theo fallback 2 giây, kể cả khi đối tác yêu cầu chờ lâu hơn hoặc thời điểm đó vượt quá budget. Delta-seconds, status không retry, malformed header và budget thông thường vẫn có thể pass, nên nhìn vào tổng số case dễ bỏ sót.

## 2. Evidence

Starter `reproduce` in ra 4 case date FAIL (`date-future-429`, `date-future-503`, `date-past-503`, `date-over-budget`, `date-at-budget` — thực tế là **5** case). Các case `delta-seconds`, `delta-over-budget`, `nonretryable-400` vẫn PASS. Dấu hiệu là nhóm dữ liệu dạng thời điểm tuyệt đối bị xử lý không đúng.

## 3. Root cause

`Retry-After` theo HTTP semantics có hai dạng: **delay-seconds** và **HTTP-date**. Starter chỉ dùng `int.TryParse`; mọi HTTP-date đi qua fallback 2 giây. Quyết định budget vì vậy cũng dựa trên delay sai. Đây là lỗi diễn giải hợp đồng giao thức, không phải lỗi clock scheduler hay network.

## 4. Why the fix works

Reference `solution/Program.cs` xử lý delta-seconds trước, sau đó parse IMF-fixdate (RFC1123) bằng invariant culture; dùng `receivedAtUtc` được truyền vào để tính khoảng chờ, clamp date trong quá khứ về 0, rồi mới kiểm tra `remainingBudget`. Bộ dữ liệu giả lập này cố ý chỉ dùng IMF-fixdate và delta-seconds. Trong production nên dùng parser chuẩn của HTTP stack (ví dụ `HttpResponseHeaders.RetryAfter`) và kiểm tra khả năng tương thích với HTTP-date legacy.

## 5. How to verify

```powershell
pwsh ./scripts/verify.ps1
dotnet run --project ./solution/RetryPlannerSolution.csproj -- verify
```

`verify.ps1` luôn kiểm tra **starter người học đã sửa**, không phải solution. Kết quả đúng: `SUMMARY: passed=13/13; ... allPassed=True`, exit 0. Không sleep; thời gian cố định giúp regression độc lập máy chạy.

## 6. Alternative fixes

- Dùng `HttpResponseHeaders.RetryAfter` của `HttpClient` để nhận `Delta` hoặc `Date` khi có `HttpResponseMessage` thực, giảm parser tự viết.
- Tách parser và retry policy thành hai thành phần để unit test riêng; hữu ích khi số đối tác và loại response tăng.
- Với request có side effect, áp dụng idempotency key hoặc chỉ retry operation an toàn; policy delay không tự đảm bảo exactly-once.

## 7. Wrong / tempting fixes

- Tăng fallback từ 2 lên 30 giây: che triệu chứng nhưng phá latency và vẫn sai khi budget nhỏ.
- Retry ngay mọi 429: tăng nguy cơ retry storm và bị rate-limit thêm.
- Bỏ budget check: có thể vượt deadline của batch.
- Dùng `DateTime.Now` trong parser: kết quả phụ thuộc timezone/clock của máy, test khó tái lập.
- Retry mọi 4xx: làm lặp request lỗi validation không thể tự hồi phục.

## 8. Production implications

Giới hạn số attempt, tổng deadline, cancellation token, jitter cho retry đồng loạt, phân biệt idempotent/non-idempotent operations. Xem xét clock skew và `Date` response header khi đối tác và client lệch giờ. Log status, parsed delay, remaining budget, attempt number, outcome; tránh log token hoặc dữ liệu nhạy cảm.

## 9. Trade-offs

Parser thủ công nhỏ, dễ test nhưng dễ bỏ sót biến thể HTTP-date. Parser chuẩn của .NET đáng tin cậy hơn nhưng cần boundary rõ giữa transport và scheduling. Chọn fallback 2 giây là chính sách demo, không phải giá trị tối ưu cho mọi hệ thống. Khi header invalid, cần metric và retry policy có giới hạn thay vì âm thầm tin dữ liệu.

## 10. Senior engineer should notice

Một policy retry cần vừa đúng giao thức vừa đúng giới hạn business: **HTTP semantics + clock domain + remaining budget + idempotency + observability**. Test nên chứng minh cả nhánh chấp nhận retry lẫn nhánh từ chối retry, không chỉ kiểm tra một phép parse thành công.

Tài liệu chính: RFC 9110 §10.2.3 (Retry-After), Microsoft .NET `System.Net.Http.Headers.RetryAfterHeaderValue`. 
