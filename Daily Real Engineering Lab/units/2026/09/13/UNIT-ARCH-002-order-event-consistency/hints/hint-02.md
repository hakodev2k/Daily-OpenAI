# Hint 2

Nếu không thể commit SQL và broker atomically, hãy tìm cách làm cho **ý định publish** trở thành một phần của cùng durable transaction với business state. Sau đó một process khác có thể tiếp tục công việc còn lại.
