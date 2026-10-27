# Reference Solution — chỉ xem sau khi tự điều tra

## Symptoms
Routing snapshot chuyển sang WEST nhưng request thứ hai và ba vẫn trả EAST với HTTP 200; hệ thống nhìn như đã cutover nhưng traffic không đổi.

## Evidence
Fixture giữ HTTP/1.1 keep-alive. Baseline mở một connection và tái sử dụng nó cho nhiều request; `ConnectCallback` chỉ chạy khi tạo connection mới. Route snapshot không được tra cứu lại cho connection đã mở.

## Root cause
`HttpClient`/`SocketsHttpHandler` tái sử dụng TCP connection. DNS (hoặc route resolver mô phỏng ở đây) được tra cứu ở bước tạo connection, không tự áp lại cho connection đang sống. Healthy persistent traffic có thể giữ upstream cũ lâu hơn mong đợi.

## Why the fix works
Mã tham khảo cấu hình `PooledConnectionLifetime` để sau một khoảng thời gian connection cũ không được dùng cho request mới. Fixture chờ 300 ms sau cutover nên lần gửi tiếp theo tạo connection và resolver đọc route WEST. Connection đang xử lý request không bị cắt ngang chỉ vì quá lifetime; retirement diễn ra sau khi sử dụng xong.

## How to verify
Chạy `./verify.ps1` trên starter do bạn chỉnh. Sau đó có thể chạy `dotnet run --project solution/TransitGatewayLab.csproj -- --verify` để kiểm tra fixture tham khảo. Kỳ vọng EAST, WEST, WEST và ít nhất hai connection attempts. Chạy lại với delay nhỏ hơn lifetime để hiểu tại sao fix không bảo đảm cutover tức thời.

## Alternative fixes
- Restart/recreate handler sau một sự kiện cutover có kiểm soát: chuyển nhanh nhưng làm mất pooling và cần coordination.
- Dùng handler lifetime qua `IHttpClientFactory`: hợp lý khi có DI, nhưng cần phân biệt handler recycling với pooled connection lifetime và kiểm tra workload.
- Chủ động failover khi lỗi transport: chỉ hữu ích khi upstream cũ lỗi; không giải quyết stale-but-healthy response nếu không có kiểm tra version/region.

## Wrong / tempting fixes
- Giảm `PooledConnectionIdleTimeout` rồi kỳ vọng xử lý traffic liên tục: connection không idle.
- Tạo `new HttpClient()` cho từng request: có thể ép kết nối mới nhưng gây connection churn và rủi ro socket exhaustion.
- Thêm retry ngay khi HTTP 200: không có tín hiệu thất bại và có thể tiếp tục cùng upstream.
- Đặt lifetime vài millisecond trong production: tăng TLS handshake, CPU, latency và tải upstream.

## Production implications
Thiết lập lifetime dựa trên DNS TTL/operational cutover SLO, cân bằng connection reuse với thời gian hội tụ. Theo dõi upstream identity, connection age, connect rate, DNS/route update, p95 latency, error budget và request idempotency. `PooledConnectionLifetime` không bảo đảm mọi request đang chạy chuyển ngay lập tức; HTTP/2 multiplexing và proxy/load balancer cần kiểm thử riêng.

## Trade-offs
Lifetime ngắn: nhanh nhận endpoint mới nhưng tăng connect/TLS overhead. Lifetime dài: tiết kiệm tài nguyên nhưng chậm chuyển tuyến. Với upstream có state, cần thiết kế readiness/drain và hợp đồng dữ liệu, không chỉ tuning HTTP client.

## What a Senior engineer should notice
Fixture cố ý dùng resolver giả để không phụ thuộc DNS cache của OS và TTL thực. Nó chứng minh connection lifecycle, **không** mô phỏng mọi tầng DNS. Chỉ kết luận về DNS thực sau khi quan sát resolver, socket, proxy và load balancer trong môi trường triển khai.

## Official references
- https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
- https://learn.microsoft.com/en-us/dotnet/api/system.net.http.socketshttphandler.pooledconnectionlifetime
- https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory
