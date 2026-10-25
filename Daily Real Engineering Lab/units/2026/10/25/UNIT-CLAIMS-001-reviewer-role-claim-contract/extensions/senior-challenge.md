# Senior challenge — Nhiều identity providers

Portal thêm một provider phát hành role dưới `http://schemas.microsoft.com/ws/2008/06/identity/claims/role`, trong khi provider hiện tại dùng `roles`. Đề xuất cách chuẩn hóa claims sau khi token validation cho **từng authentication scheme**, không chấp nhận claim từ nguồn không tin cậy. Viết contract tests chứng minh:
1. `Reviewer` từ mỗi provider tin cậy được phép;
2. `Reader` và anonymous vẫn bị chặn;
3. role không bị cấp nhầm giữa các scheme;
4. mapping thay đổi không làm tăng privilege ở endpoint khác.

So sánh JWT `RoleClaimType`, `IClaimsTransformation` và claim-specific policy theo maintainability, trust boundary, observability và chi phí regression.
