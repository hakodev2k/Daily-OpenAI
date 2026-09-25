# Reference Solution — inspect only after your attempt

## Symptoms
Export đúng dữ liệu nhưng allocation tăng mạnh và GC phải xử lý nhiều object lớn.

## Evidence
Starter tạo một chuỗi lớn rồi tạo thêm một byte array lớn cho mỗi vòng, làm tồn tại nhiều contiguous allocations trong cùng pipeline.

## Root cause
Pipeline materialize toàn bộ payload qua nhiều representation lớn thay vì ghi dần vào output boundary.

## Why the fix works
Streaming giảm số large intermediate objects và giới hạn working set theo buffer.

## How to verify
Chạy verify.ps1 sau khi sửa starter; checksum/functional contract phải còn đúng và allocation phải giảm đáng kể.

## Alternative fixes
Ghi trực tiếp vào response/file stream; dùng pooled buffers khi ownership rõ; chunking nếu downstream hỗ trợ.

## Wrong or tempting fixes
Tăng memory limit hoặc ép GC chỉ trì hoãn triệu chứng. Gọi GC.Collect thủ công làm pause khó đoán hơn.

## Production implications
Theo dõi allocation rate, Gen2/LOH behavior và peak working set cùng throughput.

## Trade-offs
Streaming làm lifecycle/error handling phức tạp hơn và có thể hạn chế retry sau khi output đã commit.

## What a Senior engineer should notice
Performance fix phải giữ correctness contract và đo allocation/GC evidence, không chỉ nhìn CPU.