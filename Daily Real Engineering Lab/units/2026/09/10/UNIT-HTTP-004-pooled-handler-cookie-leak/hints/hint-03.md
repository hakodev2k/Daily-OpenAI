# Hint 3

Kiểm tra semantics của handler pooling trong `IHttpClientFactory` và state của automatic cookie handling. Với multi-tenant gateway, cân nhắc loại bỏ automatic cookie state khỏi pooled transport và quản lý cookie theo request/session boundary rõ ràng.
