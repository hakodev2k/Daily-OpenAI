# Before — Starter chưa sửa

- reproduce.ps1 in REPRODUCE_PASS khi 128 ký tự trả exit 0 và 524288 ký tự trả exit 23 với DEADLOCK_REPRODUCED.
- verify.ps1 phải thất bại trên starter nguyên bản.
- CPU thấp và child bị chặn trước marker hoàn thành là triệu chứng, không phải bằng chứng duy nhất.
- Nếu case nhỏ không chạy, kiểm tra .NET 8 SDK/runtime và quyền tạo process. Nếu case lớn không timeout, kiểm tra starter đã được sửa hay chưa.
