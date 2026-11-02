# Scenario — retention của bằng chứng bảo hành

## Hệ thống
PostgreSQL lưu claim, `closedAt`, trạng thái tranh chấp và metadata của object. Object storage lưu trung bình 4 file, mỗi file 6 MiB. Dự kiến 2 triệu claim/năm = 48 triệu MiB (~45.8 TiB) dữ liệu thô/năm trước retention.

## Policy
- Retention: **30 ngày từ `closedAt`**, không tính từ upload.
- Legal hold được hệ thống trả `ACCEPTED` phải bảo vệ bằng chứng khỏi xóa vật lý; hold có thể được release.
- Khi đã hết retention và không còn hold, hoàn tất xóa vật lý trong **72 giờ** tính từ lúc đủ điều kiện. Không bắt buộc chờ đủ 72 giờ mới xóa.
- API đọc phải phân biệt `AVAILABLE`, `DELETION_IN_PROGRESS`, `DELETED`, `EVIDENCE_MISSING`.
- Audit append-only: actor, policy version, hold version, object version, timestamps, attempts, receipts. Không lưu payload/PII trong metric labels.
- Thời gian authoritative lấy từ server UTC. Snapshot trong lab là 03/11/2026 03:13 UTC+7.
- Không được hứa xóa khỏi immutable backup trước khi backup hết hạn; phải có restore policy.

## Constraints
- 2 replica ASP.NET Core, 3 delete workers; queue at-least-once, có thể lặp/đảo thứ tự.
- PostgreSQL là authoritative cho hold và policy; storage delete idempotent nhưng có thể timeout **sau khi đã xóa**.
- Không có distributed transaction PostgreSQL ↔ object storage.
- Hold acceptance p95 dưới 2 giây trong điều kiện bình thường.
- 3 backend engineers + 1 SRE; ưu tiên giải pháp dễ vận hành, không thêm cluster nếu không cần.
- Không cần cloud thật.

## Deliverable
ADR tối đa 2 trang, decision table C101–C108, các transaction boundary và failure windows, 12 acceptance checks, metrics/alerts, phương án DR/restore. So sánh ít nhất ba phương án, nêu điều kiện làm thay đổi quyết định.
