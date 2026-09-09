# Before

- `invoice.txt` trong allowed directory được preview thành công.
- file ở sibling directory vẫn có thể được trả về dù không thuộc intended boundary.
- starter in `SECURITY_CHECK=FAILED` và kết thúc với exit code `2`.
- `reproduce.ps1` coi exit code này là bằng chứng intended failure đã được tái hiện.
