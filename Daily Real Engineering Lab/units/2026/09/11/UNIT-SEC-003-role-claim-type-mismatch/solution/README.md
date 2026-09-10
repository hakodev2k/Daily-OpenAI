# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Principal đã authenticated và chứa `roles=Admin`, nhưng `ClaimsPrincipal.IsInRole("Admin")` trả về `false`.

## 2. Evidence

Starter in ra ba dữ kiện độc lập: authentication thành công, role-like claim tồn tại, nhưng role check thất bại. Vì vậy lỗi nằm ở semantic mapping giữa claim data và role evaluation, không phải ở việc claim bị thiếu.

## 3. Root cause

`ClaimsIdentity` không mặc định coi claim type `roles` là role claim type. `IsInRole` chỉ tìm claim có type bằng `identity.RoleClaimType`. Starter giữ dữ liệu role dưới `roles` nhưng không cấu hình contract đó cho identity.

## 4. Why the fix works

Reference solution tạo `ClaimsIdentity` với `roleType: "roles"`. Nhờ đó role evaluation và contract của identity provider cùng dùng một claim type, nên `IsInRole("Admin")` nhận diện đúng claim hiện có.

## 5. How to verify

Sửa `starter/Program.cs`, sau đó chạy:

```powershell
./verify.ps1
```

Verification chỉ pass khi principal vẫn authenticated, `roles=Admin` vẫn tồn tại và `IsInRole("Admin")` trở thành `True`.

## 6. Alternative fixes

- Normalize provider claim `roles` thành `ClaimTypes.Role` tại authentication boundary.
- Trong ASP.NET Core JWT authentication, cấu hình `TokenValidationParameters.RoleClaimType` phù hợp với token contract.
- Nếu provider có transformation layer chính thức, map claims ở một nơi duy nhất thay vì rải conversion trong business code.

## 7. Wrong / tempting fixes

- Hard-code `principal.Claims.Any(c => c.Value == "Admin")` trong từng endpoint: bỏ qua centralized authorization semantics và dễ tạo policy divergence.
- Thêm một claim giả chỉ để test pass: che giấu contract mismatch thay vì sửa identity boundary.
- Đổi authorization rule từ role-based sang `AuthenticatedUser` để hết 403: giảm security requirement.

## 8. Production implications

Identity-provider migrations thường thay đổi claim names, namespaces hoặc array semantics. Authentication có thể vẫn thành công trong khi authorization âm thầm từ chối hoặc, nguy hiểm hơn, cấp quyền sai nếu mapping bị cấu hình không chính xác.

## 9. Trade-offs

Normalize claim ở boundary giúp application code nhất quán nhưng tạo coupling với mapping layer. Giữ native provider claim type giảm transformation nhưng mọi authorization infrastructure phải được cấu hình chính xác theo provider contract.

## 10. What a Senior engineer should notice

Senior engineer phải phân biệt rõ ba lớp: token/identity data, claim mapping, và authorization policy. Khi authentication pass nhưng authorization fail, cần kiểm tra contract giữa các lớp trước khi sửa business rule hoặc nới quyền.
