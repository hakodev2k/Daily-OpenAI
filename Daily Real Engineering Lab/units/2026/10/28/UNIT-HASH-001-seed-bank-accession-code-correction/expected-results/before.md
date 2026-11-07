# Before — baseline
`./reproduce.ps1` kết thúc thành công khi **phát hiện** đúng lỗi gốc, in:
```text
FOUND_CORRECTED=False
DUPLICATE_ACCEPTED=True
REMOVED_NEW_CODE=True
REMAINING_COUNT=1
REPRODUCED: corrected record remains unreachable
REPRODUCTION_CONFIRMED
```
Đây là chứng cứ tái hiện, không phải kết quả đã fix. `./verify.ps1` trên starter chưa sửa phải thất bại.
