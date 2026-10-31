# Senior challenge
Giả lập process crash sau ACK nhưng trước khi consumer persist. Đề xuất hai kiến trúc bảo toàn audit: synchronous durable write trước ACK và transactional outbox. So sánh RPO, throughput, latency, duplicate delivery, idempotency và vận hành. Vì sao bản sửa channel chỉ giải quyết mất event do overflow, không giải quyết crash durability?
