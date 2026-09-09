# Hint 3

Tránh gọi `BuildServiceProvider()` để lấy dependency trong lúc đăng ký services. Hãy để container cuối cùng resolve dependency qua registration factory hoặc constructor injection.
