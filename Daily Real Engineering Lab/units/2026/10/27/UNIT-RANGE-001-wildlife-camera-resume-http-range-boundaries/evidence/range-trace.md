# Evidence — local fixture trước khi sửa

Fixture 10 byte, chunk size 4. Server mô phỏng trả 206 cho mỗi request hợp lệ; không có network fault.

```text
original hex: 30313233343536373839
request 1: Range: bytes=0-4  -> Content-Range: bytes 0-4/10 -> 5 bytes
request 2: Range: bytes=4-8  -> Content-Range: bytes 4-8/10 -> 5 bytes
request 3: Range: bytes=8-9  -> Content-Range: bytes 8-9/10 -> 2 bytes
reassembled: 303132333434353637383839
```

Ghi lại request/response boundaries của fixture 12 byte, sau đó kiểm tra liệu mỗi byte gốc có xuất hiện đúng một lần không. Không cần tin log được viết sẵn: chạy `run.ps1` để tạo evidence của chính bạn.
