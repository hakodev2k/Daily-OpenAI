# Reference Solution — chỉ đọc sau khi tự quyết định

## Symptoms
API p95 tăng trong khi App egress tăng mạnh, storage latency tương đối ổn định; đây là bằng chứng nghiêng về chi phí proxy/streaming, **chưa chứng minh** một bottleneck duy nhất.

## Evidence
Ở đỉnh, 90 × 6 MiB = 540 MiB/s là upper-style workload estimate theo kích thước trung bình; measured egress 490 MiB/s, sai khác có thể do phân phối kích thước, aborted downloads và sampling. Phải đo riêng request authorization p95, time-to-first-byte, duration và bytes.

## Root cause / decision tension
Proxying toàn bộ nội dung qua App Service làm tăng tải dữ liệu qua compute tier. Chuyển toàn bộ sang bearer delegated URL giảm egress nhưng làm thay đổi revocation semantics và audit visibility. Không coi sự kiện 401 của IdP là bằng chứng nguyên nhân.

## One defensible design
Giữ endpoint cấp quyền .NET 8 làm authoritative gate: xác thực principal, truy vấn quan hệ phụ huynh–học sinh **tại thời điểm cấp**, map logical document ID sang blob path nội bộ, kiểm tra tenant, ghi audit grant có correlation ID, sau đó cấp **user delegation SAS read-only cho một blob** với expiry tối đa 45 giây, HTTPS-only và storage private. Browser tải trực tiếp từ storage, không cache shared; giới hạn Content-Disposition và Referrer-Policy phù hợp. Không dùng account key trong ứng dụng nếu có Managed Identity/user delegation. Thời gian hiệu lực tính cả clock skew; nếu 60 giây là hard deadline cần budget chặt hơn và test clock drift. SAS đã cấp là bearer credential: không thể đảm bảo dừng ngay một download đang chạy, và audit cấp quyền không tương đương audit hoàn tất tải.

## Why it works
Giảm băng thông qua App Service, đồng thời chặn cấp token mới sau khi quan hệ bị thu hồi. TTL ngắn giới hạn cửa sổ bắt đầu download mới; kiểm tra grant trước mỗi issuance. Nếu yêu cầu **thu hồi tức thì từng request** hoặc kiểm soát từng byte, giữ proxy/gateway có authorization liên tục thay vì hứa SAS làm được.

## Verification
1. Revoked parent requests fresh token → 403; old token after 60s (+ defined skew tolerance) cannot initiate new read.
2. Token for document A cannot read B; cross-tenant and path enumeration denied.
3. Expired, malformed, replayed within TTL, and leaked tokens assessed: replay within TTL **có thể thành công**; giảm exposure qua TTL và telemetry, không tuyên bố single-use.
4. Storage 503 → có lỗi có kiểm soát, không fallback sang public URL.
5. Audit sink outage → chọn fail-closed nếu compliance yêu cầu audit bắt buộc, hoặc durable local outbox với giới hạn backlog và alert; nêu rõ quyết định.
6. Compare issuance p95 and App egress before/after canary 5% → 25% → 100%; kiểm tra 403 và 5xx.

## Alternative fixes
- Proxy toàn bộ: dễ kiểm soát thu hồi và download audit hơn nhưng tốn egress/compute; hợp lý cho hồ sơ đặc biệt nhạy cảm.
- Hybrid: proxy tài liệu có chính sách thu hồi tức thì, SAS TTL ngắn cho phần còn lại.
- CDN private tokenization: cân nhắc sau khi có evidence về cacheability và compliance, không phải default.

## Wrong / tempting fixes
- Tăng App instances ngay: có thể giảm p95 nhưng không giải quyết chi phí double-hop hay token leakage.
- SAS hết hạn 24 giờ: giảm tần suất issuance nhưng phá guarantee 60 giây.
- Public blob với URL khó đoán: obscurity không phải authorization.
- Ghi audit lúc cấp token rồi tuyên bố đã tải thành công: nhầm grant với actual read.

## Production implications and trade-offs
Managed Identity, key rotation policy, short-lived SAS, structured redacted logs (không log SAS query), rate limits cho issuance, storage diagnostics, correlation IDs, error budgets, network access policies và chi phí egress cần được đánh giá. User delegation SAS có giới hạn về network/firewall và các cấu hình storage; kiểm chứng trên môi trường triển khai.

## What a Senior engineer should notice
Ranh giới trust, semantics thu hồi, sự khác nhau giữa authorization và actual access, điều kiện rollback và dữ liệu cần thu trước khi quyết định; tối ưu p95 không được đánh đổi quyền riêng tư không kiểm soát.
