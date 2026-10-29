# After — starter đã sửa
- `./verify.ps1` exit 0 và in `PASS: 3 valid frames and 3 invalid frames`.
- Header `00 04` đọc thành 4, `00 03` thành 3, `00 00` thành 0.
- Payload thiếu byte, header đảo, và header thiếu vẫn bị từ chối.
- `./reproduce.ps1` không còn tái hiện lỗi cũ (có thể exit 1); không dùng script này để chứng minh fix.
