# After
- A: 42/1000; B: 7/260; C: 1/65535.
- `verify.ps1` in `PASS` và exit 0, vẫn reject short frame, wrong signature, wrong version, nonzero reserved.
- `reproduce.ps1` exit 1 sau fix là đúng: lỗi gốc không còn.
