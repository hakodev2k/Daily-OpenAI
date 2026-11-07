# After — starter đã sửa
```text
VALID_RECEIPT=PASS
EXTERNAL_ENTITY=REJECTED
Canary source: generated local test fixture only
```
`verify.ps1` phải trả exit code 0 trên starter đã sửa. Parser phải giữ nguyên hợp đồng dữ liệu XML hợp lệ và không tải nội dung ngoài tài liệu. `reproduce.ps1` có thể trả nonzero sau khi sửa vì triệu chứng gốc đã biến mất.
