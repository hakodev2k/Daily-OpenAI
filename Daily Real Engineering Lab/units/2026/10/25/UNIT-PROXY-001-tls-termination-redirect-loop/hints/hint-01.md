# Hint 1 — Phân loại request
Vẽ hai đường đi: client → edge → Kestrel và client → Kestrel. Đối chiếu `Location` trả về ở từng đường, đừng suy luận từ trạng thái TLS bên ngoài mà không kiểm tra request bên trong.
