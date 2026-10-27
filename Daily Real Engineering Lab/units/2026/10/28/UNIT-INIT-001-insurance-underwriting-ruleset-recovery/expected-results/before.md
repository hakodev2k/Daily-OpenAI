# Before — Starter

Chạy ./reproduce.ps1 phải in:
- FIRST_503=True
- SECOND_503=True
- PROVIDER_CALLS=1
- REPRODUCED

Exit code 0 nghĩa là đã xác nhận triệu chứng, không phải hệ thống đúng. ./verify.ps1 trên starter nguyên bản phải thất bại. Nếu không reproduce, kiểm tra SDK, project path và xem có sửa provider/harness hay không.
