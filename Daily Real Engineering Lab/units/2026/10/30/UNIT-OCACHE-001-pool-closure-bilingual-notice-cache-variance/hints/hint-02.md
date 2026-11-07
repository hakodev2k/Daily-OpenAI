# Hint 2 — Xác định lớp sở hữu response
Quan sát đường đi của request qua middleware và endpoint. Nếu cùng một URL được phục vụ nhiều lần mà endpoint không tăng `renderCount`, thành phần nào đang quyết định tái sử dụng response? Những input nào tạo ra một biến thể nghiệp vụ khác nhau?
