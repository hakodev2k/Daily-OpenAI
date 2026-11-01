# Before — Starter nguyên bản

```text
one-byte: correct=False rejected=False replacementChars>0
mixed: correct=False rejected=False replacementChars>0
ascii: correct=True rejected=False replacementChars=0
invalid-sequence: correct=False rejected=False replacementChars>0
truncated-sequence: correct=False rejected=False replacementChars>0
REPRODUCED: segmentation changes valid text and malformed bytes are accepted.
```

Các trường `chunks` phụ thuộc số fragment mà harness tạo ra. `./reproduce.ps1` thành công (exit 0) nghĩa là lỗi ban đầu đã được chứng minh, **không phải đã fix**. `./verify.ps1` phải thất bại trước khi sửa.
