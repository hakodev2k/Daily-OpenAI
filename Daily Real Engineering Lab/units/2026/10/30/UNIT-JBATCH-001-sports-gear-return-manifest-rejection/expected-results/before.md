# Before — starter

- reproduce.ps1: exit 0, Rejected=True, CommittedCount=2, Reproduction=PASS.
- verify.ps1: exit khác 0 và CONTRACT_FAIL.
- Kết quả không phụ thuộc thời gian, thread scheduling hay network.
- Nếu restore/build lỗi, kiểm tra .NET SDK 8 và quyền chạy PowerShell scripts.
