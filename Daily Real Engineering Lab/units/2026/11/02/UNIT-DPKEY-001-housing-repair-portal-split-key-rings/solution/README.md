# Reference Solution — chỉ xem sau khi tự điều tra

## 1. Symptoms
Session ticket giải mã thành công tại replica phát hành và sau restart cùng replica, nhưng thất bại khi load balancer gửi sang replica khác.

## 2. Evidence
Starter trả local=True, crossReplica=False, restart=True, otherAppRejected=True, tamperRejected=True. Chênh lệch phụ thuộc replica chứ không phụ thuộc nội dung ticket, thời điểm expiry hay database. Harness dùng Data Protection thật với hai provider và thư mục tạm riêng.

## 3. Root cause
Mỗi replica lưu key ring vào một thư mục riêng. Cùng purpose và ApplicationName không đủ để giải mã ciphertext nếu key material không được chia sẻ. Data Protection tự tạo key riêng cho từng repository, dẫn tới Unprotect thất bại trên replica còn lại.

## 4. Why the fix works
Reference trong solution/Program.cs dùng cùng thư mục key ring cho hai replica của cùng ứng dụng. ApplicationName vẫn ổn định là housing-repair-portal. Hai provider đọc được cùng key material; provider của ứng dụng quản trị khác vẫn bị cách ly bằng application discriminator. Đây là mô phỏng local, không phải đề xuất dùng shared filesystem thiếu bảo vệ ở production.

## 5. Verification
    ./reproduce.ps1
    # Sửa starter/Program.cs theo giả thuyết của bạn
    ./verify.ps1
    dotnet run --project ./solution/DataProtectionLab.csproj -- --verify

Verify kiểm tra 5 contract: same-node, cross-replica, restart, cross-application rejection và tamper rejection. verify.ps1 chạy starter do learner sửa, không chạy reference.

## 6. Alternative fixes
- Shared durable key repository: lựa chọn mặc định cho các replica thuộc cùng ứng dụng; dùng storage bền vững, quyền tối thiểu và encryption at rest phù hợp.
- Session affinity: có thể giảm lỗi tạm thời nhưng không giải quyết restart, failover, scaling hoặc mất key.
- Centralized session store: hữu ích khi cần server-side session/revocation, nhưng không tự thay thế Data Protection key management cho protected cookies.
- Stateless JWT: thay đổi threat model và validation/rotation/revocation, không phải phép sửa một dòng cho vấn đề hiện tại.

## 7. Wrong or tempting fixes
- Bật sticky sessions và coi như hoàn tất: che lỗi cross-node, dễ vỡ khi node mất.
- Đặt ApplicationName giống nhau cho tất cả ứng dụng: làm yếu isolation boundary, không tạo key material chung.
- Tắt cookie protection hoặc bỏ qua lỗi Unprotect: phá vỡ tính toàn vẹn và bảo mật.
- Tăng session TTL hoặc scale thêm replica: không sửa key mismatch.

## 8. Production implications
Chọn shared repository phù hợp môi trường (ví dụ ASP.NET Core Data Protection với Azure Blob Storage hoặc một durable shared store được hỗ trợ). Khi đổi key persistence mặc định, cần cấu hình bảo vệ key at rest rõ ràng, ví dụ certificate/Key Vault theo môi trường; không nhúng secret vào source. Đặt application discriminator nhất quán trong cùng logical app và khác nhau giữa app cần cách ly. Sao lưu key ring, quản lý quyền, theo dõi Unprotect failures và key rotation. Kiểm tra migration từ key ring cũ để tránh vô hiệu hóa session ngoài ý muốn.

## 9. Trade-offs
Shared ring tăng tính tương thích nhưng trở thành phụ thuộc bảo mật và vận hành. Storage outage, permission drift, key compromise, backup/restore và rotation cần runbook. Local directory chỉ minh họa tính đúng đắn của hợp đồng, không mô phỏng distributed filesystem consistency hay key wrapping.

## 10. What a Senior engineer should notice
Triệu chứng replica-specific là dấu hiệu quan trọng hơn CPU hay DB. Phân biệt cookie authentication, Data Protection purpose, application discriminator, key ring persistence và key-at-rest protection. Xác nhận cả interoperability lẫn isolation; một test cross-node xanh nhưng làm lộ dữ liệu giữa ứng dụng là regression nghiêm trọng. Official docs: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview
