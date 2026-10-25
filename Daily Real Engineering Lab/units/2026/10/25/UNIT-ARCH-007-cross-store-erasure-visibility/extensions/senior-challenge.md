# Senior challenge — Requirement change

Sau khi ADR hoàn tất, product thay đổi yêu cầu:

- Mở thêm region thứ hai, active-active public search.
- Khi primary region mất kết nối 20 phút, vẫn cần public search trong 30 giây.
- Đơn vị hành chính có quyền gỡ hồ sơ phải thấy hiệu lực trong tối đa 30 giây ở cả hai regions.
- Không được đưa raw PII vào replication log.

Hãy sửa ADR: định nghĩa consistency model, quorum hoặc fail-closed behavior, data residency, RTO/RPO, reconciliation sau partition và mức availability bị đánh đổi. Nêu điều kiện khiến ba yêu cầu mới không thể cùng được đảm bảo trong một network partition. Không cần code.
