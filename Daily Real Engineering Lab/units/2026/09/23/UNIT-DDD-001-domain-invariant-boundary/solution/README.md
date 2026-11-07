# Reference Solution — inspect only after your own decision

Đây là một defensible solution dưới constraints đã cho, không phải kiến trúc duy nhất đúng.

## Symptoms và evidence

Hai request có thể cùng đọc một trạng thái hợp lệ, cùng pass validation, rồi cùng commit thay đổi khiến tổng reservation vượt stock. Vấn đề nằm ở consistency của state transition chứ không phải CPU hay network.

## Root cause

Business invariant được kiểm tra bên ngoài boundary bảo vệ concurrent state change. Read/check/write độc lập cho phép quyết định dựa trên state đã cũ.

## Một phương án chính

Đặt inventory availability và reservation transition trong một transactional consistency boundary tại authoritative database. State transition phải có concurrency precondition để chỉ một thay đổi hợp lệ được commit khi hai request cạnh tranh. Request thua race phải reload/re-evaluate thay vì mặc định tiếp tục từ snapshot cũ.

Trong domain model, aggregate/API thể hiện operation Reserve(quantity) và invariant, nhưng persistence boundary vẫn phải cung cấp concurrency guarantee thực tế. DDD object model tự nó không tạo isolation.

## Duplicate handling

Client operation cần một stable idempotency key hoặc reservation identity để retry cùng logical command không tạo reservation thứ hai.

## Alternatives

- Pessimistic locking có thể phù hợp khi contention cao và transaction ngắn, đổi lại lock wait/deadlock handling.
- Atomic conditional update ở database có thể đơn giản và hiệu quả nếu model nhỏ và invariant biểu diễn được trực tiếp.
- Serialized command processing theo SKU có thể hữu ích ở quy mô khác nhưng thêm messaging/partition operations; không cần mặc định chọn nó cho constraints hiện tại.

## Verification

Test concurrent reservations lặp lại nhiều lần và assert invariant sau khi tất cả operation kết thúc. Test duplicate retry riêng. Quan sát conflict/retry rate trên hot SKU.

## Tempting fixes

- Chỉ kiểm tra available sớm hơn trong application service không tạo concurrency guarantee.
- Process-local lock không bảo vệ nhiều application instances.
- Thêm distributed infrastructure ngay lập tức có thể giải quyết coordination nhưng tăng complexity khi database đã là authoritative boundary.

## Production implications

Theo dõi conflict rate, transaction duration, hot SKU contention và retry amplification. Nếu contention profile thay đổi mạnh, decision có thể cần được xem lại.

## Senior engineer should notice

Aggregate boundary nên xuất phát từ invariant cần consistency, không phải từ việc gom mọi entity liên quan vào một object graph lớn. Domain rule và persistence concurrency guarantee phải hỗ trợ nhau.