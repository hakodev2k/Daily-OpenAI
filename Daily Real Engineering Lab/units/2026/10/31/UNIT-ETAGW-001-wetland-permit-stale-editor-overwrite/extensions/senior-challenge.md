# Senior Challenge
1. Thay store bằng SQL có Version, thiết kế conditional UPDATE, map affected rows = 0 thành 412.
2. Integration test với hai connection/instance thật và cùng một ETag: chỉ một ghi thành công.
3. UX khi nhận 412: reload, diff, merge hoặc hủy; không tự động ghi đè.
4. Bổ sung If-Match list, wildcard và validation syntax theo RFC 9110.
