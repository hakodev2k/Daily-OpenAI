# Before — Dữ liệu mô phỏng

- reproduce.ps1 trả exit code 0 khi phát hiện starter không đạt yêu cầu.
- Memory ước tính của buffered-proxy 1 instance: 128 + 16 × 2048 = **32896 MiB**, vượt ngân sách 1536 MiB.
- Thời gian ước tính: ceil(32768 / 80) = **410 giây**, vượt SLO 300 giây.
- Các control integrityAtCommit, resumable, abandonedCleanup, idempotentFinalize còn thiếu.
- Đây là mô hình, không phải kết quả benchmark runtime; không cấp phát 32 GiB thật.

Nếu không reproduce: chạy setup.ps1, kiểm tra JSON và PowerShell 5.1+.
