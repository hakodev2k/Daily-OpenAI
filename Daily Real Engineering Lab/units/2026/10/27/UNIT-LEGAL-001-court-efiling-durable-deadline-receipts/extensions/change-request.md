# Requirement change — phase 2

Sau khi hoàn tất ADR, Product/Legal thông báo:

- Tòa án chấp nhận hồ sơ **đến cổng trước deadline** ngay cả khi storage commit xảy ra vài trăm ms sau đó, **nhưng phải có bằng chứng đáng tin cậy** về thời điểm tiếp nhận.
- Người nộp phải nhận ngay một tracking token trong 250 ms, kể cả khi quorum unavailable.
- RPO=0 cho **confirmed legal receipts** vẫn không đổi. Không được gọi tracking token là receipt.
- Deadline phải chịu được retry và failover, không cho client tự khai timestamp.

## Nhiệm vụ

Đề xuất protocol hai giai đoạn (`PENDING_TRACKING` vs `CONFIRMED_RECEIPT`), trusted ingress timestamp và cách bảo toàn proof khi commit chậm. Nếu không thể vừa đảm bảo 250 ms, vừa có durable ingress evidence và RPO=0 trong partition, hãy **chỉ rõ bất khả thi** và đề xuất thay đổi requirement/SLA thay vì giả vờ đáp ứng. Viết lại contract cho client và failure tests A/B/E.

**Không xem phần extension của reference solution trước khi hoàn thành.**
