# After — acceptance contract

`./verify.ps1` phải in `VERIFIED` và exit code 0 với chuỗi: A-101 Applied, B-201 Applied, B-202 Conflict, B-201 retry AlreadyApplied, A-102 Applied. Final Status=Completed; Note=Valve seal damaged; Revision=3. Field revisions: Status=2, TechnicianNote=1.

Các kiểm tra bổ sung: ChangeId được tái sử dụng với payload khác → Conflict; field lạ → Conflict; thay đổi cũ trên cùng field → Conflict; không có mutation trên rejected change.

Sau khi sửa, `./reproduce.ps1` có thể thất bại vì không còn tái hiện bug gốc. Điều này là bình thường; verification mới là tiêu chí hoàn tất.
