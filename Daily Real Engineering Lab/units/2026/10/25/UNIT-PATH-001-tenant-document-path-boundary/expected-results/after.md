# After — starter đã được sửa

`./verify.ps1` phải exit 0; behavior mong đợi:
```text
CASE valid: ALLOWED; content=approved
CASE similar-sibling: DENIED; content=<none>
CASE unrelated-sibling: DENIED; content=<none>
MODE --verify: PASS
```
Kiểm tra đúng cả hai thuộc tính: tài liệu hợp lệ vẫn đọc được, tài liệu ngoài phạm vi không được đọc. `./reproduce.ps1` tiếp tục chứng minh baseline cũ bị lỗi, bất kể bạn đã sửa starter hay chưa.
