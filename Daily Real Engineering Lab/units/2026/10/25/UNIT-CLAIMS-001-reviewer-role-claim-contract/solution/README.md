# Reference Solution — SPOILER

**Chỉ đọc sau khi đã reproduce và tự thử sửa.**

## Symptoms
Principal có `roles:Reviewer` và `IsAuthenticated=true`, nhưng role-based policy trả denied; principal chỉ có `Reader` vẫn denied.

## Evidence
Đối chiếu `ClaimsPrincipal.Claims`, `ClaimsIdentity.RoleClaimType`, `ClaimsPrincipal.IsInRole("Reviewer")` và kết quả `IAuthorizationService`. Dữ liệu role có mặt, nhưng cách identity phân loại role không tương ứng với kiểu claim được phát hành.

## Root cause
Constructor `new ClaimsIdentity(claims, authenticationType: "Bearer")` dùng `ClaimTypes.Role` mặc định (URI chuẩn .NET) làm `RoleClaimType`. Provider mô phỏng phát hành `roles`, không phải `ClaimTypes.Role`. `RequireRole("Reviewer")` dựa vào `ClaimsPrincipal.IsInRole`, nên không thấy role dù claim tồn tại.

## Why the fix works
Tại ranh giới **trusted** identity, tạo `ClaimsIdentity` với `roleType: "roles"` (và `nameType: "sub"` nếu phù hợp). Xem mã đầy đủ tại [PartnerIdentityAdapter.cs](PartnerIdentityAdapter.cs). Policy giữ `RequireAuthenticatedUser()` và `RequireRole("Reviewer")`, không mở quyền rộng hơn.

Trong ASP.NET Core JWT Bearer thực tế, cấu hình tương đương ở `TokenValidationParameters.RoleClaimType` của scheme đang sử dụng, sau khi xác minh issuer, audience, signature, lifetime và claim contract. Không chuyển đổi role từ header/body do client tự gửi.

## How to verify
Thay nội dung `starter/PartnerIdentityAdapter.cs` bằng cách triển khai tương đương rồi chạy `./scripts/verify.ps1`. Positive cases phải pass; negative guardrails phải tiếp tục deny. `reproduce.ps1` sau fix trả khác 0 vì lỗi cũ không còn.

## Alternative fixes
- Chuẩn hóa role claim thành `ClaimTypes.Role` ở trusted claims transformation (có ownership rõ ràng, tránh nhân đôi hoặc nhầm scheme).
- Thiết kế policy dựa trên claim type cụ thể `RequireClaim("roles", "Reviewer")` khi contract provider ổn định; cần đánh giá ảnh hưởng đến mọi endpoint và scheme.
- Cấu hình `TokenValidationParameters.RoleClaimType` tại JWT authentication scheme là lựa chọn thực tế nếu không có adapter riêng.

## Wrong or misleading fixes
- Bỏ `RequireRole` hoặc chỉ kiểm tra `IsAuthenticated`: tạo privilege escalation.
- Chuyển sang `StringComparison.OrdinalIgnoreCase` cho role: không giải quyết vấn đề claim type và có thể làm sai contract.
- Chấp nhận `X-Role` do client gửi: vượt qua trust boundary.
- Hard-code `Reviewer` cho mọi principal: che lỗi và làm hỏng negative guardrails.

## Production implications
Lập contract test cho mỗi identity provider và scheme, kiểm tra `RoleClaimType`, issuer/audience, phân biệt app role với group, và không log token nhạy cảm. Theo dõi 403 sau thay đổi mapping nhưng không coi mọi 403 là lỗi.

## Trade-offs
Mapping tại JWT validation tập trung, nhất quán nhưng cần phân biệt scheme; claims transformation linh hoạt cho nhiều provider nhưng dễ sinh duplicate claims; claim-specific policy rõ contract nhưng coupling vào provider.

## What a Senior engineer should notice
Authentication thành công không chứng minh authorization thành công. Phải kiểm tra **nguồn tin cậy của claim**, identity construction và semantics của policy, đồng thời dùng negative regression tests để ngăn bản sửa tạo lỗ hổng quyền truy cập.
