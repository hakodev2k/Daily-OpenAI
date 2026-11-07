# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
24 hồ sơ hoàn thành đủ nhưng batch chậm dù operator yêu cầu concurrency 8.

## Evidence
requestedConcurrency=8, nhưng effectiveConcurrency và maxActive chỉ 1–2. Không có rejected, missing, duplicate. Với 120 ms mỗi I/O, 24 hồ sơ cần khoảng 12–24 đợt thay vì 3 đợt.

## Root cause
Starter dùng Math.Min(2, Math.Max(1, Environment.ProcessorCount)) để đặt MaxDegreeOfParallelism và bỏ qua tham số CLI. Heuristic giới hạn theo CPU không phù hợp với batch chủ yếu chờ I/O.

## Why the fix works
Dùng Math.Clamp(requested, 1, partnerCapacity) làm giới hạn Parallel.ForEachAsync. Cấu hình operator được áp dụng trong ngưỡng an toàn, không vượt 8 request in-flight. Await bất đồng bộ không giữ thread bận suốt thời gian chờ.

## How to verify
Chạy ./verify.ps1 tại root unit. Harness chỉ chạy starter đã sửa với N=3, 8, 16, kiểm tra capacity và tính đúng dữ liệu. Có thể tự chạy reference bằng dotnet run --project solution/PermitOcrBatch.csproj -- --max-concurrency 8, nhưng không thay thế learner verification.

## Alternative fixes
Bounded Channel với consumer cố định nếu cần queue/backpressure; SemaphoreSlim nếu nhiều producer cùng gọi downstream; adaptive concurrency nếu có đủ telemetry.

## Wrong / tempting fixes
Task.WhenAll không giới hạn dễ gây rejected/429; Task.Run quanh I/O không tăng capacity; tăng CPU/replica trước khi đo không sửa policy; tăng timeout chỉ che triệu chứng. Mỗi phương án có thể hợp lý ở bối cảnh khác, không phải fix mặc định ở đây.

## Production implications
Concurrency khác request rate. Giới hạn 8 in-flight không đảm bảo quota RPS. Production cần timeout, cancellation, retry có idempotency, telemetry p95, quota cấp account và xử lý partial failure.

## Trade-offs
Concurrency cao giảm wall-clock nhưng tăng áp lực downstream, connection pool và rủi ro overload. Clamp đơn giản dễ audit; adaptive policy linh hoạt nhưng phức tạp hơn.

## Senior engineer nên nhận ra
Đo số request in-flight trước khi scale, kiểm tra config thực sự có hiệu lực, tách throughput khỏi correctness, thử cả cấu hình thấp và cấu hình vượt giới hạn.

## Official references
Microsoft Learn: Parallel.ForEachAsync — https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.parallel.foreachasync
Microsoft Learn: ParallelOptions.MaxDegreeOfParallelism — https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.paralleloptions.maxdegreeofparallelism
