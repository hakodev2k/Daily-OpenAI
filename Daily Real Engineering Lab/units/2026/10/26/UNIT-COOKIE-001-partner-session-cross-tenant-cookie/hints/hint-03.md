# Hint 03 — Ranh giới session

Tra cứu `HttpClientHandler.UseCookies` và `CookieContainer`. Thiết kế vòng đời container theo identity/session, không theo toàn bộ process. Chứng minh cả hai tenant vẫn giữ được session sau khi lần lượt đăng nhập.
