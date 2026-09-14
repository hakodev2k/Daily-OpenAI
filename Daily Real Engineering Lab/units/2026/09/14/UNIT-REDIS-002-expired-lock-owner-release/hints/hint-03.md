# Hint 3

Mỗi acquire cần một ownership token duy nhất. Release phải là một compare-and-delete atomic operation: chỉ xóa khi value hiện tại vẫn khớp token của worker đang release.
