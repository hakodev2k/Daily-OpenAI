# Reference Solution — chỉ xem sau khi reproduce và tự sửa

## 1. Symptoms
Partner trả 503 ở attempt đầu, nhưng không nhận được request thứ hai. Ứng dụng báo InvalidOperationException thay vì nhận 202 từ partner.

## 2. Evidence
Fake handler ghi nhận đúng một request; response đầu là 503. Exception xuất hiện khi client chuẩn bị retry, trước khi handler được gọi lần hai. Điều này bác bỏ giả thuyết partner tiếp tục lỗi.

## 3. Root cause
Starter tạo một HttpRequestMessage và StringContent bên ngoài vòng retry. HttpClient.SendAsync không cho gửi lại cùng một HttpRequestMessage đã được gửi. Vòng retry tái sử dụng object đó, dẫn đến InvalidOperationException. Response đầu đã được dispose đúng cách; lỗi không phải do HTTP 503 tự ném exception.

## 4. Why the fix works
Trong solution/PartnerHoldClient.cs, mỗi attempt tạo mới HttpRequestMessage và StringContent. Method PUT, URI, JSON body và Idempotency-Key được tạo từ cùng holdId nên hợp đồng logical operation không thay đổi. Response và request được dispose sau mỗi attempt. Chỉ 503 ở attempt đầu mới retry; các status khác đi qua EnsureSuccessStatusCode.

## 5. How to verify
Thay riêng starter/PartnerHoldClient.cs bằng cách tự sửa hoặc bản tham khảo, rồi chạy scripts/verify.ps1. Verify chạy trực tiếp starter; không build solution tách biệt. Các case: 503→202, 400, 202, 503→503, cancellation trước khi gửi. Script reproduce được thiết kế để chứng minh starter ban đầu lỗi, không dùng để chấm bản đã sửa.

## 6. Alternative fixes
- Factory nhận holdId rồi tạo HttpRequestMessage mới cho mỗi attempt, giữ logic retry tách biệt.
- Dùng một retry handler có chính sách rõ ràng và request factory, nhưng cần xác minh ownership của HttpContent, retry eligibility và disposal.
- Với request body streaming, cần tạo lại stream có thể đọc từ đầu hoặc buffer có giới hạn; không được giả định stream seekable.

## 7. Wrong / tempting fixes
- Catch InvalidOperationException rồi coi là 503: che mất lỗi lập trình và không gửi request thứ hai.
- Tăng retry lên 10: không giải quyết lifecycle, chỉ tăng chi phí và nguy cơ nhân đôi side effect.
- Bỏ Idempotency-Key: có thể tạo nhiều giữ chỗ khi request đầu đã được xử lý nhưng response bị mất.
- Retry mọi status, kể cả 400: lãng phí tài nguyên và che lỗi request.
- Tạo HttpClient mới cho mỗi attempt: không cần thiết, có thể gây connection churn; vòng đời request khác vòng đời client.

## 8. Production implications
PUT có ngữ nghĩa idempotent theo HTTP, nhưng nghiệp vụ partner vẫn cần contract rõ ràng. Giữ cùng Idempotency-Key cho mọi attempt của một logical operation; partner phải thực thi deduplication nếu side effect không thuần idempotent. Retry thực tế cần exponential backoff + jitter, xử lý Retry-After, timeout/cancellation budget chung, giới hạn số lần thử, metrics theo attempt và logical request. Không retry vô điều kiện POST có side effect.

## 9. Trade-offs
Lab chọn tối đa hai attempt và không delay để có bằng chứng deterministic. Trong production, delay và retry budget làm tăng tail latency nhưng giảm tải trong sự cố. Buffering body giúp replay nhưng tốn memory; streaming cần request factory có khả năng tái tạo nội dung. Cân bằng tính sẵn sàng, duplicate risk và tải partner.

## 10. Senior engineer should notice
Phải phân biệt failure xảy ra trước transport với status từ downstream, phân biệt lifetime của HttpClient và HttpRequestMessage, kiểm tra request equivalence qua retry, và thiết kế test cả failure path lẫn success path. Không suy từ số vòng lặp trong code ra số lần side effect thực tế.

## Reference source
[PartnerHoldClient.cs](PartnerHoldClient.cs). Chỉ dùng để đối chiếu sau khi đã thử tự sửa.
