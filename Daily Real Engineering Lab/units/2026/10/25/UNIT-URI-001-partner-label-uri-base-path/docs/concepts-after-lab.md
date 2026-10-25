# Sau lab — URI resolution và HTTP route contract
URI gồm scheme, authority, path, query, fragment. `HttpClient.BaseAddress` resolve request URI tương đối. Reference bắt đầu bằng `/` thay thế path từ origin; reference không bắt đầu bằng `/` được resolve theo base path. Nếu base URI không kết thúc bằng `/`, path segment cuối có thể bị thay thế.

Trong tích hợp B2B, API prefix có thể thay đổi theo gateway, tenant hoặc environment. Contract test nên kiểm tra resolved URI bằng `HttpMessageHandler` in-memory, không chỉ kiểm tra string truyền vào client. Khi gặp 404, thu thập method, resolved path, routing contract và status trước khi cân nhắc retry.

Đọc thêm trong tài liệu chính thức .NET về `HttpClient.BaseAddress`, `System.Uri` và RFC 3986 Section 5.
