# Scenario và ràng buộc kiến trúc

## Hệ thống hiện tại

Ứng dụng ASP.NET Core ghi trạng thái lô và \`AuditEvents\` vào PostgreSQL. Background export đọc trực tiếp bảng audit để tạo CSV. Nhóm vận hành có quyền DB maintenance; ứng dụng, export worker và các bản sao lưu vận hành dùng cùng miền quản trị. Một dịch vụ tuân thủ khác lưu receipt tóm tắt đã phát hành lúc 09:05 UTC; trong fixture, receipt **chỉ là số liệu đối chiếu, không phải chữ ký mật mã có thể tự kiểm chứng**.

## Business contract

- 12 cơ sở, trung bình 1,2 triệu audit events/ngày, peak 600 events/giây.
- Payload trung bình 700 byte; khoảng 0,84 GB/ngày raw, ~1,53 TB/5 năm trước index, bản sao và metadata.
- p95 ghi trạng thái nghiệp vụ <= 300 ms tại peak; lỗi audit không được tạo ra một quyết định phát hành không có dấu vết có thể truy nguyên.
- Hợp đồng yêu cầu giữ 5 năm; quyền truy cập dữ liệu nhạy cảm phải tách biệt với khả năng xác minh integrity.
- Phát hiện bất nhất đã được ghi nhận trong <= 15 phút; điều tra độc lập một ngày dữ liệu trong <= 30 phút sau khi lấy dữ liệu.
- Nhóm vận hành: 5 kỹ sư, không thể vận hành cluster mật mã tự xây phức tạp.
- Không được khẳng định "tamper-proof" hoặc "exactly once" nếu chưa có mô hình tin cậy và bằng chứng.

## Threat model cần làm rõ

Xét ít nhất: lỗi ứng dụng, duplicate delivery, DBA sửa/xóa bản ghi, operator có quyền backup/restore, kẻ tấn công chiếm API credential, signer bị xâm phạm, mất liên lạc với nơi lưu checkpoint và backfill lịch sử. Phân biệt **phát hiện thay đổi sau khi ghi** với **chứng minh sự kiện thực sự xảy ra**.

## Bằng chứng và phạm vi

- \`evidence/snapshot-t0.csv\`: export được lấy lúc 09:05 UTC.
- \`evidence/snapshot-t1.csv\`: export được lấy lúc 09:31 UTC.
- \`evidence/independent-receipt.json\`: receipt từ hệ thống độc lập.
- \`evidence/incident-timeline.md\`: các quan sát có cả noise.

Các file là fixture đào tạo, không phải forensic evidence thật. Không có secret, chữ ký thật hoặc dữ liệu bệnh nhân.

## Deliverables

Tạo ADR ngắn: mục tiêu và giả định; trust-boundary diagram; bảng so sánh >= 3 phương án; sequence diagram cho write/checkpoint/verify; 8–10 failure-path tests; migration, rollback, alerting, key custody, key rotation, chi phí và giới hạn bảo đảm. Hoàn thành [worksheet](decision-worksheet.md) trước khi đọc solution.
