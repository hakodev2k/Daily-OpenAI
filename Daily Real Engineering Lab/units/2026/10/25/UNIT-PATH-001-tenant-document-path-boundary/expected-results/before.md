# Before — baseline chưa sửa

`./reproduce.ps1` phải exit 0 vì đã tái hiện **lỗi có chủ đích**. Dự kiến:
```text
CASE valid: ALLOWED; content=approved
CASE similar-sibling: ALLOWED; content=restricted
CASE unrelated-sibling: DENIED; content=<none>
MODE --reproduce: PASS
```
`./verify.ps1` trên starter ban đầu phải exit 1 và in `MODE --verify: FAIL`. Nếu không thấy các case trên, xác nhận bạn đang chạy đúng unit, đúng SDK và baseline chưa bị thay đổi.
