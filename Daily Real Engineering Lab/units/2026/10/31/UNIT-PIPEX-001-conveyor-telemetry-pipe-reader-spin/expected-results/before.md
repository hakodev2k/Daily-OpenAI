# Before

- `./reproduce.ps1`: exit 0 và in `REPRODUCED: READ_SPIN_GUARD...`.
- `./verify.ps1`: exit 1, báo reader kết thúc trước khi producer gửi thêm byte.
- Không cần timing tuyệt đối giống nhau giữa các máy.
