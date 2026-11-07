# Hint 03
Parallel.ForEachAsync dùng ParallelOptions.MaxDegreeOfParallelism. Giới hạn dựa trên Environment.ProcessorCount có thể không phù hợp với I/O. Chọn bounded concurrency dựa trên partner quota và headroom, giữ cancellation.
