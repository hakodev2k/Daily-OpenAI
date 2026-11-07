# Senior challenge — Requirement change

Sau 3 tháng, portal chuyển từ 2 lên 6 replica, có blue/green deployment, thời hạn ticket 8 giờ, yêu cầu RTO 30 phút. Đội vận hành muốn rotate encryption keys, thu hồi quyền node bị xâm nhập và không mất session khi rolling restart.

Hãy cập nhật ADR: key repository durability, key encryption at rest, access controls, deployment application discriminator, key retention/rotation policy, backup/restore và observability. Chỉ rõ giới hạn: chia sẻ key ring không đồng nghĩa có cơ chế thu hồi tức thì mọi ticket đã phát hành. Đề xuất kiểm thử sự cố mất storage và khôi phục, không sử dụng key thật trong lab.
