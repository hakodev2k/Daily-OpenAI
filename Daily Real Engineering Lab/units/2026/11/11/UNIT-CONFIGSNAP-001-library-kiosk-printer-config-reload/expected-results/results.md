# Expected results
| Check | Baseline | Sau sửa |
|---|---|---|
| First label | `KIOSK-A:OLD:BOOK-104` | Không đổi |
| Registry version | `2` | `2` |
| Second label | `KIOSK-A:OLD:BOOK-105` | `KIOSK-B:NEW:BOOK-105` |
| Refresh observed | `False` | `True` |

`reproduce.ps1` kiểm tra symptom của starter gốc; `verify.ps1` kiểm tra starter sau khi người học sửa. Không chạy verify trên solution thay cho starter.

