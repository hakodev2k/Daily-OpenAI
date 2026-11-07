# Senior extension — clock domains và distributed tracing
Đối tác trả kết quả từ host khác; SLA end-to-end 2 giây, cutoff nghiệp vụ 18:00Z. Thiết kế per-hop monotonic duration, UTC event timestamps, correlation ID, acceptance tests cho clock skew và timeout. Giải thích vì sao không trừ hai monotonic timestamps từ hai host.
