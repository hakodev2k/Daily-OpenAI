# Hint 3

Trong simulator, chỉ nên dùng `ForwardedFor` khi `networkPeer` chính là trusted proxy. Với ASP.NET Core thực tế, đối chiếu cơ chế `ForwardedHeadersMiddleware` và cấu hình `KnownProxies` / `KnownNetworks`.
