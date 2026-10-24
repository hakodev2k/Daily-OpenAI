# After

- `./verify.ps1` chạy learner-editable starter, kỳ vọng exit code 0 và `VERIFIED`.
- `FirstKeyCompileCount=1`, `AllWorkersShareCachedInstance=True`, `AllResultsMatchKey=True`.
- `TotalCompileCountAfterSecondKey=2`, `SecondKeyContractValid=True`.
- Đây là single-process verification, không phải distributed coordination.
