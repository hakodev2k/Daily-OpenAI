# Spoiler — Reference solution

Nguyên nhân: starter lấy `X-Forwarded-For` do bất kỳ client nào gửi làm nguồn IP đáng tin cậy, cho phép giả mạo office IP. Bản `solution/Program.cs` chỉ minh họa trust boundary với loopback proxy **trong môi trường local**, không phải cấu hình production. Do loopback được xem là proxy tin cậy, replay local vẫn cho 200; điều đó KHÔNG chứng minh bảo vệ chống spoof từ untrusted peer.

Production: sử dụng `ForwardedHeadersMiddleware` với `KnownProxies`/`KnownNetworks` chính xác, `ForwardLimit`, thứ tự middleware phù hợp, chặn truy cập trực tiếp bypass proxy. Không tin bất kỳ header nào khi peer không thuộc danh sách trusted proxies. Dùng network-level controls và authentication/authorization cho admin route; IP allowlist không thay thế authorization. Kiểm tra integration test từ untrusted peer gửi forged header phải 403, từ trusted proxy có forwarded IP hợp lệ mới 200, chain nhiều proxy phải xử lý theo thứ tự an toàn.

Lưu ý: mẫu local sử dụng loopback làm trusted proxy chỉ để minh họa; nếu reverse proxy không chạy trên loopback, phải cấu hình lại. Xem tài liệu Microsoft ASP.NET Core Forwarded Headers Middleware và RFC 7239.
