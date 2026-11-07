# Senior Challenge
Giả sử gateway trả reading theo thời gian thực, audit phải bền vững và client có thể retry cùng `signalId`. Đề xuất boundary để bảo đảm receipt không báo thành công khi audit chưa durable; nêu idempotency key, xử lý unknown commit outcome, retry và chi phí vận hành. Không bắt buộc triển khai cho lab 55 phút này.
