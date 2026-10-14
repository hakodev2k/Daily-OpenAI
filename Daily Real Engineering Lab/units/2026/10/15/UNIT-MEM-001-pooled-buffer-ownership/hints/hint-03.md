# Hint 3 — Khái niệm
`ReadOnlyMemory<byte>` chỉ ngăn ghi thông qua view đó; nó không sở hữu hay đóng băng underlying array. Hãy xét contract ownership trước khi buffer được tái sử dụng.
