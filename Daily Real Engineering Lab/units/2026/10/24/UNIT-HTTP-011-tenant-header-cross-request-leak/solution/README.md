# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Baseline tuần tự đúng; kịch bản overlapping gán header `tenant-beta` cho cả hai order.

## 2. Evidence
`RecordingHandler` đọc header từ từng `HttpRequestMessage` ngay lúc `SendAsync`; không có proxy, network hay server cache. `aReady` và `bReady` áp đặt thứ tự thay vì trông chờ race ngẫu nhiên.

## 3. Root cause
`HttpClient.DefaultRequestHeaders` là trạng thái mặc định **chia sẻ theo instance**. Starter thay đổi trạng thái này trên từng lời gọi, nhưng request A chưa gửi khi B thay đổi default header. Khi A gửi, nó thừa hưởng header của B. Không được chỉnh `DefaultRequestHeaders` khi có các request đang hoạt động.

## 4. Why the fix works
Đặt `X-Tenant-Id` trên **từng** `HttpRequestMessage.Headers`. Request A và B giữ ngữ cảnh riêng, bất kể thứ tự await và thời điểm gửi. Dùng chung `HttpClient` vẫn hợp lý để tái sử dụng connection pool.

## 5. How to verify
Sau khi lưu bản sửa vào `starter/PartnerClient.cs`, chạy `./scripts/verify.ps1`. Cả serial và overlap đều phải có hai mapping chính xác; exit code 0.

## 6. Alternative fixes
Tạo client cấu hình sẵn theo tenant có thể phù hợp nếu số tenant hữu hạn và lifecycle được quản lý rõ, nhưng thường gây bùng nổ handler/client hoặc cấu hình. Per-request header đơn giản và dễ kiểm chứng hơn.

## 7. Wrong or misleading fixes
- `lock` quanh thao tác đổi default header: nếu lock không bao cả network send thì vẫn sai; giữ lock qua I/O làm nghẽn throughput.
- Tạo `new HttpClient()` cho mỗi request: che giấu race nhưng tăng rủi ro socket/connection exhaustion.
- Thêm `Task.Delay`: chỉ đổi xác suất, không bảo đảm isolation.
- Đưa tenant vào `static AsyncLocal` mà không kiểm soát propagation: tăng độ phức tạp; vẫn phải ánh xạ chính xác vào request.

## 8. Production implications
Sai tenant có thể gây data leakage, vi phạm authorization và audit trail. Server đối tác phải xác thực quyền tenant độc lập, không chỉ tin header do client gửi. Không log token hoặc PII; dùng correlation ID để truy vết.

## 9. Trade-offs
Per-request `HttpRequestMessage` cần quản lý `Dispose` và payload ownership, nhưng giúp ownership rõ ràng. `IHttpClientFactory` hỗ trợ quản lý handler lifetime, **không** tự động giải quyết lỗi shared mutable request metadata.

## 10. What a Senior engineer should notice
Phân biệt thread-safe sending với thread-safe mutation của shared configuration. Viết regression test với interleaving có điều khiển, không chỉ stress test may rủi. Bảo vệ isolation cả phía client và server.

## Drop-in source
Tham khảo [PartnerClient.cs](PartnerClient.cs); chỉ copy sang `starter/` sau khi đã thử tự sửa.
