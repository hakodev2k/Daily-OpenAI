# Before — starter

Chạy ./reproduce.ps1: PASS NORMAL_REPEAT, FAIL COLLISION_FORWARD, FAIL COLLISION_REVERSE, FAIL MISSING_ORIGIN, RESULT 1/4, exit 12. Đây là symptom đã được xác định, không phải build failure.

Nếu không reproduce: xác nhận đang đứng ở thư mục unit, .NET 8 SDK được chọn, chưa thay đổi starter/BriefingCache.cs, và không chạy nhầm solution.
