# Reference Solution — chỉ xem sau khi tự reproduce và thử fix

## 1. Symptoms
Lần gửi đầu trả `TimedOut` nhưng transport còn một công việc active. Khi retry cùng `BEACON-17`, cả hai attempt đều có thể hoàn tất sau khi gate mở, tạo hai delivery.

## 2. Evidence
Fixture ghi `Active=1, Delivered=0` ngay sau timeout; trước khi mở gate có `Active=2`; sau gate có `Delivered=2`. Không có network latency ngẫu nhiên: thứ tự được kiểm soát bằng `TaskCompletionSource`.

## 3. Root cause
`Task.WhenAny` chỉ trả về task hoàn tất đầu tiên, không hủy task thua cuộc. Starter trả `TimedOut` mà không hủy và chờ kết thúc `SendAsync` đang chạy. Deadline của caller không tự động đồng nghĩa operation đã ngừng tạo side effect.

## 4. Why the fix works
[Mã tham chiếu](AlertDispatcher.cs) tạo linked cancellation token cho từng attempt, truyền token xuống transport, phát tín hiệu hủy khi deadline thắng và `await` operation cho đến khi đã quan sát trạng thái cuối. Chỉ nuốt `OperationCanceledException` tương ứng với token đã hủy; các lỗi transport khác vẫn nổi lên. Khi transport tuân thủ cancellation, không còn attempt cũ hoạt động lúc method trả timeout.

## 5. How to verify
Sao chép `solution/AlertDispatcher.cs` vào `starter/AlertDispatcher.cs`, rồi chạy `.\verify.ps1`. Script luôn chạy code trong `starter/`, không chạy solution trực tiếp. Các kiểm tra: timeout dọn sạch active work; retry tạo đúng một delivery; caller cancellation propagates; lỗi transport không bị biến thành success/timeout.

## 6. Alternative fixes
- Dùng `WaitAsync(timeout, token)` để giới hạn chờ, nhưng **chỉ** `WaitAsync` không hủy underlying operation; phải có cơ chế hủy và quan sát completion.
- Nếu transport không hỗ trợ cancellation, cần quyết định rõ: chờ operation kết thúc, chuyển sang state `UNKNOWN` và reconciliation, hoặc thực hiện outbox/receipt + idempotency.
- Với đối tác có idempotency key, gửi cùng một key qua các retry và đối chiếu receipt là hướng production mạnh hơn.

## 7. Wrong / tempting fixes
- Tăng timeout: chỉ giảm tần suất biểu hiện, không giải quyết lifecycle.
- Bỏ `await pending` khi deadline thắng: có thể để lại side effect và unobserved fault.
- `Task.Run` bọc `SendAsync`: không biến network operation thành cancellable.
- Cho rằng cancellation bảo đảm exactly-once: đối tác có thể đã commit trước khi nhận hủy.

## 8. Production implications
Phân biệt `timed-out`, `cancelled`, `delivered`, `unknown`. Gắn correlation/idempotency key với attempt, ghi receipt và theo dõi pending operations, retries, late completions; không log payload cảnh báo chứa dữ liệu nhạy cảm. Hạn chế retry tự động khi trạng thái kết quả chưa rõ.

## 9. Trade-offs
Chờ operation acknowledge cancellation có thể kéo dài thời gian trả response nếu transport không hợp tác. Nếu latency budget nghiêm ngặt, cần mô hình durable queue và trạng thái asynchronous thay vì khẳng định `TimedOut` là chưa gửi. Idempotency phía nhận và reconciliation tốn thêm storage/complexity nhưng bảo vệ khỏi duplicate side effects.

## 10. Senior engineer should notice
Timeout là hợp đồng về **thời gian chờ**, không phải bằng chứng một thao tác chưa tạo side effect. Test phải chứng minh không còn in-flight work, không chỉ assert return value. Thiết kế retry phải nêu rõ at-least-once, at-most-once hay effectively-once và giới hạn của từng lựa chọn.
