# Expected Results — After

Một fix hợp lệ phải:

- giữ gzip support
- không tăng limit
- không giảm fixture payload
- không materialize toàn bộ oversized response rồi mới reject
- enforce 1,000,000-byte budget trên dữ liệu mà application thực sự consume
- fail rõ ràng khi budget bị vượt

`verify.ps1` build và chạy trực tiếp learner-editable project.
