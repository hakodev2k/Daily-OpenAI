# Progressive hints

## Hint 1
Theo dõi riêng server version, client baseVersion và operationId; cùng một operation có thể được gửi lại với body khác nhau.

## Hint 2
Đừng giả định mọi field có cùng khả năng merge. Phân biệt dữ liệu quan sát, tập ảnh và quyết định an toàn.

## Hint 3
Hãy xác định quy tắc an toàn bất biến và cách từ chối một stale write trước khi tính đến UI xử lý conflict.
