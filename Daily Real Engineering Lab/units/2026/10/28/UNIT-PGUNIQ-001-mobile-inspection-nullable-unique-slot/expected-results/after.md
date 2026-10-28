# After — starter đã sửa
- verify.ps1 phải kết thúc thành công với hai thông báo PASS cho lịch xung đột và một PASS cho ba lịch hợp lệ.
- Test được chạy trong transaction và ROLLBACK nên không để lại dữ liệu test.
- Chạy reproduce.ps1 sau khi sửa không còn là test hợp lệ: script này cố tình mong lỗi gốc vẫn xảy ra.
- Nếu verification vẫn đỏ, xác nhận đã sửa starter/01-schema.sql chứ không chỉ solution/01-schema.sql.
