# Sau khi sửa

`./verify.ps1` phải trả exit code 0 và in `PASS: tenant session isolation`.

| Bước | HTTP | Body |
|---|---:|---|
| Coral mở phiên | 200 | — |
| Coral đọc profile | 200 | coral |
| Kelp đọc trước khi mở phiên | 401 | — |
| Kelp mở phiên | 200 | — |
| Kelp đọc profile | 200 | kelp |
| Coral đọc lại | 200 | coral |

Đây là contract chức năng và cô lập dữ liệu; không dùng thời gian chạy làm tiêu chí. Sau khi sửa, `./reproduce.ps1` có thể trả non-zero vì triệu chứng cũ không còn, đó là điều mong đợi.
