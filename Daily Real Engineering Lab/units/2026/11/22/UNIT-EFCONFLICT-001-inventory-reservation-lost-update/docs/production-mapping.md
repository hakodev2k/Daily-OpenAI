# Mapping sang EF Core

Trong hệ thống thật, đánh dấu concurrency token (SQL Server rowversion hoặc token do app quản lý), dùng SaveChanges và bắt DbUpdateConcurrencyException. Một UPDATE nguyên tử có điều kiện remaining > 0 cũng bảo vệ bất biến; kiểm tra affected rows và transaction boundary. Đừng dùng kết quả mô phỏng in-memory để suy ra isolation của DB. Xây integration test hai DbContext riêng với database thật và một barrier đồng bộ.
