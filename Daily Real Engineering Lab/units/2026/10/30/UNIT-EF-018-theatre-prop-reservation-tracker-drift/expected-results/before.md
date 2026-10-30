# Before — Starter nguyên gốc
- `./reproduce.ps1` trả exit code 0 và chỉ ra một giá trị persisted không bằng 7.
- `./verify.ps1` trả exit code khác 0 vì một hoặc nhiều invariant bị vi phạm.
- Trường hợp opening=3, reserved=2, sold=2 không được chấp nhận, nhưng starter xử lý sai.
- Các kết quả là deterministic trên SQLite in-memory, không cần đo timing.
