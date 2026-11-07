# Hint 3
AsyncLocal phân tách việc gán giá trị theo execution context, nhưng không tự sao chép object mutable được lưu bên trong. Xem xét ranh giới tạo context của mỗi nhánh, thay vì thay đổi thuộc tính trên một instance chung.
