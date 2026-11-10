# Reference Solution — spoiler
## Symptoms
Hệ thống báo client disconnect trong khi downstream quá budget.
## Evidence
Case upstream-exceeds-budget actual CLIENT_CANCELLED, expected UPSTREAM_TIMEOUT.
## Root cause
Linked CancellationTokenSource kết hợp các nguồn hủy; OperationCanceledException không cho biết nguồn gốc riêng.
## Why the fix works
[Reference code](Program.cs) kiểm tra callerToken trước, budget.Token sau, rethrow nếu không có nguồn nào bị hủy.
## How to verify
Áp dụng fix vào starter rồi chạy `./verify.ps1`; verify không kiểm tra reference.
## Alternative fixes
Wrapper trả outcome phân loại; WaitAsync(timeout, callerToken) với chính sách quản lý operation nền rõ ràng.
## Wrong fixes
Tăng timeout vô hạn che giấu downstream; retry mọi cancellation lãng phí sau disconnect; gán mọi cancellation thành timeout làm sai metrics.
## Production implications
Log callerCancelled, budgetExpired, traceId và elapsedMs. Cần chính sách cho race khi cả hai token cùng hủy.
## Trade-offs
Ưu tiên caller cancellation tránh retry không cần thiết nhưng có thể che timeout gần đồng thời; với SLO nghiêm ngặt đo dual-cancel riêng.
## Senior engineer should notice
Cancellation là control signal, không phải taxonomy nguyên nhân; classification quyết định alert, retry, SLO và capacity.
