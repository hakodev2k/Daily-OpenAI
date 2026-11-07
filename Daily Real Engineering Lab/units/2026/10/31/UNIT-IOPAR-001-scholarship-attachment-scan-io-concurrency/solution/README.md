# Reference Solution — chỉ đọc sau khi reproduce và tự sửa

## 1. Symptoms
Batch 24 tệp không tận dụng đủ quota 8. Tăng concurrency vô hạn có thể gây throttling ở quota 1.

## 2. Evidence
Peak của starter không vượt 2, trong khi scanner giả lập I/O wait 80ms. CPU thấp không chứng minh cần scale CPU.

## 3. Root cause
AttachmentBatch.ScanAsync chọn MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount, 2), không xét downstream capacity. Đây là CPU-oriented policy cho workload I/O-bound.

## 4. Why the fix works
solution/Program.cs dùng Min(6, Max(1, scanner.Capacity - 1)). Quota 8 cho 6 slot có headroom; quota 1 cho đúng 1 slot. CancellationToken vẫn được truyền. Đây là policy mẫu theo contract lab, không phải hằng số tối ưu cho mọi production.

## 5. How to verify
Sửa starter/Program.cs và chạy scripts/verify.ps1, phải có CAPACITY_8=PASS và CAPACITY_1=PASS. Chạy reference riêng bằng dotnet run --project solution/AttachmentScanLab.csproj -- --check chỉ để đối chiếu, không thay thế verify.ps1.

## 6. Alternative fixes
SemaphoreSlim theo quota hoặc bounded Channel + worker pool khi cần queue/backpressure. Parallel.ForEachAsync đơn giản hơn cho batch hữu hạn.

## 7. Wrong fixes
- Scale CPU ngay: không giải quyết admission I/O.
- MaxDegreeOfParallelism=-1: có thể vượt quota và gây 429.
- Hard-code 8 cho mọi tenant: partner yếu bị quá tải.
- Chỉ tăng timeout: không cải thiện throughput.
- Bỏ qua false/429: báo cáo hoàn tất sai.

## 8. Production implications
Theo dõi active requests, p50/p95, 429, throughput, queue depth, cancellation và quota theo partner. Quota động cần admission policy cập nhật, backoff có giới hạn.

## 9. Trade-offs
Concurrency cao tăng throughput nhưng gây áp lực lên downstream. Concurrency thấp giảm throttling nhưng kéo dài backlog. Cân bằng SLA, quota và chi phí.

## 10. Senior engineer should notice
Phân biệt CPU parallelism, I/O in-flight và downstream quota. Kiểm tra cả underutilization và overutilization; không dùng elapsedMs tuyệt đối trên máy dev làm SLA.

## Official references
- https://learn.microsoft.com/dotnet/api/system.threading.tasks.parallel.foreachasync
- https://learn.microsoft.com/dotnet/api/system.threading.tasks.paralleloptions.maxdegreeofparallelism
- https://learn.microsoft.com/dotnet/standard/parallel-programming/task-based-asynchronous-programming
