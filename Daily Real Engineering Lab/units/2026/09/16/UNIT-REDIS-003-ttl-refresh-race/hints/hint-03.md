# Hint 3

Trong Redis thật, nhiều bước liên quan cùng invariant thường cần một atomic boundary, ví dụ transaction có điều kiện hoặc Lua script. Simulator có thể biểu diễn cùng nguyên tắc bằng version-checked mutation.