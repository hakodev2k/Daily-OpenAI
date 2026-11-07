# Reference Solution — chỉ xem sau khi đã tự thử

## 1. Symptoms
Ruleset provider lỗi transient đúng một lần. Những request tiếp theo vẫn nhận 503 dù provider có thể trả thành công. Khởi động lại process khiến hiện tượng biến mất.

## 2. Evidence
Reproduce in FIRST_503=True, SECOND_503=True, PROVIDER_CALLS=1. Hai exception được quan sát nhưng chỉ có một lần fetch. Điều này bác bỏ giả thuyết upstream tiếp tục trả 503 cho từng request.

## 3. Root cause
Starter dùng Lazy<Task<Ruleset>>. Lazy<T> chỉ tạo giá trị một lần; giá trị ở đây là Task, không phải ruleset thành công. Khi fetch đầu tiên faulted, Task faulted vẫn được giữ lại và trả về mọi lần sau. ExecutionAndPublication đảm bảo chỉ một factory chạy, nhưng không tự khởi tạo lại Task đã faulted.

## 4. Why the fix works
solution/RulesetSnapshotCache.cs giữ Task hiện hành sau một lock. Nếu chưa có Task, hoặc Task trước đã faulted/canceled, nó tạo tác vụ mới. Khi Task hoàn tất thành công, các caller tiếp tục dùng cùng Task. Lock ngăn nhiều cold callers tạo nhiều fetch. Không giữ lock qua await hay I/O.

## 5. How to verify
Sau khi tự thử, so sánh hoặc copy phần triển khai từ solution/RulesetSnapshotCache.cs sang starter/RulesetSnapshotCache.cs rồi chạy ./verify.ps1. Script kiểm chứng learner-editable starter. Tự chạy reference bằng dotnet run --project ./solution/RulesetLab.csproj -- verify.

## 6. Alternative fixes
Có thể dùng Lazy<Task<T>> với compare-and-swap để thay thế instance sau lỗi, nhưng phải xử lý race giữa nhiều caller. Có thể dùng async lock hoặc cache library hỗ trợ invalidation. Với một ruleset toàn process, lock ngắn và một Task field dễ kiểm chứng.

## 7. Wrong / tempting fixes
- Restart process sau 503: che giấu lỗi phục hồi và gây downtime.
- Retry trong provider nhưng giữ mãi Task faulted sau khi hết retry budget: lỗi vẫn tồn tại.
- Bỏ đồng bộ, fetch cho mỗi request: phục hồi nhưng gây thundering herd.
- Cache exception mãi: chỉ hợp lý nếu failure là terminal, không phải transient.

## 8. Production implications
Phân biệt transient/permanent errors; đặt timeout/backoff ở provider; ghi metrics fetch_attempts, fetch_failures, snapshot_age. Quyết định fail-closed hay dùng last-known-good theo compliance và business policy. Không log dữ liệu nhạy cảm.

## 9. Trade-offs
Sau lỗi, caller kế tiếp thử lại ngay. Khi outage kéo dài cần backoff/circuit breaker. Cache thành công vô thời hạn chỉ phù hợp snapshot bất biến; nếu cần refresh, bổ sung TTL và atomic snapshot swap.

## 10. What a Senior engineer should notice
Single-flight không đồng nghĩa failure recovery. Xác định lifecycle của successful value, faulted Task, cancellation, cache invalidation và upstream recovery. Kiểm chứng correctness lẫn concurrency invariant.

## Static-review scope
Source và script đã được kiểm tra nhất quán bằng static review; chưa chạy dotnet restore, build hoặc test trong môi trường tạo lab.
