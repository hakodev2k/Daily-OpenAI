# Before — starter
- `./reproduce.ps1` kết thúc thành công vì đã phát hiện mismatch (đây là kiểm thử triệu chứng, không phải kiểm thử fix).
- Cả hai worker Ready nhưng `node-east` chạy revision A, `node-west` chạy revision B.
- `./verify.ps1` thất bại ở release/rollback/provenance/input validation.
- Tình huống hoàn toàn deterministic; không dùng clock hoặc random.
