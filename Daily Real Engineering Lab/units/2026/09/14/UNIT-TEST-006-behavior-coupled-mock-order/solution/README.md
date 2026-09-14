# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Production behavior của `NotificationCoordinator` vẫn đúng theo contract quan sát được, nhưng unit test đỏ sau một refactor nội bộ.

## 2. Evidence

Các assertion về `DeliveryReceipt`, sent message và audit record đều đúng. Failure chỉ đến từ assertion yêu cầu collaboration sequence chính xác là `template → send → audit`, trong khi implementation hiện tại thực hiện `audit → template → send`.

## 3. Root cause

Test đang coupling với internal interaction order dù thứ tự này không phải business contract. Vì vậy một behavior-preserving refactor tạo false negative và làm CI chặn thay đổi hợp lệ.

## 4. Why the fix works

Giữ các assertion về observable result và essential side effects: message đúng được gửi đúng một lần và audit đúng được ghi đúng một lần. Bỏ assertion về thứ tự nội bộ không có requirement. Test vẫn bảo vệ hành vi quan trọng nhưng cho implementation freedom để refactor.

## 5. How to verify

Copy cách tiếp cận trong `NotificationCoordinatorTests.cs` của solution sang file starter rồi chạy:

```powershell
./verify.ps1
```

Test suite phải pass.

## 6. Alternative fixes

- Nếu thứ tự thực sự là protocol requirement, giữ order assertion nhưng document requirement và đặt tên test theo contract đó.
- Tách orchestration thành các component nhỏ hơn nếu interaction graph quá lớn khiến test khó hiểu.
- Dùng integration test cho behavior xuyên nhiều collaborator khi unit test mock-heavy không còn mang nhiều giá trị.

## 7. Wrong / Tempting Fixes

- Đổi production code về thứ tự cũ chỉ để test xanh, khi không có requirement cho thứ tự đó: làm test chi phối thiết kế.
- Xóa toàn bộ interaction assertions: có thể làm mất protection với side effect quan trọng như send hoặc audit.
- Thay assertion order bằng sleep/retry: không liên quan tới root cause.
- Mock thêm nhiều collaborator hơn: thường tăng implementation coupling thay vì giảm nó.

## 8. Production implications

Over-specified tests làm chi phí refactoring tăng, tạo CI noise và khiến team ngại cải tiến code. Ngược lại, test quá lỏng có thể bỏ lọt regression. Mục tiêu là chọn đúng contract cần khóa.

## 9. Trade-offs

Behavior-focused tests thường bền hơn nhưng có thể ít pinpoint internal failure hơn. Interaction assertions vẫn hữu ích khi interaction chính là contract: transaction order, exactly-once boundary, security check trước side effect, protocol sequencing hoặc resource lifecycle.

## 10. What a Senior engineer should notice

Senior engineer không hỏi "mock có được gọi đúng thứ tự không?" trước tiên, mà hỏi "thứ tự đó có phải requirement hay chỉ là implementation hiện tại?". Test suite phải tạo safety net cho behavior và invariants, không biến implementation hiện tại thành specification ngoài ý muốn.
