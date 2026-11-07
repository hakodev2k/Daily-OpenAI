# Hint 3

Trong MongoDB, filter của update có thể chứa cả document identity và version đã đọc. Nếu matched count bằng 0, request biết snapshot của nó đã cũ và có thể retry theo policy thay vì silently overwrite.