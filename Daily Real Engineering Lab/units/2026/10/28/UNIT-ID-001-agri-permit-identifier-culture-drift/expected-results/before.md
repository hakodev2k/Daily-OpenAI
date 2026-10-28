# Before — starter

- `./reproduce.ps1` trả exit 0 vì **symptom** được tái hiện, không phải vì bug đã sửa.
- `en-US`: count 2, `INSP-204=CLEARED`, `INSP-205=PENDING`.
- `tr-TR`: count 3, `INSP-204=RECEIVED`, `INSP-205=PENDING`.
- `./verify.ps1` trả exit khác 0.
- Nếu `tr-TR` không khởi tạo được, kiểm tra globalization-invariant mode; không bỏ test đó.
