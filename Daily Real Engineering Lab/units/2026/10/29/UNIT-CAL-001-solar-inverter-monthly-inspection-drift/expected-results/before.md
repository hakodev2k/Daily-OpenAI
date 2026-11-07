# Before
`./reproduce.ps1` exit 0 khi baseline còn lỗi; `./verify.ps1` phải thất bại. Với ngày gốc 31/01/2026, starter tạo 31/01, 28/02, 28/03, 28/04, 28/05. Control ngày 15 vẫn đúng. Nếu không tái hiện, kiểm tra đang build `starter/Program.cs`, không phải `solution/`.
