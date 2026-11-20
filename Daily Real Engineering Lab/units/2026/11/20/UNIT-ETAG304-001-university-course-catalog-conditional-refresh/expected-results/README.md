# Expected results
**Starter:** refresh đầu 200 và 2 courses; refresh thứ hai 304 nhưng job kết thúc lỗi. `reproduce.ps1` phải xác nhận triệu chứng (exit 0 của script sau khi quan sát lỗi có chủ đích).

**Fixed learner code:** refresh đầu 200, refresh thứ hai 304 không làm mất snapshot, khi ETag đổi sẽ lấy bản 200 mới có 3 courses; 503 phải báo lỗi và giữ snapshot cuối cùng. `verify.ps1` kết thúc `VERIFY PASS`.

Nếu không reproduce: xác nhận SDK `dotnet --version` là 8.x, chạy script từ PowerShell và kiểm tra không sửa fixture handler.
