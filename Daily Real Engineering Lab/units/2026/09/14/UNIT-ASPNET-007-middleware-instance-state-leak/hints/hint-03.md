# Hint 3

Request-specific data không nên nằm trong mutable instance field của conventional middleware. Thử giữ tenant trong state chỉ thuộc invocation hiện tại.
