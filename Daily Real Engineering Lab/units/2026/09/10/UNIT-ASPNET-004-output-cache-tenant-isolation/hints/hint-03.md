# Hint 3

ASP.NET Core Output Caching hỗ trợ vary cached response theo request header.

Tìm API `SetVaryByHeader(...)` và quyết định liệu endpoint này nên vary theo tenant header hay nên tắt caching hoàn toàn nếu dữ liệu không phù hợp để cache.
