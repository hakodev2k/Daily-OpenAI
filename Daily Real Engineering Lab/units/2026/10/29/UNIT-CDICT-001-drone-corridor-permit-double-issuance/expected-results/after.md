# After — tiêu chí nghiệm thu

```powershell
./verify.ps1
# OBSERVED issued=1; sameCachedPermit=True; reusedPermit=True; issuedAfterOtherKey=2
# VERIFY_PASS: one side effect per key and stable cache behavior
# exit code: 0
```

Hai request đồng thời cho `CORRIDOR-42` chỉ tạo một external issuance; request lặp lại dùng value đã có; `CORRIDOR-99` tạo thêm đúng một permit. `reproduce.ps1` sau khi sửa phải báo không còn symptom (exit code 1), vì nó là kiểm tra baseline.
