# Before — Starter nguyên bản

Sau `./run.ps1`, kết quả dự kiến (cách viết boolean theo .NET):

```text
en-US: asciiLookup=True lookalikeLookup=False duplicateAccepted=False
tr-TR: asciiLookup=False lookalikeLookup=True duplicateAccepted=True
```

Trong một số môi trường globalization, hành vi Unicode bổ sung ngoài những case này có thể khác; hợp đồng verify tập trung vào hai locale và ba case được mô hình hóa.

- `./reproduce.ps1`: exit 0, `REPRODUCED`.
- `./verify.ps1`: exit khác 0, `FAILED`.

Không được coi reproduce exit 0 là một fix thành công.
