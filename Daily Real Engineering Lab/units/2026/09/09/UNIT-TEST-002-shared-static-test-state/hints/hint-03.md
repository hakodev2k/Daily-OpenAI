# Hint 03

Technical configuration mà mỗi test cần kiểm soát nên được truyền qua một explicit dependency có ownership rõ ràng thay vì mutable process-wide state.

Mục tiêu không phải là tắt parallelism; mục tiêu là làm hai test thực sự độc lập.
