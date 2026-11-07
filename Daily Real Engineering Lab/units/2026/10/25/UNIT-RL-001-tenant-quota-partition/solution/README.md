# Reference Solution — SPOILER

Chỉ đọc sau khi đã reproduce, ghi hypothesis và thử sửa starter.

## 1. Symptoms
Tenant Beta bị từ chối dù chưa dùng quota; Alpha lại có thêm ngân sách khi gọi một endpoint khác. Scenario D vẫn xanh nên test một tenant/một route không phát hiện sai lệch.

## 2. Evidence
A và B cho kết quả trái chiều theo trục tenant/route. C cho thấy anonymous có thể đổi endpoint để nhận thêm permit. D xác nhận cơ chế đếm và giới hạn 2 permits tự nó vẫn hoạt động.

## 3. Root cause
`RateLimitPartition` nhóm các request theo **partition key**. Starter chọn path làm key. Vì vậy Alpha và Beta trên cùng path cùng tiêu thụ một bucket; Alpha trên path khác lại có bucket mới. Đây là lỗi lựa chọn identity cho quota, không phải lỗi timer hay HTTP status mapping.

## 4. Why the fix works
[Code tham chiếu](TenantQuotaPolicy.cs) chọn claim `tenant_id` từ `HttpContext.User` đã được authentication middleware xác lập; prefix `tenant:` ngăn trùng khóa với `anonymous`. Tất cả endpoints của cùng tenant dùng chung bucket; tenant khác có bucket riêng.

## 5. How to verify
Chỉ sửa `starter/TenantQuotaPolicy.cs`, sau đó chạy `.\scripts\verify.ps1`. Cả A–D phải pass. Không chỉnh expected assertions. `reproduce.ps1` chỉ dùng để kiểm chứng starter ban đầu; sau khi fix nó sẽ không còn tái hiện lỗi.

## 6. Alternative fixes
- Partition theo authenticated user nếu hợp đồng quota thực sự là per-user, không phải per-tenant.
- Một rate limiter ở API gateway có tenant-aware auth context nếu quota phải đồng nhất qua nhiều replicas.
- Distributed limiter / centralized quota store khi cần giới hạn toàn cụm, nhưng phải tính latency, consistency, availability và operational cost.

## 7. Wrong or misleading fixes
- Tăng `PermitLimit`: che giấu 429 ở A nhưng không giải quyết việc Alpha né quota bằng route khác.
- Dùng `RemoteIpAddress`: nhiều tenant cùng NAT vẫn chia sẻ quota; proxy và IPv6 khiến identity không ổn định.
- Dùng header `X-Tenant-Id` từ client: dễ giả mạo nếu không có lớp xác thực/ủy quyền ràng buộc.
- Reset limiter sau mỗi request: làm mất mọi giới hạn; test có thể xanh giả.
- Chỉ viết thêm test D: D vốn đã xanh, không kiểm chứng tenant isolation.

## 8. Production implications
Bài lab gọi limiter trực tiếp; middleware ASP.NET Core cần được cấu hình chính xác, ví dụ `options.GlobalLimiter = TenantQuotaPolicy.Create()`, và đặt authentication trước khi policy đọc claims. Xem lại ordering của `UseAuthentication`, `UseAuthorization` và `UseRateLimiter` theo kiểu policy. Khi chạy nhiều pod, bộ đếm in-memory là **per-instance**, không đảm bảo quota toàn cụm.

## 9. Trade-offs
Quota per-tenant bảo vệ công bằng giữa doanh nghiệp nhưng tenant lớn có thể cần burst/queue hoặc tier riêng. Anonymous fallback dùng một bucket chung đơn giản nhưng dễ bị một client gây starvation cho client khác; production có thể cần phân tầng theo network identity đã chuẩn hóa và anti-abuse policy. Fixed window cho phép burst gần ranh giới cửa sổ; sliding window/token bucket có đặc tính khác. Không chọn Redis chỉ vì đã có 429.

## 10. Senior engineer should notice
Hợp đồng business nói **quota thuộc về ai** phải dẫn đến partition key. Identity là security boundary: claim phải được xác thực và tenant phải được authorize cho tài nguyên. Cần test cả **isolation giữa principals** và **aggregation xuyên routes**, thêm metrics theo policy/tenant group nhưng tránh nhãn cardinality cao. Khi scale-out, phân biệt tính đúng đắn của một instance với bảo đảm toàn hệ thống.

## Verification scope
Đây là mã tham chiếu được rà soát tĩnh, chưa có bằng chứng đã thực thi trong run tạo lab. Kiểm tra lại với SDK .NET 8 trong môi trường của bạn.
