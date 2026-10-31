# Concepts — đọc sau khi tự thử sửa

HTTP `Range: bytes=a-b` mô tả hai byte offset **bao gồm cả a và b**. `Content-Range` cho response 206 phải khớp với bytes thực truyền; một 206 không tự bảo đảm integrity. Với đoạn từ a đến b, số byte là `b - a + 1`.

Các trường hợp biên: `0-0` là một byte; `7-` đến cuối representation; `-4` lấy bốn byte cuối; range ngoài representation nhận 416 với `Content-Range: bytes */length`. Endpoint demo chỉ xử lý một range; multipart và `If-Range` nằm ngoài scope, nên không nên dùng implementation này như một file server production đầy đủ.

Nguồn tham khảo:
- [RFC 9110 — Range Requests](https://www.rfc-editor.org/rfc/rfc9110.html#section-14)
- [RFC 9110 — Content-Range](https://www.rfc-editor.org/rfc/rfc9110.html#section-14.4)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
