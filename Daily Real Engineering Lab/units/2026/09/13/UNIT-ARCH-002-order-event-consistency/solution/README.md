# Reference Solution — xem sau khi đã tự làm

## 1. Symptoms

Order có thể đã ở trạng thái `Approved` trong SQL nhưng Inventory/Billing không nhận được `OrderApproved`. Client retry cũng không sửa được vì business state đã đổi và request retry bị từ chối là `AlreadyApproved`.

## 2. Evidence

Incident timeline cho thấy SQL `COMMIT` hoàn tất ở T+109 ms nhưng process chết ở T+112 ms, trước khi có bằng chứng broker nhận message. Đây là một partial failure giữa hai durability boundary độc lập.

## 3. Root cause

Database commit và message publish là hai side effect riêng biệt, không nằm trong cùng atomic boundary. Sau khi SQL commit thành công, process có thể chết hoặc broker có thể unavailable trước khi integration event trở nên durable.

## 4. One defensible solution

Dùng **Transactional Outbox**:

1. Trong cùng SQL transaction với việc đổi `Orders.Status`, insert một `OutboxMessage` chứa event ID, aggregate ID, event type, payload và timestamp.
2. Commit transaction. Khi commit thành công, cả business state và publish intent cùng durable; khi rollback, cả hai cùng rollback.
3. Một publisher/background worker đọc các outbox record chưa publish và gửi chúng tới broker.
4. Chỉ đánh dấu outbox record đã xử lý sau khi broker acknowledgement thành công.
5. Vì crash có thể xảy ra sau broker acknowledgement nhưng trước lúc đánh dấu sent, publisher phải chấp nhận duplicate publish.
6. Consumer phải xử lý idempotently bằng stable event ID / inbox-deduplication hoặc business idempotency key.

## 5. Why the fix works

Giải pháp không làm SQL và broker trở thành một distributed transaction. Thay vào đó, nó loại bỏ failure window nơi business state đã durable nhưng không còn durable record nào cho biết event vẫn cần được gửi. Publisher có thể retry/recover độc lập sau restart.

## 6. How to verify

Failure-path review phải chứng minh:

- Crash trước SQL commit: không có approved order và không có outbox record.
- Crash ngay sau SQL commit: approved order và outbox record cùng tồn tại; publisher mới có thể tiếp tục gửi.
- Broker outage: outbox backlog tăng nhưng intent không mất.
- Crash sau publish trước `sent` update: event có thể được gửi lại; consumer vẫn không tạo duplicate side effect.
- HTTP retry: không phải cơ chế duy nhất để recovery event publication.

Operational checks nên có:

- số outbox record pending,
- age của oldest pending record,
- publish retry/error rate,
- dead/poison publication records,
- consumer deduplication failures.

## 7. Alternative fixes

### Publish trước rồi mới commit SQL

Chỉ đảo failure window: message có thể được consumer xử lý dù SQL transaction sau đó rollback hoặc process chết trước commit.

### Synchronous HTTP tới downstream

Có thể phù hợp với workflow khác, nhưng không đáp ứng constraint hiện tại là downstream integration qua broker và vẫn tạo distributed partial-failure/retry problems nếu có nhiều downstream.

### Distributed transaction

Có thể tồn tại trong một số technology topology, nhưng constraint của lab loại bỏ shared DTC giữa SQL và broker; operational coupling cũng thường lớn hơn đáng kể.

### Change Data Capture / database-log based publishing

Là phương án hợp lệ khi platform đã có CDC/connectors đáng tin cậy và team vận hành được nó. Nó có thể giảm application-side publisher logic nhưng tăng infrastructure dependency và mapping/governance complexity.

## 8. Wrong / tempting fixes

- `try/catch` quanh `PublishAsync`: không giúp khi process chết trước hoặc giữa call.
- Retry publish ngay trong HTTP request: giảm transient failures nhưng không loại bỏ crash window sau database commit.
- Retry toàn bộ HTTP request vô hạn: có thể tạo duplicate side effects và còn phụ thuộc business state cho phép replay.
- Đánh dấu order `Approved` chỉ sau khi publish: chuyển inconsistency sang chiều ngược lại.
- Giả định broker exactly-once nên consumer không cần idempotency: end-to-end business exactly-once không tự động xuất hiện chỉ từ broker semantics.

## 9. Production implications

Outbox cần retention/cleanup, batching, concurrency control, retry policy, poison-record handling và observability. Table/index design phải tránh publisher scan gây tải lớn lên primary workload. Event schema/versioning vẫn là contract riêng cần quản lý.

## 10. Trade-offs

**Ưu điểm:** recovery rõ ràng, không mất publish intent sau DB commit, phù hợp với at-least-once messaging.

**Chi phí:** thêm outbox storage, worker/publisher lifecycle, duplicate handling và operational monitoring. Event delivery là eventually consistent thay vì synchronous với request completion.

## 11. What a Senior engineer should notice

Mục tiêu không phải tạo “exactly once” bằng khẩu hiệu. Cần xác định rõ từng durability boundary, enumerate crash windows, chọn semantics có thể chứng minh, và thiết kế idempotency + observability cùng lúc với happy path. Transactional Outbox là một phương án tốt ở đây vì nó phù hợp constraints, không phải vì pattern này luôn đúng cho mọi hệ thống.
