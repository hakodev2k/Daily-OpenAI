# Reference Solution — UNIT-AUTHZ-001

**Spoiler: chỉ xem sau khi đã reproduce, lập hypothesis và thử fix.**

## 1. Symptoms
Hai API trả 200 cho anonymous request, trong khi frontend chỉ hiển thị sau login; endpoint `/health` công khai theo thiết kế.

## 2. Evidence
`--reproduce` in `anonymous-audit=200`, `anonymous-export=200`, `authenticated-audit=200`, `anonymous-health=200`. Không có downstream database hay proxy trong fixture, nên giả thuyết do database/cache không giải thích được 200 tại HTTP boundary.

## 3. Root cause
`AuthorizationOptions.DefaultPolicy` chỉ được áp dụng khi endpoint có metadata yêu cầu authorization (ví dụ `[Authorize]` hoặc `RequireAuthorization()`). Các route chưa gắn metadata đó không tự động được bảo vệ. `FallbackPolicy` mặc định là null; chỉ có `UseAuthentication()` và `UseAuthorization()` không đủ để khóa toàn bộ route.

## 4. Why the fix works
Trong [Program.cs](Program.cs), cấu hình `FallbackPolicy` yêu cầu authenticated user cho các endpoint không có authorization metadata. Đánh dấu `/health` bằng `AllowAnonymous()` vì đây là public endpoint có chủ đích. Authentication handler chỉ là simulator; production phải dùng cơ chế xác thực thực (OIDC/JWT/cookie theo threat model).

## 5. How to verify
Sửa trực tiếp `starter/Program.cs`, chạy `./verify.ps1`. Expected: `anonymous-audit=401`, `anonymous-export=401`, `authenticated-audit=200`, `anonymous-health=200`, exit 0. `verify.ps1` **không chạy code trong solution/**. Có thể kiểm tra reference riêng bằng:
```powershell
dotnet run --project solution/AuthBoundary.csproj -- --verify
```

## 6. Alternative fixes
- Gắn `RequireAuthorization()` cho từng route hoặc cho một `RouteGroupBuilder` chứa các route riêng tư. Hợp lý nếu API có ranh giới public/private rõ và policy review chặt, nhưng endpoint mới dễ bị quên.
- Dùng `FallbackPolicy` làm default-deny và `AllowAnonymous()` cho public endpoints: thuận lợi khi mở rộng API, nhưng phải inventory health/docs/webhooks để không làm gián đoạn truy cập hợp lệ.
- Với nhiều vai trò, thêm named policies theo quyền cụ thể, không nhầm authentication với authorization theo resource/tenant.

## 7. Wrong or misleading fixes
- **Ẩn link trong frontend:** direct HTTP call vẫn tồn tại.
- **Chỉ thêm `UseAuthentication()`:** xác thực danh tính không đồng nghĩa bắt buộc mọi endpoint phải có danh tính.
- **Chỉ chỉnh `DefaultPolicy`:** route không có authorization metadata không nhận policy này.
- **Bật CORS nghiêm ngặt:** CORS không phải cơ chế chặn API clients ngoài browser.
- **Chỉ test HTTP 200 khi có login:** bỏ sót negative security assertions.

## 8. Production implications
Inventory toàn bộ routes và public exceptions, log anonymous access ở mức phù hợp không rò dữ liệu, kiểm tra health probes sau khi thay đổi, test anonymous GET/POST và cross-tenant authorization. Thay fake header handler bằng provider được cấu hình đúng, kiểm soát proxy và token validation. Không dùng fixture này làm production template.

## 9. Trade-offs
Fail-closed fallback giảm lỗi quên gắn attribute nhưng có thể ảnh hưởng health/docs/integration callbacks. Route-group explicit auth dễ đọc theo module nhưng phụ thuộc discipline của reviewer. Security regression tests nên chạy trong CI với cả endpoint mới.

## 10. Senior engineer should notice
Không suy ra chính sách API từ trạng thái UI; phải đánh giá HTTP boundary độc lập. Chính sách default-deny phải được chứng minh bằng **negative tests** và kiểm tra cả anonymous, authenticated, public endpoints, chứ không chỉ đường đi thành công.

## Tham khảo
- [Microsoft: AuthorizationOptions.DefaultPolicy](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationoptions.defaultpolicy)
- [Microsoft: AuthorizationOptions.FallbackPolicy](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationoptions.fallbackpolicy)
