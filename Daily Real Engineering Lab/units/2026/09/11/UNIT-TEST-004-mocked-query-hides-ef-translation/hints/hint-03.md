# Hint 3

Kiểm tra custom .NET method nằm **bên trong** `Where`. Một hướng sửa là đưa phần tính toán độc lập với row ra ngoài expression và giữ phần chạy trên entity ở dạng mà relational provider dịch được. Sau đó bổ sung relational-provider coverage thay vì chỉ mock queryable.