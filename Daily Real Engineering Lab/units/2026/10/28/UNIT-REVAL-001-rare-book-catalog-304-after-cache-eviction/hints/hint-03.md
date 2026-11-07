# Hint 03 — Hợp đồng

HTTP 304 không mang representation body mới. Chỉ phát conditional request khi có một representation tương ứng còn sử dụng được; nếu upstream trả 304 bất thường khi không có body, đừng trả dữ liệu rỗng như một kết quả hợp lệ.
