# Hint 03

Kiểm tra lifetime của từng `HttpResponseMessage` và response stream. Với streaming response, việc chỉ đọc headers chưa đồng nghĩa connection đã sẵn sàng cho request kế tiếp.
