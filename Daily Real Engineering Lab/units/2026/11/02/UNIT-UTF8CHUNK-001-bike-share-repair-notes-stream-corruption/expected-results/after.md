# After — Hợp đồng mong muốn

```text
one-byte: correct=True rejected=False replacementChars=0
mixed: correct=True rejected=False replacementChars=0
ascii: correct=True rejected=False replacementChars=0
invalid-sequence: correct=True rejected=True replacementChars=0
truncated-sequence: correct=True rejected=True replacementChars=0
VERIFIED: valid text is chunk-independent and malformed input is rejected.
```

`./verify.ps1` phải trả exit 0 trên `starter/ChunkedNoteDecoder.cs` đã sửa. `./reproduce.ps1` không còn tìm thấy triệu chứng cũ và có thể trả lỗi sau fix. Không sửa harness hoặc chỉ loại bỏ dấu tiếng Việt để vượt qua test.
