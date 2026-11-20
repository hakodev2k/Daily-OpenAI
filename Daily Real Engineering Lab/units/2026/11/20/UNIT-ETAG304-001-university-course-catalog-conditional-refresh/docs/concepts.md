# Sau lab: kiến thức nền
- HTTP conditional requests dùng `If-None-Match` và `ETag`.
- `304 Not Modified` chỉ có nghĩa khi client có representation phù hợp để tái sử dụng; response không mang body mới.
- `HttpResponseMessage.EnsureSuccessStatusCode()` ném exception với status ngoài 2xx.
- Tài liệu tham khảo: https://www.rfc-editor.org/rfc/rfc9110.html#section-15.4.5 ; https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage.ensuresuccessstatuscode
