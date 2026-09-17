# Hint 3
Một Promise chain chỉ chờ asynchronous work khi callback trả về Promise đại diện cho work đó. Kiểm tra boundary giữa `save()` và `followUp()`.