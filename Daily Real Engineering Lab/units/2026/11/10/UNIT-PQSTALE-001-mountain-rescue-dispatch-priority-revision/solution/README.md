# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Một incident được nâng mức ưu tiên nhưng thứ tự dispatch vẫn dựa vào severity cũ.

## Evidence
Trace cho thấy update thành công và giá trị trả về đúng; contract `severity revision changes dispatch order` thất bại.

## Root cause
`PriorityQueue<string,int>` lưu priority tại lúc `Enqueue`. Sửa giá trị trong `Dictionary` không cập nhật heap. Việc lấy severity mới sau dequeue chỉ thay đổi dữ liệu trả về, không thay đổi thứ tự đã chọn.

## Why the fix works
`DispatchQueue.cs` tham khảo dùng ticket bất biến cùng version tăng dần. Mỗi revision thêm một ticket mới. Dequeue bỏ qua ticket có version cũ; chỉ một ticket hiện hành được dispatch. Mọi lần hạ hoặc nâng severity đều có hiệu lực trong heap.

## How to verify
Chép `solution/DispatchQueue.cs` sang `starter/DispatchQueue.cs` **chỉ sau khi hoàn thành bài** rồi chạy `pwsh ./verify.ps1`. Không sửa `starter/Program.cs` để né contract.

## Alternative fixes
Có thể dùng indexed heap với decrease/increase-key O(log n) và bản đồ ID→heap index; ít stale entries hơn nhưng code và invariant phức tạp. Với số incident nhỏ, quét dictionary để tìm min là O(n) và đơn giản, đôi khi hợp lý hơn.

## Wrong or misleading fixes
- Chỉ cập nhật dictionary: không thay đổi priority trong heap.
- Enqueue lại không kiểm tra version: một incident có thể được dispatch nhiều lần.
- Sort kết quả sau dequeue: không thể khôi phục incident chưa được chọn đúng.
- Tăng worker count: không sửa sai thứ tự.

## Production implications
Cần khóa đồng bộ hoặc single-owner event loop nếu Enqueue/Revise/DispatchNext chạy từ nhiều thread; `PriorityQueue` và `Dictionary` không thread-safe. Stale ticket làm heap tăng theo số revision; cân nhắc compact định kỳ hoặc indexed heap. Quy tắc severity bằng nhau chưa bảo đảm FIFO, cần sequence tiebreaker nếu SLA yêu cầu.

## Trade-offs
Lazy invalidation đơn giản, sửa nhanh và O(log n) mỗi revision, nhưng dùng thêm bộ nhớ và có chi phí bỏ ticket cũ khi dequeue. Indexed heap có invariant phức tạp nhưng giới hạn bộ nhớ tốt hơn.

## What a Senior engineer should notice
Không nhầm lẫn **state of record** với **ordering index**. Contract phải bao phủ cả tăng/hạ ưu tiên, nhiều revision, duplicate và concurrency. Cần đo backlog, revision rate và thời gian dispatch trước khi chọn cấu trúc dữ liệu.

## Further reading
Microsoft Learn: [PriorityQueue](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2).
