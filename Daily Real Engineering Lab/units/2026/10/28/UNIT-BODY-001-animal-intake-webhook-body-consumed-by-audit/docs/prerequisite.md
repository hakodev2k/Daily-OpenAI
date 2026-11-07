# Prerequisites
- `Stream` có trạng thái đọc hiện tại; `ReadToEndAsync` không tự khôi phục vị trí.
- ASP.NET Core `RequestDelegate` và middleware chạy theo thứ tự đăng ký.
- `DefaultHttpContext` hỗ trợ fixture cục bộ, không cần khởi động Kestrel.
- `FrameworkReference Microsoft.AspNetCore.App` cung cấp ASP.NET Core mà không cần NuGet bên thứ ba.
- Cần .NET SDK hỗ trợ target `net8.0`. Kiểm tra bằng `dotnet --info`.
