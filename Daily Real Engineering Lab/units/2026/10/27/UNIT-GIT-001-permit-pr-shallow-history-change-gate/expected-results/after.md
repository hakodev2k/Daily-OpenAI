# After

- verify.ps1 với starter đã sửa trả PASS.
- Output chính xác hai dòng (thứ tự có thể khác):
  - docs/permit-rules.md
  - src/PermitEngine/Rules.cs
- Base SHA không tồn tại trả exit code khác 0.
- Không sửa baseline, không đổi fixture để che lỗi.
- Trong CI thật, khi PR có nhiều commit hoặc target branch đã di chuyển, phải xác định comparison base theo contract của provider, không giả định HEAD^ là base.
