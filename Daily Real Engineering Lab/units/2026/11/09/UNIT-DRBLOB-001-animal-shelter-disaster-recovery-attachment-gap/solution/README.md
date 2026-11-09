# Reference Solution — chỉ đọc sau khi tự điều tra

## Symptoms và evidence
Replica database khôi phục được A-101, A-102, A-103, A-104; object storage tại thời điểm outage chỉ có vax-101 và vax-104. A-102 và A-103 trở thành hồ sơ có metadata nhưng thiếu chứng từ bắt buộc.

## Root cause
RPO được đo riêng cho PostgreSQL; replication của blob không nằm trong cùng consistency boundary. Database foreign key hoặc row commit không bảo đảm object đã được copy sang region DR.

## Fix và lý do
Dùng recovery manifest/watermark gồm database commit boundary và object replication checkpoint, đối soát mọi mandatory blob trước khi publish case. A-101 và A-104 READY; A-102 và A-103 QUARANTINED cho đến khi chứng từ được phục hồi hoặc quy trình nghiệp vụ giải quyết. Không được giả mạo hồ sơ là hoàn chỉnh.

Trong `workspace/decision.json`, chọn `reconcile-and-quarantine` hoặc `cross-store-watermark`, bật ba boolean bảo vệ và giữ đủ bốn case trong drill. Chạy `scripts/verify.ps1` từ thư mục unit; script kiểm tra chính đường dẫn learner-editable.

## Alternative fixes và trade-offs
- **Async replication + quarantine:** chi phí thấp, khả dụng cho hồ sơ đầy đủ, nhưng RPO thực tế phải được công bố theo cả hai store và hồ sơ thiếu có thể tạm thời không truy cập được.
- **Synchronous cross-region blob durability trước khi xác nhận case READY:** giảm khoảng hở nhưng tăng latency, cost, coupling và rủi ro write outage.
- **Transactional outbox + recovery manifest:** cải thiện auditability và recovery ordering, nhưng không tạo atomic transaction xuyên database và object store; vẫn cần readiness check.

## Wrong fixes
Tăng DB replica frequency không làm blob replica nhanh hơn. Trả HTTP 200 với link 404 chỉ che dấu lỗi. Đặt một RPO duy nhất theo database gây hiểu lầm; không tự động xoá hồ sơ thiếu tài liệu.

## Production implications
Định nghĩa RPO cho **complete adoption case**, không chỉ SQL row. Kiểm thử region loss ở các mốc interleaving; log số hồ sơ READY/QUARANTINED, tuổi replication lag, object checksum và orphan objects. RTO tính đến thời gian reconcile; có cảnh báo và manual escalation. Bảo vệ dữ liệu nhạy cảm bằng encryption, IAM least privilege, retention policy và audit trail.

## Senior engineer cần nhận ra
DR success là khả năng phục hồi **business invariant** dưới partial replication, không phải việc database báo healthy. Chọn policy với Product Owner về hồ sơ nào được phép tra cứu trong degraded mode, lập runbook rollback và diễn tập định kỳ.
