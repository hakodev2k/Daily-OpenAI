# Incident evidence — certificate review portal

Dữ liệu mô phỏng được ghi **sau** bước xác thực danh tính; không có private key, token thực hoặc thông tin người dùng thật.

| Principal | Token check | Authenticated | Received role values | Review policy |
|---|---|---|---|---|
| reviewer-104 | passed | true | Reviewer | DENY |
| reader-208 | passed | true | Reader | DENY |
| reviewer-310 | passed | true | Auditor, Reviewer | DENY |
| anonymous | not applicable | false | Reviewer (untrusted local claim) | DENY |

Nhóm hỗ trợ đã thử khởi động lại service và làm mới token nhưng triệu chứng không đổi. Logs không có lỗi mạng hay exception. Một bản dump an toàn cho thấy claim type `sub` và `roles` có trong principal. Cần kiểm chứng bằng chạy starter và đối chiếu authorization contract thay vì suy luận từ một cột log.
