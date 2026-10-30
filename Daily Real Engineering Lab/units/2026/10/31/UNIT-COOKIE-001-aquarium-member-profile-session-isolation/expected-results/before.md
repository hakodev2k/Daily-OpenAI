# Trước khi sửa

Với starter ban đầu, `./reproduce.ps1` trả exit code 0 vì tái hiện đúng lỗi.

| Bước | HTTP | Body |
|---|---:|---|
| Coral mở phiên | 200 | — |
| Coral đọc profile | 200 | coral |
| Kelp đọc trước khi mở phiên | 200 | coral |
| Kelp mở phiên | 200 | — |
| Kelp đọc profile | 200 | kelp |
| Coral đọc lại | 200 | kelp |

`./verify.ps1` phải trả exit code khác 0. Nếu kết quả khác, kiểm tra .NET 8, loopback, trạng thái code starter và output của `./run.ps1`.
