# Reference Solution — SPOILER

**Chỉ xem sau khi tự reproduce, viết hypothesis và sửa starter.** Source tham khảo: [DispatchQueue.cs](DispatchQueue.cs). Chạy reference riêng: `dotnet run --project ./solution/DispatchLab.Reference.csproj`. Script `verify.ps1` luôn kiểm tra **starter**, không kiểm tra reference.

## 1. Symptoms
Sau khi BLD-A nâng mức từ 5 lên 0, BLD-B mức 2 vẫn được dispatch trước. Dữ liệu yêu cầu phản ánh mức khẩn mới, nhưng kết quả lập lịch không theo hợp đồng.

## 2. Evidence
Case đầu có timeline: enqueue A(5) → enqueue B(2) → escalate A(0) → dequeue B. Trường `Urgency` trên đối tượng A đã đổi nhưng thứ tự nội bộ vẫn phản ánh độ ưu tiên lúc enqueue.

## 3. Root cause
`PriorityQueue<TElement, TPriority>` lưu `TElement` và `TPriority` riêng. Heap sắp xếp bằng **priority value truyền vào khi Enqueue**, không tự tái sắp xếp khi một property của `TElement` bị sửa. Starter chỉ cập nhật `Request.Urgency` mà không cập nhật ticket trong heap.

## 4. Why the fix works
Reference tạo ticket bất biến `(Id, Version)`, còn trạng thái có `Urgency`, `Version`, `Sequence`. Mỗi lần nâng mức hợp lệ, tăng version và enqueue ticket mới với priority hiện hành. `DispatchNext` bỏ qua ticket cũ (version không khớp hoặc ID đã dispatch). Như vậy chỉ ticket mới nhất có quyền điều phối; `Sequence` gốc giữ FIFO cho yêu cầu đồng mức.

## 5. How to verify
- Chạy `./scripts/reproduce.ps1` trên starter ban đầu: có case FAIL.
- Tự sửa `starter/DispatchQueue.cs`, chạy `./scripts/verify.ps1`: năm case PASS.
- Chạy `dotnet run --project ./solution/DispatchLab.Reference.csproj` để so sánh tham khảo (không thay thế verify của learner).
- Đặt breakpoint vào vòng dequeue để thấy ticket cũ bị bỏ qua.

## 6. Alternative fixes
- **Remove rồi Enqueue lại:** .NET 8 có `PriorityQueue.Remove`; cần giữ thông tin ưu tiên và đảm bảo không bỏ nhầm request, chi phí tìm phần tử O(n) trong nhiều implementation.
- **Dùng sorted index hỗ trợ update-key:** phức tạp hơn nhưng hợp lý khi thay đổi ưu tiên liên tục, queue rất lớn và cần giới hạn heap rác.
- **Rebuild heap:** có thể chấp nhận với batch nhỏ, cập nhật hiếm, không phải lựa chọn mặc định cho luồng realtime.

## 7. Wrong / tempting fixes
- Chỉ đổi `Request.Urgency`: không thay đổi priority đã enqueue.
- Enqueue lại ngay mà không vô hiệu hóa ticket cũ: có nguy cơ dispatch trùng sau khi queue cạn.
- Sắp xếp kết quả sau khi dispatch: quá muộn, đã gây side effect vận chuyển.
- Thay `PriorityQueue` bằng `ConcurrentQueue`: thread-safe không đồng nghĩa với ưu tiên đúng.
- Chỉ sửa case đầu: bỏ sót nhiều lần nâng mức và thứ tự FIFO.

## 8. Production implications
Cấu trúc reference phù hợp với một dispatcher thread và số lần nâng mức có giới hạn. Ticket cũ chiếm bộ nhớ đến khi được pop; với nhiều lần thay đổi, cần metric `staleTicketsSkipped`, giới hạn backlog và có thể compact/rebuild định kỳ. Không dùng nguyên mẫu này cho multi-worker nếu chưa bảo vệ cập nhật state, enqueue và claim bằng synchronization/transaction phù hợp.

## 9. Trade-offs
Mỗi escalation O(log n) để thêm ticket, dispatch có thể phải bỏ qua nhiều ticket cũ; tổng số ticket tăng theo số escalation. Đổi lại, tránh tìm và xóa phần tử trong heap ở mỗi lần thay đổi, giữ invariant đúng một lần và FIFO. Với yêu cầu durable hoặc nhiều replica, cần persistence, idempotency và cơ chế claim phân tán thay vì heap trong bộ nhớ.

## 10. Senior engineer should notice
Đây là **invariant giữa mutable business state và scheduling index**. Phải phân biệt source of truth với index được tạo từ snapshot. Test cần bao phủ thay đổi sau enqueue, nhiều lần thay đổi, phát trùng, FIFO và hành vi khi không còn công việc. Cần nêu rõ giới hạn single-thread, khả năng mất queue khi restart, backpressure và khả năng audit quyết định điều phối.

## Further reading
Tài liệu Microsoft: https://learn.microsoft.com/dotnet/api/system.collections.generic.priorityqueue-2
