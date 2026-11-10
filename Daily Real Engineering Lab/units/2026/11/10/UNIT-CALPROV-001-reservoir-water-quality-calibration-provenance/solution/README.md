# Reference solution — Spoiler

## Root cause và reproduce
Bảng cấu hình bị cập nhật tại chỗ trong khi báo cáo cũ được tái tính từ reading IDs. T1: M-71 = 120 × 0.10 = **12.00 NTU**, M-72 = 80 × 0.10 = **8.00 NTU**. T2: 120 × 0.12 = **14.40 NTU**, 80 × 0.12 = **9.60 NTU**. Chênh lệch lần lượt +2.40 và +1.60 NTU. Raw reading không thay đổi; lỗi nằm ở provenance và snapshot của published report, không chỉ ở công thức.

## Proposed architecture
1. `RawReading(readingId PK, sensorId, observedAt, rawADC, ingestedAt, payloadHash)`: append-only; sửa reading bằng correction event, không UPDATE lịch sử.
2. `CalibrationVersion(calibrationId PK, sensorId, validFrom, validTo, recordedAt, factor, offset, approvedBy)`: version bất biến, interval hiệu lực không chồng lấn trong cùng nhánh approved; ghi riêng event correction nếu có backdate.
3. `ReportRevision(reportId, revisionNo, status, createdAt, approvedAt, supersedesRevision, signedPayloadHash, signedPayloadLocation, formulaVersion, roundingPolicy)`: unique(reportId, revisionNo); status draft/approved/published/superseded.
4. `ReportReadingSnapshot(reportId, revisionNo, readingId, calibrationId, rawADC, publishedNTU)`: khóa kép; lưu số liệu đã ký và liên kết nguồn. Payload đã ký được lưu bất biến, không tái tạo JSON tùy ý khi verify hash.
5. `ReportCurrent(reportId PK, revisionNo, rowVersion)`: chỉ là con trỏ mutable, không phải nội dung lịch sử.

## Publish boundary
Trong một transaction: kiểm tra calibration/version hợp lệ, tính decimal theo rounding policy, chụp snapshot, ghi payload bất biến và hash, ghi approval/audit, CAS update con trỏ current theo expected rowVersion. Nếu ký số thực hiện ngoài DB transaction, dùng trạng thái pending + outbox và chỉ chuyển published sau khi signed artifact đã tồn tại; retry theo publication ID để tránh hai revision. Đọc revision cụ thể luôn dùng stored signed payload. Đọc current trả revisionNo và ETag.

## Correction và bitemporal semantics
Hiệu chỉnh hồi tố không làm đổi revision 1. Tạo revision 2 với reason, actor, approval, supersedesRevision=1, effective-time và recorded-time riêng. Cấu hình mới cho M-73 phải được chọn bằng observedAt trong approved valid interval; backdated correction phải chọn theo policy rõ ràng, không tự áp lại lịch sử đã ký. Nếu thiếu provenance lịch sử, ghi `UNKNOWN` và yêu cầu human review.

## Acceptance replay
- A: revision 1 vẫn chứa [12.00, 8.00], H1 bất biến.
- B: nếu M-73 rawADC=100 sau T2, calibration mới cho 12.00 NTU.
- C: revision 2 có số liệu hiệu chỉnh được ký mới; revision 1 vẫn [12.00, 8.00].
- D: unique constraint + CAS/transaction đảm bảo một current; loser retry/review, không silently overwrite.
- E: provenance-unknown, không giả định factor lịch sử.
- F: DB exclusion constraint (PostgreSQL) hoặc serialized validation ngăn overlap; fail closed.
- G: cache key reportId+revisionNo hoặc invalidation sau commit, current lookup ETag.

## Wrong fixes
- **Chỉ sửa factor về 0.10**: phá các phép đo mới và không giải quyết revision.
- **Tính lại mọi báo cáo khi hiệu chuẩn**: làm mất khả năng chứng minh bản đã ký.
- **Chỉ thêm audit log UPDATE**: audit không tự cung cấp signed payload/version cho báo cáo lịch sử.
- **Chỉ xóa cache**: có thể làm sai lệch lộ rõ hơn, không sửa provenance.

## Trade-offs
Snapshot chiếm storage nhưng cho truy xuất và kiểm toán ổn định; versioned calibration + raw data cho reproducibility nhưng cần chính sách xử lý bitemporal correction. Ký ngoài transaction đòi hỏi outbox và reconciliation; ký trong transaction có thể kéo dài lock. Migration legacy cần phân loại mức độ tin cậy, không “khôi phục” lịch sử bằng suy đoán.
