# Investigation
1. Lưu nguyên văn các eventId gốc.
2. Chạy starter và so sánh khóa được log.
3. Xác định điểm biến đổi dữ liệu trước dedup.
4. Tạo giả thuyết: duplicate upstream, collision khi chuyển kiểu, hay bug của Dictionary?
5. Thử một cặp ID nhỏ hơn và cặp ID gần ngưỡng lớn; đo số accepted.
6. Sau khi sửa, chạy verify và thêm test cho ID biên.
