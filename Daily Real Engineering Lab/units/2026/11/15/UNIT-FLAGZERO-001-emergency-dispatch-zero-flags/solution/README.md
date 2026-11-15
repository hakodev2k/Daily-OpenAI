# Reference Solution

## Root cause
`Enum.HasFlag(0)` trả về true vì `(value & 0) == 0` luôn đúng. Nhưng `None` ở đây là sentinel chưa có quyền, không phải tài nguyên public. Phép toán tập bit không thay thế được business authorization policy.

## Fix
Từ chối khi `assigned == None` hoặc `required == None`, sau đó kiểm tra `(assigned & required) == required`. Đối chiếu `solution/Program.cs` rồi sửa learner-editable `starter/Program.cs`.

## Verification
Chạy `pwsh -File scripts/verify.ps1` trên starter đã sửa: `MISMATCHES=0`, exit 0. `reproduce.ps1` chỉ kiểm tra triệu chứng starter ban đầu.

## Wrong fixes
- Chỉ thay `HasFlag` bằng phép `&` không chặn zero: lỗi vẫn còn.
- Chỉ chặn `assigned == None`: người có Medical vẫn xem được resource yêu cầu None.
- Cấp quyền mặc định cho mọi người: vi phạm least privilege.

## Trade-offs
Nếu cần public resources, thêm policy `IsPublic` rõ ràng; không overload `None`. Phân biệt yêu cầu **all** permissions với **any** permissions.

## Validation
Static review, chưa chạy .NET trong môi trường xuất bản.
