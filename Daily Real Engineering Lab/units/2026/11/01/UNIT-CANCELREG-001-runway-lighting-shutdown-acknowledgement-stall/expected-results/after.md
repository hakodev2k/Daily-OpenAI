# After
- `./verify.ps1` exit 0 và in `PASS`.
- `Cancel()` trả về khi gate vẫn đóng.
- Cleanup chưa hoàn tất trước khi mở gate; sau gate, Task cleanup hoàn tất.
- Flush count đúng một lần, cả khi gọi `Cancel()` lặp.
- Token đã cancelled trước lúc đăng ký vẫn hoàn thành cleanup.
- `./reproduce.ps1` có thể exit 2 sau fix vì không còn triệu chứng cũ.
