# Reference Solution — SPOILER
Chỉ đọc sau khi reproduce, ghi hypothesis và tự sửa.

## 1. Symptoms
Hai worker cùng hoàn tất nhưng hai phiếu có cùng request ID; request thứ hai bị bỏ sót.

## 2. Evidence
Fixture ép hai worker chạm checkpoint trước khi tiếp tục. Hai lượt nhận việc thành công, nhưng multiset phiếu đầu ra khác multiset yêu cầu. Không có exception, nên kiểm tra chỉ dựa vào exit code hoặc số lượng phiếu sẽ bỏ sót sai lệch.

## 3. Root cause
`ConcurrentQueue<T>.TryPeek` chỉ quan sát phần tử đầu, **không claim** nó. Hai worker có thể cùng đọc phần tử A. Sau checkpoint, `TryDequeue` lần lượt lấy A và B, nhưng starter bỏ qua phần tử thực sự lấy được và xử lý giá trị đã quan sát trước đó (A). Từng lời gọi thread-safe, nhưng cặp `Peek → Dequeue` không nguyên tử.

## 4. Why the fix works
Chỉ dùng `TryDequeue(out var claimed)` làm ranh giới nhận việc. Mỗi worker xử lý đúng `claimed.RequestId`, không dùng snapshot trước khi claim. File [PickupDispatcher.cs](PickupDispatcher.cs) là mã tham khảo, không được harness compile.

## 5. How to verify
Sửa `starter/PickupDispatcher.cs`, chạy `pwsh -File ./verify.ps1`. Hai fixture overlap phải giữ đủ cả hai ID; fixture một yêu cầu và rỗng không regression. Sau sửa, `reproduce.ps1` phải exit 1.

## 6. Alternative fixes
- Dùng `Channel<T>` với một reader hoặc nhiều reader, đọc từng item bằng thao tác nhận việc rõ ràng; phù hợp khi có producer/consumer bất đồng bộ.
- Dùng lock bao quanh toàn bộ đoạn quan sát và claim nếu nghiệp vụ thật sự cần đọc trước khi nhận; đơn giản trong một process nhưng tăng contention.
- Dùng database transaction/lease cho queue bền vững nếu worker chạy trên nhiều instance; không thể thay bằng `ConcurrentQueue<T>` trong memory.

## 7. Wrong or misleading fixes
- Thêm `Task.Delay` hoặc tăng ThreadPool: thay lịch chạy, không tạo tính nguyên tử.
- Chỉ dùng `Distinct()` trên phiếu: che duplicate và vẫn làm mất yêu cầu B.
- Kiểm tra `Count == 2`: số lượng đúng nhưng danh tính sai.
- Retry toàn batch không có idempotency: có thể phát hành thêm phiếu trùng.
- Đổi `ConcurrentQueue<T>` sang `Queue<T>` mà không đồng bộ: làm mất thread-safety.

## 8. Production implications
Log `batchId`, `requestId`, `workerId`, `claimId` và kết quả xử lý; theo dõi mismatch giữa request và receipt theo ID, không chỉ tổng count. Trong hệ thống phân tán, phải định nghĩa điểm ack, replay và idempotent side effect; queue trong process không đảm bảo durability.

## 9. Trade-offs
Atomic dequeue đủ cho bài toán một process, không yêu cầu DB hay Redis. Nếu phải phục hồi sau crash, queue bền vững với ack/lease có thêm latency và chi phí vận hành nhưng cần thiết. Một reader giảm tranh chấp nhưng hạn chế throughput; nhiều reader tăng concurrency và yêu cầu claim rõ ràng.

## 10. What a Senior engineer should notice
Thread-safe API không biến chuỗi thao tác thành transaction. Cần xác định linearization point cho claim, phân biệt **at-most-once claim trong process** với **exactly-once business effect**, và đo đúng invariant về danh tính khi chạy song song.
