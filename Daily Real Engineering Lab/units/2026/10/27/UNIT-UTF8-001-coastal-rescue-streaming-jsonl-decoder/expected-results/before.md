# Before
- `./reproduce.ps1` exit 0 khi baseline tái hiện sai khác.
- Hai JSONL record có thể vẫn parse được, nhưng text không khớp wire; output có U+FFFD.
- `./verify.ps1` trên starter ban đầu exit khác 0.
- Nếu không tái hiện: xác nhận đã chạy `baseline`, .NET 10 và fixture nguyên bản.
