# Investigation guide
1. Chạy demo và reproduce; ghi loại input, độ dài, outcome (true/false/exception).
2. Giữ nguyên chính sách chấp nhận SKU; chỉ sửa cách đánh giá.
3. Thử giả thuyết về chi phí CPU theo chiều dài chuỗi và cấu trúc pattern, không suy từ một latency đơn lẻ.
4. Sau khi sửa starter, chạy verify; nếu có failure, lưu test name và exception.
5. Tự đánh giá: thay đổi có loại bỏ nguyên nhân hay chỉ che triệu chứng? Có giới hạn input? Có thể dùng parser ký tự đơn giản thay regex không?
