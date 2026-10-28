# After — yêu cầu sau khi learner sửa

- `./verify.ps1` trả exit code 0 và có `VERIFY_PASS`.
- `ACCEPTED` chứa đúng `CAL-101`, `CAL-102`, `CAL-103`, `CAL-104`.
- Không có bản ghi trùng sau callback lặp hoặc đối soát lặp.
- Không cần giả định callback phải đến cho mọi tệp đã hoàn chỉnh.
- `reproduce.ps1` chỉ dành cho baseline ban đầu; sau khi sửa nó có thể báo mismatch và đó là bình thường.
