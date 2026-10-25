# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms
Các alert có cùng severity đôi khi được dispatch khác thứ tự đến, dù alert severity cao hơn vẫn thường được ưu tiên đúng.

## Evidence
Bốn alert A,B,C,D có cùng severity được enqueue theo thứ tự này. reproduce in expected A,B,C,D nhưng actual khác; verify phát hiện vi phạm contract. Không có mutation của input và không có concurrency trong case này.

## Root cause
System.Collections.Generic.PriorityQueue<TElement,TPriority> đảm bảo phần tử có priority nhỏ nhất được lấy trước, nhưng **không đảm bảo stable ordering khi priority bằng nhau**. Priority chỉ gồm Severity nên nhiều alert có priority không phân biệt được. Cấu trúc heap có thể thay đổi thứ tự giữa các phần tử cùng priority.

## Why the fix works
Bản reference trong solution/Program.cs sử dụng priority dạng (Severity, Sequence), so sánh lexicographic: Severity nhỏ hơn thắng; nếu bằng nhau, Sequence nhỏ hơn thắng. Sequence được cấp tăng dần khi nhận alert, nên mỗi phần tử có priority khác biệt và đúng arrival contract.

## How to verify
~~~powershell
./scripts/verify.ps1
~~~
Lệnh này chạy **starter** sau khi người học sửa. Để tự kiểm tra reference độc lập:
~~~bash
dotnet run --project solution/BaggageDispatch.csproj -- verify
~~~
Expected: tất cả PASS. Reproduce chạy trên starter sau fix trả 2, đúng theo contract của script.

## Alternative fixes
- Dùng nhiều FIFO queue, mỗi severity một queue: dễ hiểu nếu số severity ít và cố định, nhưng phức tạp hơn khi severity động.
- Dùng SortedDictionary<int, Queue<BaggageAlert>>: FIFO theo từng bucket, O(log k) cho quản lý bucket; cần xử lý đồng bộ nếu có nhiều producer.
- Dùng priority composite như reference: thay đổi nhỏ, tận dụng API hiện tại, O(log n) cho enqueue/dequeue.

## Wrong / tempting fixes
- Chỉ tăng capacity hoặc đổi sang concurrent collection: không thêm cam kết stable tie ordering.
- Sắp xếp theo alert Id: Id không nhất thiết phản ánh thời điểm nhận; sai contract khi ID đến không theo thứ tự từ điển.
- Gọi OrderBy(x => x.Severity) sau dequeue: lúc đó thông tin arrival order đã mất; stable sort trên một chuỗi đã bị đảo không thể khôi phục thứ tự gốc.
- Thêm Task.Delay giữa các enqueue: thay đổi timing không giải quyết contract so sánh.

## Production implications
Sequence phải được cấp tại một điểm nhận thống nhất; với nhiều producer, Interlocked.Increment tạo thứ tự tuyến tính theo lúc cấp sequence, không nhất thiết theo timestamp trên mạng. Với nhiều instance, local sequence không tạo global FIFO: cần partition ownership, durable sequence từ broker hoặc quyết định contract per-partition. Sau restart, persistence và replay cần giữ sequence; kiểm tra overflow, duplicate events và tie-breaker scope.

## Trade-offs
Composite priority tăng metadata và yêu cầu định nghĩa rõ arrival boundary, nhưng ít thay đổi cấu trúc. Bucketed FIFO queue phù hợp khi priority hữu hạn và cần điều phối theo từng severity. Nếu thứ tự phải bền vững qua crash, cả hai giải pháp in-memory đều không đủ; cần persistence/acknowledgement.

## What a Senior engineer should notice
Một data structure có thể đúng cho invariant ưu tiên nhưng sai cho invariant ổn định. Viết contract dưới dạng hai thuộc tính độc lập, kiểm thử các tie và các thứ tự xen kẽ, sau đó định nghĩa chính xác thế nào là arrival khi có concurrency hoặc distributed consumers.
