# Reference Solution

Sau bước tính SHA-256, stream vẫn giữ vị trí đọc hiện tại. Reference implementation đặt lại vị trí đọc về đầu trước khi copy sang storage.

Chạy `./verify.ps1` trên code bạn sửa trong `starter/`. Kết quả đúng khi số byte, nội dung và SHA-256 của dữ liệu lưu trữ đều khớp payload ban đầu.

Một hướng khác là buffer payload một lần nếu kích thước phù hợp. Với stream không hỗ trợ seek, cần thiết kế pipeline khác thay vì giả định có thể đặt lại vị trí.

Không nên bỏ integrity check, retry mù hoặc chỉ kiểm tra `Length`, vì các cách đó không giải quyết contract về trạng thái của stream.

Trong production, API nhận `Stream` nên làm rõ ownership, vị trí đọc mong đợi, seekability và trách nhiệm dispose. Senior engineer nên nhận ra đây là vấn đề hidden mutable state qua component boundary, không chỉ là một dòng code thiếu.
