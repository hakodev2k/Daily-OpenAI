# Reference Solution
Mở rộng work-item contract để mang operation ID immutable từ producer sang consumer. Khi worker xử lý item, tạo logging scope mới từ metadata đó. Không capture `HttpContext` hoặc request-scoped service.

Root cause là work item hiện chỉ mang business ID; correlation metadata không vượt qua asynchronous lifetime boundary. Global mutable correlation state và giữ nguyên request object đều là wrong fixes vì gây coupling/lẫn dữ liệu giữa concurrent operations.