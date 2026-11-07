# Reference Solution — chỉ mở sau khi tự điều tra

## 1. Symptoms
Hai request hợp lệ được accepted và enqueue; worker xử lý sau đó báo `AUDIT_TENANT_CONTEXT_MISSING`. Tăng worker không sửa được lỗi.

## 2. Evidence
`contract/Program.cs` tạo hai request với claim tenant A/B rồi kết thúc mỗi request trước khi drain queue. Trên starter gốc, factory của audit record thất bại khi worker thực thi; request ingress đã có claim hợp lệ. Đây không phải lỗi database hay CPU.

## 3. Root cause
`starter/AuditDispatchService.cs` enqueue một closure đọc `IHttpContextAccessor.HttpContext` **khi worker chạy**, thay vì lấy tenant lúc request còn sống. `HttpContext` là ambient context có vòng đời request, không phải durable payload. Sau khi request kết thúc, accessor có thể trả null; nếu tái sử dụng nhầm context còn có nguy cơ gán sai tenant.

## 4. Why the fix works
[AuditDispatchService.cs](AuditDispatchService.cs) lấy và validate `tenant_id` trước khi enqueue, tạo `AuditRecord` bất biến và chỉ enqueue factory trả về snapshot đó. Worker không phụ thuộc vào `HttpContext` hoặc request DI scope; request thiếu tenant bị từ chối sớm.

## 5. How to verify
Chỉ sửa `starter/AuditDispatchService.cs`, chạy `./scripts/verify.ps1`. Contract reference tới `starter/Starter.csproj`, không tới solution. Kiểm tra cả multi-tenant và missing-tenant negative path; exit 0 là pass. Build lời giải riêng bằng `dotnet build ./solution/Solution.csproj -c Release`.

## 6. Alternative fixes
- Đưa một DTO bất biến (`PermitId`, `TenantId`, correlation ID) trực tiếp vào channel hoặc durable queue thay vì `Func<AuditRecord>`: phù hợp production hơn.
- Với hệ thống không cần async audit, ghi audit trong cùng request/transaction: giảm độ trễ nhất quán nhưng tăng latency và coupling.
- Durable outbox cho audit quan trọng: đảm bảo atomicity với nghiệp vụ, đổi lại tăng DB/worker complexity.

## 7. Wrong or misleading fixes
- Tăng số worker hoặc delay worker: không thay đổi vòng đời `HttpContext`.
- Truyền `HttpContext` trực tiếp vào queue: giữ reference tới đối tượng không còn hợp lệ; rủi ro concurrency và leak.
- Dùng `Task.Run` để đọc accessor sớm hơn: tạo race và không đảm bảo durability.
- Dùng tenant mặc định khi claim thiếu: tạo audit sai chủ thể, đặc biệt nguy hiểm ở multi-tenant.
- Bắt exception rồi bỏ audit: biến lỗi quan sát được thành mất dữ liệu âm thầm.

## 8. Production implications
Chụp chỉ những trường cần thiết và đã được authorize; không serialize `ClaimsPrincipal` hay toàn bộ `HttpContext`. Với audit có tính tuân thủ, in-memory queue không đủ durability: cân nhắc outbox, retry, deduplication và observability riêng cho accepted vs persisted.

## 9. Trade-offs
Snapshot nhỏ giảm coupling và giữ đúng identity; cần xác định tenant nào là authoritative nếu quyền thay đổi sau enqueue. Với công việc có thời gian chờ dài, có thể phải re-authorize hoặc kiểm tra phiên bản chính sách tại lúc thực thi. Outbox thêm độ tin cậy nhưng tăng vận hành.

## 10. What a Senior engineer should notice
`202 Accepted` không đồng nghĩa background side effect đã hoàn tất. Thiết kế contract ở admission boundary, phân biệt identity snapshot với authorization tại execution, và test negative path sau khi request scope kết thúc. Đừng nhầm tăng capacity với sửa correctness.

Tài liệu chính thức: [ASP.NET Core HttpContext](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/use-http-context), [Background tasks with hosted services](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services).
