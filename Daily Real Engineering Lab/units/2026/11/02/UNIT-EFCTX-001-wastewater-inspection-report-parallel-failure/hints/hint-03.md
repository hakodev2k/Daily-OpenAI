# Hint 03 — concept

Một `DbContext` của EF Core không hỗ trợ nhiều operation đang chồng lấn. Nếu muốn giữ song song, mỗi operation cần một `DbContext` độc lập với vòng đời được quản lý; nếu không cần song song, await tuần tự cũng là một lựa chọn có trade-off.
