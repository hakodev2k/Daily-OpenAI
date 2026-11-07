# Before — starter

- `./run.ps1` hiển thị ít nhất một `REJECTED`, trong khi case thông thường là `ACCEPTED`.
- `./reproduce.ps1` kết thúc exit 0 ở cấp script sau khi phát hiện chương trình con báo `REPRO_FAIL` (exit 2).
- `./verify.ps1` thất bại vì không phải tất cả 6 token round-trip đúng.
- Mã phát hành và mã partner tiếp nhận khác nhau ở một số case; không có mạng hoặc partner thật.
