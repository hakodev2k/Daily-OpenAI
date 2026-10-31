# Hint 2 — Theo dấu dữ liệu

Quan sát quá trình tạo `MemoryStream`: kích thước được cấp phát ban đầu, số byte đã ghi và đối tượng `byte[]` được chuyển qua boundary. Đặt breakpoint ngay trước `return` và xem `Capacity`, `Length` và `Payload.Length`.
