# After — learner fix
- `./verify.ps1` PASS: cả hai worker chạy revision B sau release mới.
- Rollback chạy revision A trên cả hai worker ngay cả khi registry tag hiện trỏ về B.
- Release B và rollback A có spec khác nhau; digest không hợp lệ bị từ chối.
- `./reproduce.ps1` không còn xác nhận triệu chứng sau khi sửa (kỳ vọng).
