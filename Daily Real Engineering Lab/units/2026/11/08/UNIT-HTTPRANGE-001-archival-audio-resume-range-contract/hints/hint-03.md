# Hint 3
Khi server trả 200 cho request Range, dữ liệu phản hồi có thể là toàn bộ tài nguyên. Chỉ append nếu Content-Range đúng offset và tổng chiều dài mong đợi; nếu không hãy xử lý như tải mới.
