# Concepts after lab — đọc sau khi tự verify

Data Protection tạo các protector theo purpose và sử dụng key ring để encrypt/authenticate payload. Với provider ephemeral, vật liệu key chỉ sống trong process. Một token có thể hợp lệ ở process phát hành nhưng không thể giải mã ở process khác.

Cấu hình production thường cần một key repository dùng chung giữa replica và một application discriminator ổn định. Chia sẻ key ring không đồng nghĩa chia sẻ mọi purpose: cùng key ring nhưng purpose khác vẫn không giải mã được. Tách ứng dụng hoặc môi trường có thể cần kho key riêng.

Lưu key vào filesystem của pod không đủ nếu filesystem không bền vững hoặc không chia sẻ giữa pod. Local filesystem trong lab là mô phỏng; production cần persistent shared storage phù hợp, giới hạn quyền truy cập, bảo vệ key at rest (ví dụ cơ chế encryptor thích hợp), backup, giám sát, chiến lược rotation/revocation và kiểm thử rolling deployment.

**Phân biệt:** token/session lifetime, key lifetime, key rotation và key revocation là các chính sách khác nhau. Đừng xóa key cũ chỉ để ép logout nếu chưa đánh giá tác động tới các payload còn hợp lệ.
