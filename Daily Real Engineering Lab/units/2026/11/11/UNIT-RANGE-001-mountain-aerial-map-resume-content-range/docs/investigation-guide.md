# Investigation
1. So sánh độ dài file cũ, payload mới, và tổng mong đợi.
2. Đọc `Content-Range` và thử dựng bảng các chỉ số byte 0-based.
3. Giải thích tại sao HTTP 206 không đủ để đảm bảo integrity.
4. Thử case response hợp lệ, response lệch start, response thiếu range.
5. Chỉ sửa `ResumeAssembler.Append`; giữ fixture nguyên để đảm bảo reproduce vẫn phát hiện lỗi.
