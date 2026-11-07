# After — Hợp đồng mong muốn

```text
en-US: asciiLookup=True lookalikeLookup=False duplicateAccepted=False
tr-TR: asciiLookup=True lookalikeLookup=False duplicateAccepted=False
VERIFIED: ASCII pass-code identity contract holds across both locales.
```

- `./verify.ps1`: exit 0 trên code **starter đã sửa**.
- `./reproduce.ps1`: không còn tái hiện triệu chứng cũ nên trả exit khác 0.
- Không thay đổi `Program.cs` hoặc assertions để làm xanh bài kiểm tra.
- Ghi chú rằng mã bí mật trong production cần chính sách case sensitivity rõ ràng; bài này mô phỏng **mã định danh vé không bí mật**, được quy định ASCII case-insensitive.
