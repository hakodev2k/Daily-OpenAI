# After — learner-modified starter

- `verify.ps1` in `PASS` và exit 0.
- `200`: body `ABCDEFGHIJ`, không có Content-Range.
- `206 bytes=3-6`: body `DEFG`, Content-Range `bytes 3-6/10`.
- `206 bytes=0-0`: body `A`, Content-Range `bytes 0-0/10`.
- `206 bytes=7-`: body `HIJ`, Content-Range `bytes 7-9/10`.
- `206 bytes=-4`: body `GHIJ`, Content-Range `bytes 6-9/10`.
- `416 bytes=50-60`: body rỗng, Content-Range `bytes */10`.
- Sau sửa, `reproduce.ps1` có thể báo `NOT REPRODUCED` và exit 2: đây là hành vi đúng của phase reproduce khi lỗi cũ đã biến mất.
