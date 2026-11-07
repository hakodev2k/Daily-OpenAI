# Hint 03

Một hướng sửa hợp lệ là đảm bảo query được materialize trong cùng lifetime nơi `DbContext` còn sống, rồi chỉ trả dữ liệu đã materialize qua boundary.
