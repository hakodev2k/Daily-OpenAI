# Before — expected symptom

- ./reproduce.ps1 trên starter nguyên bản: exit code 0, in ra SYMPTOM REPRODUCED.
- Một action đã cấu hình vẫn DENY cho user thiếu quyền.
- Action xuất biên bản không có trong fixture cấu hình lại ALLOW cho user chỉ có quyền xem.
- ./verify.ps1 trên starter nguyên bản: exit code khác 0, một số kiểm tra về deny-by-default báo FAIL.

Nếu kết quả khác, kiểm tra SDK, thư mục chạy và bảo đảm starter chưa bị sửa. Không sửa checks/Program.cs để ép PASS.