# Before — baseline chưa sửa

- `./run.ps1` in `DISPATCH_CANDIDATES=["WT-041","WT-042","WT-044","WT-045","WT-046"]`.
- `./reproduce.ps1` in `REPRODUCTION_CONFIRMED` và exit code 0.
- `./verify.ps1` in `VERIFY_FAILED baseline` và exit code khác 0.
- Có 2 false positives: `WT-041` và `WT-046`. Không cần dựa vào timing.
