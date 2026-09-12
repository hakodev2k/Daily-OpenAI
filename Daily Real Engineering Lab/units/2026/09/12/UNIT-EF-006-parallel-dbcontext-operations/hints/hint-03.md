# Hint 3

Nếu hai queries thật sự cần chạy song song, mỗi concurrent operation cần execution boundary độc lập. Nếu không cần song song, serialize operations trên cùng context.