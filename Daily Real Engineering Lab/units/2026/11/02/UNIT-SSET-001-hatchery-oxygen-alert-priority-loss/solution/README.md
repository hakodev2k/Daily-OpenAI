# Reference Solution — chỉ xem sau khi tự reproduce và fix

1. **Symptoms:** Năm cảnh báo hợp lệ nhưng snapshot chỉ giữ ba; hai event khẩn cấp độc lập biến mất.
2. **Evidence:** `Enqueue` trả false cho event mới có cùng Severity; `SortedSet<T>` không xem chúng là khác nhau theo comparer.
3. **Root cause:** Comparer chỉ so sánh `Severity`. Trong `SortedSet<T>`, `Compare(a,b)==0` quyết định sự tương đương phần tử, không chỉ thứ tự.
4. **Why fix works:** So sánh Severity giảm dần, RaisedAt tăng dần và AlertId ordinal làm tie-breaker cuối; khác ID thì không bị gộp.
5. **Verify:** Chỉ sửa `starter/AlertQueue.cs`, chạy `./scripts/verify.ps1`, yêu cầu 4/4 PASS. Có thể kiểm tra riêng mã tham chiếu với `dotnet run --project solution/Reference.csproj -- verify`, không thay cho learner verification.
6. **Alternatives:** Dictionary theo AlertId cộng priority queue nếu cần truy xuất theo ID; PriorityQueue<TElement,TPriority> nếu cần dequeue và tự quản lý uniqueness; database queue cho durability/multi-consumer.
7. **Wrong fixes:** Thêm timestamp ngẫu nhiên che lỗi và mất tính tất định; dùng List<T> có thể phù hợp nhỏ nhưng cần quản lý uniqueness; thêm lock không đổi comparer.
8. **Production implications:** Mất cảnh báo ảnh hưởng SLA và an toàn; cần event identity ổn định, metrics received/retained, persistence và audit.
9. **Trade-offs:** SortedSet thêm O(log n), nhưng không được mutate khóa sắp xếp in-place; nên remove/reinsert hoặc dùng cấu trúc khác. AlertId phải duy nhất theo contract.
10. **Senior engineer notices:** Phân biệt ordering với equality; test invariant, negative case và dữ liệu thiếu; HTTP 200 không chứng minh sự kiện được lưu.

Tài liệu Microsoft: [SortedSet<T>](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sortedset-1), [Comparer<T>.Create](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.comparer-1.create), [StringComparer.Ordinal](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinal).
