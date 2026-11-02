# Expected Results — Sau khi sửa

`./verify.ps1` phải chạy code từ `starter/` đã được người học chỉnh sửa và in:

```text
hostStoppedEarly=False
attempts=AQ-101,AQ-102,AQ-103
published=AQ-101,AQ-103
failed=AQ-102
LAB_RESULT=PASS
```

Chương trình trả exit code 0. Lỗi của `AQ-102` được ghi nhận, không khiến `AQ-103` biến mất. Nếu muốn kiểm tra lại lỗi gốc sau khi sửa, dùng bản Git sạch hoặc checkout phiên bản starter ban đầu; không dùng `reproduce.ps1` như một bài kiểm tra sau fix.
