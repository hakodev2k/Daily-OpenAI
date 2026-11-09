# Acceptance matrix — điền trước khi xem lời giải

Đánh dấu PASS/FAIL và ghi bằng chứng staging tương ứng.

| Gate | Điều kiện |
|---|---|
| G1 Compatibility | API v1, API v2 và worker v1/v2 có hành vi ghi được xác định trong mỗi pha |
| G2 Historical data | Không còn NULL không được giải quyết; trạm đã xóa được quarantine hoặc phục hồi mapping có căn cứ |
| G3 New writes | Sau cutover, mọi sự kiện mới có zone hợp lệ; không coi sentinel là dữ liệu thật |
| G4 Lock safety | Thử migration trên staging kích thước tương đương; quan sát lock waits, timeouts, replica lag |
| G5 Rollback | Biết rõ rollback code nào còn tương thích với schema; không mất dữ liệu |
| G6 Observability | Có dashboard theo app version, writer, null count, quarantine count, migration progress |
| G7 Exit criteria | Chỉ siết constraint sau khi backfill, kiểm tra writer và thời hạn offline backlog đạt điều kiện |
| G8 Concurrency | Backfill theo batch không ghi đè zone mới hợp lệ và có thể chạy lại an toàn |

**Thử nghiệm bắt buộc trên staging:** old/new writers xen kẽ, delayed offline replay 48h, backfill restart giữa chừng, row station bị xóa, rollback ứng dụng, lock timeout. Đo và ghi nhận số lỗi ghi, row chưa giải quyết và thời gian giữ lock. Không tự tuyên bố PASS nếu chưa chạy staging.