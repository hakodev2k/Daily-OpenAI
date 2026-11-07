# Hint 2

Endpoint thay đổi output dựa trên `X-Tenant-Id`, nhưng hãy kiểm tra xem cache key hiện tại có biết tới input này không.

Tập trung vào cấu hình `OutputCachePolicyBuilder` thay vì thay đổi business logic trong endpoint.
