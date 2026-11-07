# After (starter đã được sửa)
- `./verify.ps1` exit 0.
- `EVENT host-startup-before-release=True` trong lúc gate vẫn đóng.
- `EVENT warmup-completed` sau `gate-released`.
- `EVENT host-stopped` chứng minh shutdown có kiểm soát.
- `./reproduce.ps1` không còn exit 0 sau fix: đây là expected vì triệu chứng cũ đã biến mất.

Kết quả chứng minh startup contract, không chứng minh worker đã sẵn sàng phục vụ dữ liệu trước warmup.
