# Investigation guide
1. Chạy reproduce và ghi lại từng hàng FAIL/PASS.
2. Kiểm tra thời điểm mở bằng `TimeZoneInfo.ConvertTime`.
3. Phân loại ba giả thuyết: sai ngày trừ 7, sai timezone, sai parse/format.
4. Chỉ thay một thành phần tại một thời điểm; chạy verify sau mỗi thay đổi.
5. Thảo luận khi nào local wall-clock time không tồn tại hoặc không duy nhất.
