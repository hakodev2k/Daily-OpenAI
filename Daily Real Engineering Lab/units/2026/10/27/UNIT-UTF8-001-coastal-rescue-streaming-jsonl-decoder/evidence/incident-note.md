# Simulated incident note
- 09:12:14: gateway xác nhận hai JSONL record có newline và UTF-8 payload nguyên vẹn.
- 09:12:15: receiver log đủ hai record nhưng mô tả tiếng Việt ở record đầu không chính xác.
- 09:12:16: retry có lúc hiển thị đúng.
- Fixture chỉ thay đổi ranh giới đọc, không đổi byte payload. CPU và database chưa có bằng chứng liên quan.
