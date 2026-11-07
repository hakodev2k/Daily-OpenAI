# Reference Solution

> Reference Solution — inspect only after attempting your own decision. Đây là một defensible solution dưới constraints đã cho, không phải kiến trúc duy nhất đúng.

## 1. Symptoms

Team đang tranh luận theo pattern identity: “CQRS everywhere” đối đầu với “no CQRS”. Cả hai đều bỏ qua sự khác nhau giữa CRUD configuration và behavior-rich workflows.

## 2. Evidence

10/18 endpoints có business complexity thấp. Ba write workflows có state transition, transaction, concurrency hoặc side effects rõ rệt. Không có requirement separate read store, traffic thấp và team nhỏ.

## 3. Root cause

Vấn đề thiết kế là áp dụng abstraction theo convention toàn module thay vì theo behavioral boundary và operational constraints.

## 4. One defensible solution

Giữ CRUD configuration đơn giản qua application/query services. Dùng explicit command handlers cho `ConfirmReceiving`, `AllocateInventory` và `CloseShipment`, nơi command boundary làm transaction ownership, validation, concurrency policy và side effects dễ nhìn thấy hơn. Dashboard reads có thể dùng query services/projections trực tiếp mà không cần ép thành một hệ CQRS vật lý riêng.

MediatR là implementation option, không phải bản thân CQRS. Nếu dùng MediatR, chỉ dùng khi pipeline behaviors tạo giá trị đo được; không tạo handler chỉ để đổi một method call thành `Send()`.

## 5. How to verify the decision

Review ba workflow phức tạp: transaction boundary phải explicit, business validation testable, authorization không bị bỏ qua và outbox write phải nằm cùng transaction với state change cần bảo vệ. Review CRUD paths: số lớp thêm vào phải có lý do cụ thể.

## 6. Alternatives

**CQRS/MediatR everywhere:** convention rất đồng nhất và pipeline behaviors dễ áp dụng, nhưng tăng ceremony/cognitive load cho CRUD đơn giản.

**Application services everywhere:** ít abstraction, dễ onboarding, nhưng behavior-rich workflows có thể trở thành service methods lớn với boundary khó thấy nếu discipline yếu.

**Selective CQRS:** phù hợp constraints hiện tại nhưng yêu cầu team document boundary rõ để tránh kiến trúc trở nên ngẫu nhiên.

## 7. Wrong or misleading fixes

Tách read/write database ngay không giải quyết vấn đề hiện tại và thêm consistency/operations cost. Chuyển sang microservices không làm transaction semantics rõ hơn tự động. Ngược lại, cấm handler hoàn toàn chỉ vì “overengineering” cũng bỏ qua giá trị của explicit use-case boundary ở workflow phức tạp.

## 8. Production implications

Selective boundary giữ operational topology đơn giản trong khi tạo nơi rõ ràng để instrument latency, failure rate, concurrency conflicts và business outcomes cho các workflow quan trọng.

## 9. Trade-offs

Giải pháp đổi một phần convention uniformity lấy lower ceremony. Team cần architecture tests hoặc review checklist để bảo đảm authorization, validation và transaction policy không phụ thuộc vào việc endpoint có đi qua MediatR hay không.

## 10. What a Senior engineer should notice

Senior engineer không chọn CQRS vì tên pattern. Họ xác định nơi complexity thật sự nằm, chọn abstraction nhỏ nhất giúp quản lý complexity đó, và định nghĩa signals để revisit. Ví dụ: read workload diverges mạnh, query scaling khác write scaling, business workflows tăng nhanh, hoặc pipeline concerns lặp lại đủ nhiều để một mediator trở nên có giá trị.

## Suggested migration sequence

1. Không rewrite 18 endpoints.
2. Chọn `AllocateInventory` làm pilot vì concurrency/validation boundary rõ.
3. Viết behavior tests trước khi đổi structure.
4. Chuyển hai workflow phức tạp còn lại nếu pilot cải thiện maintainability/observability.
5. Giữ CRUD hiện tại trừ khi có evidence abstraction mới tạo giá trị.
6. Revisit sau vài release dựa trên defect rate, change lead time, handler/service complexity và production diagnostics.