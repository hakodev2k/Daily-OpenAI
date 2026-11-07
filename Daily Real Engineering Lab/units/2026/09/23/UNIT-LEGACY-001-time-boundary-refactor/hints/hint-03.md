# Hint 03

.NET 8 có abstraction `TimeProvider`. Xem cách `TimeProvider.System` và `GetUtcNow()` có thể giữ production behavior nhưng cho phép verifier cung cấp clock riêng.