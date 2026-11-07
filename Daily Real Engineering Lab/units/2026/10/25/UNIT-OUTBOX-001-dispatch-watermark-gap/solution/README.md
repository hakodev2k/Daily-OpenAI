# Reference Solution — SPOILER

**Chỉ đọc sau khi đã reproduce và thử sửa starter.** Code tham chiếu: [OutboxPipeline.cs](OutboxPipeline.cs). Có thể copy file này vào `starter/OutboxPipeline.cs` rồi chạy `.\scripts\verify.ps1` để so sánh hành vi.

## 1. Symptoms
Worker báo batch có lỗi 503 ngắn, vẫn gửi được event mới hơn. Một event cũ không đến downstream và không xuất hiện ở lượt retry kế tiếp.

## 2. Evidence
Outbox chứa 101, 102, 103; lượt đầu broker chấp nhận 101 và 103, từ chối 102 đúng một lần. Checkpoint sau lượt đầu là 103. Lượt hai đọc các sequence lớn hơn 103 nên không lấy lại 102.

## 3. Root cause
Dispatcher bắt `TransientDeliveryException` rồi tiếp tục gửi các event sau phần tử thất bại. Với checkpoint kiểu **high-water mark**, việc lưu sequence 103 khẳng định ngầm rằng toàn bộ prefix <=103 đã hoàn tất, dù 102 chưa được acknowledge. Checkpoint đã vượt qua một **gap**.

## 4. Why the fix works
Ở mô hình **một active dispatcher, outbox có thứ tự**, khi gặp transient publish failure, dừng vòng lặp batch (không đánh dấu record thất bại là đã gửi). Checkpoint giữ ở 101. Lượt kế tiếp lấy lại 102 rồi 103; không có gap. Thay đổi tham chiếu nằm trong catch block: `break;`. Không thay đổi thứ tự commit: publish thành công trước, checkpoint sau.

## 5. How to verify
Chạy `.\scripts\verify.ps1` trên code đã sửa trong `starter/`. Bốn acceptance cases kiểm tra: gap + retry, happy-path, batch boundary, và checkpoint write failure. Với solution copy sang starter, kỳ vọng `ALL_VERIFICATIONS_PASSED` và exit 0. Không cần dịch vụ bên ngoài.

## 6. Alternative fixes
- **Dừng tại lỗi đầu tiên**: đơn giản, giữ ordering, phù hợp một dispatcher; một poison message có thể chặn cả stream, cần retry policy, dead-letter hoặc intervention.
- **Trạng thái delivery per-row**: đánh dấu riêng từng record, retry những record thất bại; hỗ trợ parallelism và tránh head-of-line blocking nhưng cần schema/transaction và cơ chế claim/lease, ordering không còn mặc định.
- **Checkpoint prefix theo ACK bitmap**: có thể xử lý ngoài thứ tự nhưng chỉ advance đến prefix liên tục; thêm memory/state, thuật toán phục hồi và chi phí vận hành.

## 7. Wrong / tempting fixes
- **Bỏ qua 503 và tiếp tục**: giữ throughput tức thời nhưng làm mất event khi checkpoint vượt gap.
- **Retry ngay 3 lần rồi vẫn tiếp tục**: giảm xác suất lỗi, không bảo đảm correctness khi dependency vẫn fail.
- **Đặt checkpoint = max(sequence) của batch**: càng làm mất dữ liệu.
- **Đặt checkpoint = min(sequence)**: gây replay không cần thiết, có thể lặp vô hạn; không mô tả prefix đã acknowledge.
- **Tin rằng publish + checkpoint là exactly-once**: hai thao tác không có distributed transaction; crash giữa chúng vẫn có thể phát lại.

## 8. Production implications
Persist checkpoint cùng durable outbox, có metrics `oldest-undelivered-age`, retry count, poison-message alert, và trace với event ID (tránh PII). Consumer nên idempotent theo event ID. Trong lab, fake publisher dedupe chỉ mô phỏng **consumer-side idempotency**; broker thật không tự bảo đảm dedupe trừ khi cấu hình/contract nói rõ.

## 9. Trade-offs
Dừng tại lỗi đầu tiên ưu tiên không mất event và ordering, đánh đổi throughput và availability khi có poison message. Nếu cần nhiều worker, cần claim ownership, lease/fencing token hoặc per-record status và kiểm soát cạnh tranh; giải pháp một-worker không đủ cho scale-out. Đảm bảo thực tế là **at-least-once**, không phải exactly-once.

## 10. What a Senior engineer should notice
Tách ba khái niệm: **published/acknowledged**, **checkpoint persisted**, **business effect applied**. Một checkpoint là cam kết logic về tất cả event trước nó, không chỉ là sequence lớn nhất từng nhìn thấy. Đặt invariant và fault injection ở mọi ranh giới I/O, không chỉ happy-path.

## Optional follow-up
Nếu broker trả ACK nhưng worker crash trước khi ghi checkpoint, record sẽ phát lại. Thiết kế idempotency key và retry policy có dead-letter mà không vi phạm ordering contract.
