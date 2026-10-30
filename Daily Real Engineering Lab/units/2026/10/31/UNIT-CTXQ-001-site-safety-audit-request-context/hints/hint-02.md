# Hint 02 — Phân biệt dữ liệu và môi trường

Kiểm tra `IHttpContextAccessor` trong `starter/AuditDispatchService.cs`. Thử tìm thời điểm thực sự gọi `FindFirst("tenant_id")`, rồi so sánh với lúc contract đặt `HttpContext = null`.
