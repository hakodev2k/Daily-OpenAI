# Reference Solution — SPOILER
Chỉ đọc sau khi reproduce, ghi evidence/hypotheses và tự sửa `starter/SessionPartitioner.cs`. Đây là một lời giải phù hợp với constraints của bài, không phải cấu hình Azure Service Bus production hoàn chỉnh.

## 1. Symptoms
Receipt của charger B được xử lý ở cycle 5 dù không lỗi. Receipt `A-next` còn pending. Operator khác tiếp tục chạy.

## 2. Evidence
Baseline mapping gom `A-poison,B-ok,A-next` vào cùng một session `operator-17`. Mỗi cycle chỉ attempt head của từng session. `A-poison` chiếm head trong bốn cycle trước khi vào DLQ; các receipt phía sau không được attempt. Số lượng worker không phải bằng chứng của lỗi xử lý dữ liệu.

## 3. Root cause
Session key chỉ là `TenantId` trong khi yêu cầu ordering thực sự là **mỗi charger trong mỗi tenant**. Session granularity rộng hơn business invariant, tạo head-of-line blocking giữa các charger độc lập. Receipt lỗi là trigger, không phải lý do duy nhất khiến backlog lan rộng.

## 4. Why the fix works
[SessionPartitioner.cs](SessionPartitioner.cs) tạo key từ cả tenant và charger với length-prefix để tránh ambiguity khi identifiers chứa dấu phân cách. Charger B tiến ở cycle 1 trong session riêng; charger A vẫn bảo toàn thứ tự và DLQ sau bốn lần attempt. Không dùng random key vì sẽ phá ordering.

## 5. How to verify
Thay logic tương ứng trong `starter/SessionPartitioner.cs`, chạy `.\verify.ps1`. Probe kiểm tra timing tương đối, DLQ, ordering cùng charger, isolation giữa tenant và không mất/trùng receipt. `verify.ps1` không đọc file solution. Trên starter gốc, `.\reproduce.ps1` PASS còn `.\verify.ps1` FAIL.

## 6. Alternative fixes
- Phân vùng theo `tenant+charger` là lựa chọn mặc định khi mỗi charger độc lập.
- Phân vùng theo một entity khác (ví dụ transaction aggregate) chỉ phù hợp nếu thứ tự nghiệp vụ gắn với entity đó.
- Tăng session concurrency hữu ích khi có nhiều session, nhưng không làm song song các message trong **một** session mà vẫn giữ session ordering.
- Cách ly poison message, backoff và DLQ policy là biện pháp bổ trợ; không thay thế thiết kế key đúng.

## 7. Wrong / tempting fixes
- Tăng số worker vô hạn: không giải quyết head-of-line trong một session.
- Bỏ session hoàn toàn: phá yêu cầu thứ tự receipt cùng charger.
- Đẩy ngay mọi message lỗi vào DLQ sau lần đầu: có thể làm mất cơ hội phục hồi lỗi transient; cần phân loại lỗi.
- Tăng MaxDeliveryCount: kéo dài blocking.
- Dùng key chỉ theo ChargerId: hai tenant có cùng charger identifier có thể bị gom nhầm.

## 8. Production implications
Azure Service Bus sessions gắn `SessionId` vào message lúc gửi; không sửa được `SessionId` của message đã enqueue. Triển khai cần thay đổi producer có version, kiểm tra consumer concurrency, drain/route backlog cũ, đo age-of-oldest-message theo session, số DLQ, delivery count và session lock renewal. Thực hiện kiểm tra ordering end-to-end và chống duplicate side effects bằng idempotency. Mô phỏng cố ý giản lược lock renewal, fairness, retry delay và thực tế DLQ.

## 9. Trade-offs
Session nhỏ hơn tăng khả năng song song và cô lập lỗi nhưng tăng số lượng active sessions và chi phí scheduling/monitoring. Session lớn hơn dễ giữ thứ tự toàn nhóm nhưng tạo coupling không cần thiết. Key cần ổn định, không collision và không tiết lộ dữ liệu nhạy cảm nếu được log.

## 10. What a Senior engineer should notice
Phân biệt trigger và root cause; xác định business ordering invariant trước khi tăng capacity. Dùng timeline có đối chứng, test positive/negative, đo blast radius, hoạch định migration của dữ liệu đã nằm trong broker và không đánh đồng simulator với behavior được Azure bảo đảm.
