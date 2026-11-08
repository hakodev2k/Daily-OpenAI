# Reference Solution — chỉ đọc sau khi tự điều tra

Đây là **một phương án có thể bảo vệ được**, không phải kiến trúc duy nhất.

## Symptoms và evidence
Lỗi tăng tương quan với số V1 còn chạy, trong khi CPU DB ổn định. Worker V1 retry sau khi API đã lên V2. Rollback app không thể khôi phục hợp đồng schema V1.

## Root cause
Migration phá huỷ đổi tên và kiểu cột trước khi tất cả writer cũ nghỉ. Deploy app và DB không atomic; schema mới không backward-compatible. Migration ngược sau khi V2 đã ghi còn gây mất dữ liệu.

## Phương án chọn: expand → migrate → contract
1. **Expand:** giữ `sms_opt_in`; thêm `contact_preference TEXT NULL` và constraint hợp lệ ở chế độ an toàn. Không drop/rename cột cũ.
2. **Compatibility:** phát hành bản V1.5 (API + worker) có thể đọc cả hai, viết theo protocol tương thích. Chọn một nguồn sự thật cho mỗi giai đoạn, ví dụ old field ở phase A; dual-write được thực hiện trong một transaction kèm audit và optimistic concurrency/version. Nếu legacy V1 vẫn ghi old-only, DB trigger đồng bộ old→new có thể là bridge tạm thời; kiểm thử chiều ngược khi V2 ghi. Không giả định dual-write ở app là đủ khi còn writer V1.
3. **Backfill:** cập nhật theo batch nhỏ, resumable, chỉ cập nhật bản ghi còn thiếu với điều kiện version/updated_at để tránh ghi đè consent mới; đối soát mapping `true→SMS`, `false→NONE`. Theo dõi số row chưa backfill, mismatch và audit.
4. **Cutover:** sau khi mọi writer (kể cả job) hiểu cả hai field, chuyển read preference sang new field bằng feature flag; tiếp tục duy trì old compatibility trong rollback window. Bảo vệ concurrent writes bằng version check và transaction; xử lý retry idempotently theo command ID.
5. **Contract:** chỉ sau khi inventory xác nhận 0 legacy writer, window rollback đóng, mismatch = 0 và backfill hoàn tất mới bỏ old field/trigger/compatibility. Contract là release riêng.

## Verification / acceptance gates
- Replay V1-only write, V2-only write, V1→V2→V1 rollback, worker retry, race backfill vs consent update; assert cùng một consent và audit record đúng.
- Error rate từng version không vượt baseline + 0.5 điểm phần trăm trong canary 30 phút; 0 lỗi unknown-column/type.
- Backfill remaining = 0; mismatch = 0 trên toàn bộ 12 triệu rows trước cutover; alert nếu tiến độ đứng hoặc retry tăng.
- Inventory từ deployment, process/worker heartbeat và scheduler phải xác nhận 0 writer cũ trước contract; chạy rollback drill 10 phút trước khi mở traffic.
- Không được dùng một ngưỡng tổng error rate để che lỗi ở nhóm V1.

## Alternative solutions
- **DB compatibility trigger + app V2:** ít thay đổi app legacy, nhưng tăng độ phức tạp write path, nguy cơ recursion và vận hành trigger. Phải kiểm thử conflict và audit.
- **Versioned API/DB views:** che thay đổi schema với V1, hữu ích khi query contract ổn định; writeable view và migration constraint phức tạp.
- **Maintenance freeze:** đơn giản nhưng vi phạm giới hạn dừng ghi 30 giây và yêu cầu rollback nhanh.

## Wrong / tempting fixes
- Chỉ tăng DB capacity: không giải quyết lỗi contract.
- Chỉ rollback application: DB vẫn không tương thích.
- Chỉ backfill một lần trước rollout: writer V1 còn chạy sẽ tạo divergence.
- Dual-write không có concurrency guard: backfill và concurrent updates có thể ghi đè consent.
- Drop old column ngay khi API V2 đạt 100%: worker và rollback vẫn có thể phụ thuộc V1.

## Production implications / trade-offs
Chi phí là thêm một hoặc hai release và telemetry; đổi lại tránh downtime và giảm rủi ro mất dữ liệu. Trigger tạm thời có thể hợp lý nếu không thể deploy V1.5 trước; phải có owner, thời hạn loại bỏ và test rollback.

## Senior engineer cần nhận ra
Thay đổi schema là protocol evolution giữa nhiều phiên bản writer. Compatibility, observability, concurrency, audit và rollback phải là điều kiện nghiệm thu, không phải bước phụ sau deploy.
