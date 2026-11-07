# Reference Solution — Spoiler
## Symptoms
Gateway nhận năm yêu cầu nhưng hàng đợi chỉ còn hai; không có exception.

## Evidence
Fixture có bốn yêu cầu Priority=5 và một yêu cầu Priority=3. Starter dự kiến Enqueue chỉ true lần đầu với mỗi Priority.

## Root cause
SortedSet<T> dùng IComparer<T> vừa sắp xếp vừa xác định equivalence. Starter trả Compare=0 cho hai job khác nhau cùng Priority, nên các job sau bị xem là trùng.

## Why the fix works
Thứ tự đầy đủ: Priority giảm dần; ReceivedSequence tăng dần; JobId so sánh ordinal cuối cùng. Hai job cùng Priority/Sequence vẫn phân biệt được nếu JobId khác; bản ghi giống hệt bị từ chối.

## How to verify
Thay starter/DispatchQueue.cs bằng code reference rồi chạy ./verify.ps1. Acceptance kiểm tra đủ năm job, thứ tự chính xác và duplicate giống hệt. Đây là static review; chỉ đánh dấu executed sau khi tự chạy.

## Alternative fixes
- List<T> rồi sort khi đọc: đơn giản cho batch nhỏ, chi phí sort theo lần đọc.
- PriorityQueue với composite priority: tốt cho dequeue, cần giải pháp khác để duyệt snapshot và deduplicate.
- SortedDictionary<CompositeKey,Job>: khóa rõ hơn nhưng thêm kiểu composite.

## Wrong / tempting fixes
- Retry Enqueue không thay đổi comparator.
- ConcurrentBag giữ dữ liệu nhưng bỏ yêu cầu thứ tự.
- Random tie-breaker phá thứ tự ổn định và hợp đồng comparer.
- Chỉ thêm ReceivedSequence không đủ nếu các producer dùng sequence trùng nhau (F-205, F-208).

## Production implications
SortedSet<T> không thread-safe. Cần lock hoặc một actor/consumer sở hữu hàng đợi. Không mutate trường so sánh khi đã thêm vào set. Nếu muốn uniqueness theo JobId bất kể Priority/Sequence, cần chính sách riêng; comparator này chỉ định nghĩa uniqueness theo bộ ba. Nếu cần chống mất việc khi crash, ack chỉ sau durable commit.

## Trade-offs
SortedSet hỗ trợ thêm/xóa O(log n), snapshot duyệt có thứ tự O(n), nhưng cần comparator đúng và đồng bộ concurrent. PriorityQueue tối ưu dequeue, không phải snapshot sorted. In-memory collection không thay thế durable queue.

## Senior engineer should notice
Phân biệt received và accepted; test cùng priority, cùng sequence khác ID, duplicate thật và concurrency. Kiểm tra semantics của collection trước khi quy lỗi cho transport hay database.
