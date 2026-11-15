# Hợp đồng tải video theo đoạn

API nội bộ nhận byte range `start-end`, hai đầu đều được tính vào payload. File có 101 byte; client tải ba request: `0-31`, `32-63`, `64-100`. Dữ liệu sau khi ghép phải trùng chính xác file gốc. Trường hợp `0-0` trả một byte, `100-100` trả byte cuối; range âm, đảo đầu/cuối hoặc vượt kích thước phải bị từ chối. Metadata `Content-Range` phải khớp byte thực trả.

Nhiệm vụ: thu thập bằng chứng; xác định boundary invariant; sửa starter mà không thay đổi bộ fixture và bộ verification; mô tả cách kiểm thử khi API thật hỗ trợ HTTP 206 và 416.
