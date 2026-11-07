# Reference Solution

> Reference Solution — inspect only after reproducing the issue and attempting your own fix.

## 1. Symptoms
Batch lớn làm peak live memory tăng theo số document và throughput suy giảm.

## 2. Evidence
Mỗi iteration cấp một buffer lớn; collection giữ reference của mọi buffer cho tới cuối batch. `peakLiveBytes` vì vậy tăng gần tuyến tính với document count.

## 3. Root cause
Temporary per-document payload bị kéo dài lifetime tới batch scope dù output contract chỉ cần số document hoàn tất. Kích thước buffer cũng đủ lớn để allocation pattern trở nên đắt hơn trong workload thật.

## 4. Why the fix works
Giữ temporary data ở item scope và chỉ persist output thực sự cần thiết. Ví dụ learner có thể thay collection payload bằng counter/output metadata và để buffer hết reachable sau mỗi iteration. Khi cần I/O thật, stream/chunk processing thường tránh materialize toàn bộ payload cùng lúc.

## 5. How to verify
Chạy `./verify.ps1`. `outputCount` phải vẫn là 40 và `peakLiveBytes` phải dưới threshold của lab.

## 6. Alternative fixes
Buffer pooling có thể giảm allocation churn khi buffer thực sự cần tái sử dụng. Streaming trực tiếp tới destination phù hợp khi output API hỗ trợ. Chia batch thành bounded chunks có thể phù hợp khi downstream cần một nhóm item cùng lúc.

## 7. Wrong or misleading fixes
Gọi `GC.Collect()` trong vòng lặp không giải quyết live references và thường làm pause tệ hơn. Tăng memory limit chỉ trì hoãn symptom. Chuyển sang server GC không sửa lifetime sai. Pooling mà vẫn giữ mọi rented buffer đến cuối batch vẫn giữ working set lớn.

## 8. Production implications
Memory pressure có thể tăng GC frequency, pause time, container eviction risk và giảm throughput dù CPU trung bình nhìn có vẻ bình thường.

## 9. Trade-offs
Streaming giảm peak memory nhưng có thể tăng complexity và thay đổi error/transaction boundary. Pooling giảm allocation nhưng yêu cầu ownership rõ ràng. Chunking cân bằng memory với batching efficiency.

## 10. What a Senior engineer should notice
Đầu tiên phải phân biệt allocation rate với live-set retention. Tối ưu GC trước khi hiểu object lifetime thường nhắm sai tầng. Cần đo peak live data, allocation rate, GC generations và business throughput cùng nhau.