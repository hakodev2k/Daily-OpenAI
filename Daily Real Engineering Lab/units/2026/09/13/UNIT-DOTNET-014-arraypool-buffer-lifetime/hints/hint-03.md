# Hint 3

Một buffer được mượn chỉ nên được đưa trở lại pool khi không còn consumer nào cần dữ liệu trong buffer đó. Nếu API không thể chuyển ownership của lease cho caller, hãy cân nhắc tạo dữ liệu owned ổn định tại boundary.