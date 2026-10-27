# Sau khi sửa
- First: EAST; after cutover: WEST; next: WEST.
- ConnectionAttempts >= 2, chứng minh route lookup xảy ra lại sau khi connection cũ không còn được reuse.
- `./verify.ps1` exit code 0. Kiểm tra thêm repeated requests trước và sau cutover, failure khi WEST unreachable, latency do connection churn.
- Giá trị lifetime trong fixture chỉ để deterministic reproduction, không phải giá trị mặc định cho production.
