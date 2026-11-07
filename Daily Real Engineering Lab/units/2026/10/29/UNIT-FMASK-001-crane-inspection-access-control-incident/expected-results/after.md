# After — verification contract

- ./verify.ps1: 9/9 PASS và exit code 0 trên learner-editable starter/PermissionGate.cs.
- ViewInspection và ApproveInspection có phân quyền đúng.
- Action không đăng ký, requirement rỗng và requirement chứa bit không định nghĩa đều bị DENY.
- Một user có đủ các quyền hợp lệ vẫn không thể gọi action không được đăng ký.
- ./reproduce.ps1 sau khi sửa sẽ không còn tìm thấy symptom và có thể trả exit code 1; đây là hành vi đúng cho phase reproduction.

Nếu verification thất bại, đọc tên assertion thất bại và so sánh với invariant thay vì sửa test.