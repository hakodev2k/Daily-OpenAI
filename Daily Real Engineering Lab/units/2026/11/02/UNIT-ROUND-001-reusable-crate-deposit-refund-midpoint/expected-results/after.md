# After — Learner starter sau khi sửa

`./verify.ps1` phải chạy toàn bộ 12 contract và kết thúc exit code 0:

```text
PASS: single midpoint
PASS: batch rounding once
PASS: midpoint with odd prior cent
PASS: another midpoint
PASS: negative reversal
PASS: negative batch reversal
PASS: non-midpoint below
PASS: non-midpoint above
PASS: zero quantity
PASS: exact cent
PASS: zero unit deposit
PASS: negative unitDeposit rejected.
VERIFIED: 12 learner-starter behavior contracts passed.
```

`./reproduce.ps1` không còn được kỳ vọng thành công sau khi fix: nó được thiết kế để xác nhận lỗi gốc, không phải kiểm tra fix. Số tiền cuối cùng giữ 2 chữ số thập phân; đầu vào không hợp lệ vẫn bị từ chối.
