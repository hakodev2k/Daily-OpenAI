# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Contract trả về asynchronous sequence, nhưng consumer chỉ nhận record đầu tiên sau khi upstream đã tạo xong toàn bộ dataset. Peak buffered item count tăng cùng kích thước export.

## 2. Evidence
Trong starter, toàn bộ `PRODUCED 1..8` xuất hiện trước `CLIENT_RECEIVED 1`. Điều này chứng minh delay nằm trong application enumeration boundary, không cần suy đoán từ network timing.

## 3. Root cause
`ExportAsync` enumerate toàn bộ upstream sequence vào `List<string>` trước khi bắt đầu `yield return`. Kiểu trả về là `IAsyncEnumerable<string>` nhưng implementation đã phá incremental delivery bằng materialization trung gian.

## 4. Why the fix works
Reference implementation chuyển tiếp từng row ngay trong `await foreach`. Consumer có thể nhận row N trước khi producer tạo row N+1, nên memory không còn cần tăng tuyến tính chỉ để giữ toàn bộ export trước delivery.

## 5. How to verify
Chạy `./verify.ps1` sau khi sửa `starter/`. Script yêu cầu `CLIENT_RECEIVED 1` xuất hiện trước `PRODUCED 8` và vẫn phải có `TOTAL_RECEIVED 8`.

## 6. Alternative fixes
Nếu cần transform, có thể transform từng item trước khi yield. Nếu business requirement thật sự cần sort/group toàn bộ dataset, buffering có thể là bắt buộc; khi đó cần làm rõ contract và giới hạn resource thay vì giả định streaming.

## 7. Wrong / tempting fixes
- Chỉ tăng memory limit: trì hoãn symptom nhưng không thay đổi delivery boundary.
- Đổi `List` sang collection khác nhưng vẫn materialize toàn bộ: cơ chế không đổi.
- Chỉ tối ưu `Task.Delay`/producer: tổng thời gian có thể giảm nhưng first-item delivery vẫn bị giữ đến cuối batch.

## 8. Production implications
Streaming giúp giảm time-to-first-item và memory pressure, nhưng giữ connection lâu hơn và cần cancellation/backpressure hợp lý. Proxy hoặc response compression cũng có thể buffering ở layer khác; lab này cô lập application-level boundary trước.

## 9. Trade-offs
Incremental delivery làm partial response trở thành khả năng thực tế nếu lỗi xảy ra giữa stream. API contract cần xác định client xử lý partial data, retry và idempotency như thế nào.

## 10. What a Senior engineer should notice
Đừng suy ra behavior từ type signature. `IAsyncEnumerable<T>` chỉ là khả năng enumerate bất đồng bộ; streaming end-to-end phụ thuộc mọi boundary từ data source, application transforms, serializer, server, proxy đến client. Evidence phải xác định layer nào đang buffering trước khi tối ưu.