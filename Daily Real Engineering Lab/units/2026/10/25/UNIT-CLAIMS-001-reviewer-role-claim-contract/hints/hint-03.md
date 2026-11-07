# Hint 03 — Khái niệm cần thử

`ClaimsIdentity.RoleClaimType` quyết định claim nào được `ClaimsPrincipal.IsInRole` nhận diện. Với payload đang mô phỏng, role được phát hành dưới claim type `roles`. Hãy thử khai báo role claim type phù hợp tại điểm tạo trusted identity; giữ nguyên policy và negative tests.
