# Hint 3

Mục tiêu không phải serialize toàn bộ cache. Hãy nghĩ về coordination scope theo cache key và việc nhiều caller có thể await cùng một in-flight load.