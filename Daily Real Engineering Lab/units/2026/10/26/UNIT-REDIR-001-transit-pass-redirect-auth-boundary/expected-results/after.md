# After
- `verify.ps1` trả exit 0; direct và redirected path đều 200, service ghi nhận 2 request có credential hợp lệ.
- Redirect sang origin không tin cậy bị chặn với exception chính sách, trước khi gửi request.
- `reproduce.ps1` trả exit 1 vì symptom gốc đã được loại bỏ.
- Credential không xuất hiện trong URL, query string hoặc logs; giới hạn redirect hop rõ ràng.
