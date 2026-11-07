# Before — starter

- `reproduce.ps1` trả exit code 0 khi triệu chứng được tái hiện đúng.
- `EDGE_SIM` là HTTP 307 và `Location` có scheme `https`, host `partner.example.test`.
- `DIRECT_HTTP` là HTTP 307 chuyển sang HTTPS.
- Không có backend 500; request bị xử lý trước khi endpoint trả JSON.
- `verify.ps1` phải thất bại trên starter nguyên bản; đây là phép kiểm độc lập.
