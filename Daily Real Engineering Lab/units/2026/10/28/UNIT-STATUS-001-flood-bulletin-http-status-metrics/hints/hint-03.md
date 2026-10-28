# Hint 3

Trong ASP.NET Core middleware, `await _next(context)` là ranh giới trước/sau downstream. Kiểm tra status khi downstream đã hoàn thành; đồng thời cân nhắc exception path.
