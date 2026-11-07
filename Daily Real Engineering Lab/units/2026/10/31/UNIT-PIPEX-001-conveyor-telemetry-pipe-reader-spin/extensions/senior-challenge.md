# Senior Challenge

Mở rộng parser cho nhiều frames trong cùng buffer, giới hạn 4 KiB/frame, xử lý EOF và cancellation. Viết contract test cho ba frames đến trong một `ReadAsync` và một frame chia thành nhiều fragments. Chứng minh không mất/trùng byte và đề xuất metric cardinality thấp.
