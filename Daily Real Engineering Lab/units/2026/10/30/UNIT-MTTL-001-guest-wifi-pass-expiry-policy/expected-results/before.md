# Before — starter gốc

reproduce.ps1 phải trả exit code 0 vì ba case expires-at-boundary, expired-but-present, revoked-but-present đều FAIL (actual ALLOW, expected DENY). Các case active-before-expiry, cleanup-eventually-removes, missing-document, future-revocation-not-yet-effective PASS.

Nếu tất cả case PASS ngay, có thể starter đã được sửa; khôi phục AccessPolicy.cs gốc rồi chạy lại.
