# Senior challenge — requirement change
Đối tác thêm endpoint redirect sang một hostname khác trong cùng tổ chức, và đôi khi timeout sau khi đã lưu receipt.

Hãy đề xuất contract mới: origin allowlist theo cấu hình, HTTPS bắt buộc, idempotency key, truy vấn trạng thái theo evidence ID, timeout/retry budget và cách phân biệt "unknown" với "confirmed". Không triển khai tự động retry mù; đưa ra acceptance tests cho duplicate, partial failure và redirect đến host lạ.